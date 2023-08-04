using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    class SassDependencyResolver : DependencyResolverBase
    {
        private Regex importsReg = new Regex(@"(?<=@import|@use|@forward(?:[\s]+))(?:(?:\(\w+\)))?\s*(?:url)?(?<url>[^;]+)", RegexOptions.Compiled|RegexOptions.Multiline);
        private static Regex filesReg = new Regex(@"(?:(?!\bas\b|\bwith\b)[\s\S])*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override string[] SearchPatterns
        {
            get { return new[] { "*.scss", "*.sass" }; }
        }

        public override string FileExtension
        {
            get
            {
                return ".scss";
            }
        }


        /// <summary>
        /// Updates the dependencies of a single file
        /// </summary>
        /// <param name="path"></param>
        public override void UpdateFileDependencies(FilePath path)
        {
            if ( Dependencies == null )
            {
                return;
            }

            FileInfo info = new FileInfo(path.Original);
            path = new FilePath(info.FullName);

            if (!Dependencies.ContainsKey(path))
                Dependencies[path] = new Dependencies();

            //remove the dependencies registration of this file
            Dependencies[path].DependentOn = new HashSet<FilePath>();
            //remove the dependentfile registration of this file for all other files
            foreach (var dependenciesPath in Dependencies.Keys)
            {
                if (Dependencies[dependenciesPath].DependentFiles.Contains(path))
                {
                    Dependencies[dependenciesPath].DependentFiles.Remove(path);
                }
            }

            Console.WriteLine($"Reading content for {info.FullName}");
            string content = File.ReadAllText(info.FullName);
            Console.WriteLine($"Content: {content}");

            //match both <@<type> "myFile.scss";> and <@<type> url("myFile.scss");> syntax (where supported)
            foreach (Match match in importsReg.Matches(content))
            {
                var importedfiles = GetFileInfos(info, match).ToList();
                Console.WriteLine($"Imported files:{Environment.NewLine}{string.Join($"{Environment.NewLine}{Environment.NewLine}", importedfiles.Select(f => f.FullName))}");

                foreach (FileInfo importedfile in importedfiles)
                {
                    if (importedfile == null)
                        continue;

                    var theFile = importedfile;

                    //if the file doesn't end with the correct extension, an import statement without extension is probably used, to re-add the extension (#175)
                    if (string.Compare(importedfile.Extension, FileExtension, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        theFile = new FileInfo(importedfile.FullName + FileExtension);
                    }

                    var dependencyFilePath = new FilePath(theFile.FullName);

                    if (!File.Exists(dependencyFilePath.Original))
                    {
                        // Trim leading underscore to support Sass partials
                        var dir = Path.GetDirectoryName(dependencyFilePath.Original);
                        var fileName = Path.GetFileName(dependencyFilePath.Original);
                        var cleanPath = Path.Combine(dir, "_" + fileName);

                        if (!File.Exists(cleanPath))
                            continue;

                        dependencyFilePath = new FilePath(cleanPath);
                    }

                    if (!Dependencies[path].DependentOn.Contains(dependencyFilePath))
                        Dependencies[path].DependentOn.Add(dependencyFilePath);

                    if (!Dependencies.ContainsKey(dependencyFilePath))
                        Dependencies[dependencyFilePath] = new Dependencies();

                    if (!Dependencies[dependencyFilePath].DependentFiles.Contains(path))
                        Dependencies[dependencyFilePath].DependentFiles.Add(path);
                }
            }
        }

        private static IEnumerable<FileInfo> GetFileInfos(FileInfo info, System.Text.RegularExpressions.Match match)
        {
            var url = filesReg.Matches(match.Groups["url"].Value)
                .OfType<Match>()
                .FirstOrDefault()?
                .Value
                .Replace("'", "\"").Replace("(", "").Replace(")", "").Replace(";", "") ?? string.Empty;

            var list = new List<FileInfo>();

            foreach (string name in url.Split(new[] { "\"," }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    string value = name.Replace("\"", "").Replace('/', Path.DirectorySeparatorChar).Trim();
                    list.Add(new FileInfo(Path.Combine(info.DirectoryName, value)));
                }
                catch (Exception ex)
                {
                    // Not a valid file name
                    System.Diagnostics.Debug.Write(ex);
                }
            }

            return list;
        }
    }
}
