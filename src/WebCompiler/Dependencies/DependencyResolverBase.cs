using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebCompiler
{
    /// <summary>
    /// Base class for file dependency resolver
    /// </summary>
    public abstract class DependencyResolverBase
    {
        private Dictionary<FilePath, Dependencies> _dependencies;

        /// <summary>
        /// Stores all resolved dependencies
        /// </summary>
        protected Dictionary<FilePath, Dependencies> Dependencies
        {
            get
            {
                return _dependencies;
            }
        }

        /// <summary>
        /// The search patterns to use to determine what files should be used to build the dependency tree
        /// </summary>
        public abstract string[] SearchPatterns
        {
            get;
        }

        /// <summary>
        /// The file extension of files of this type
        /// </summary>
        public abstract string FileExtension
        {
            get;
        }

        /// <summary>
        /// Gets the dependency tree
        /// </summary>
        /// <returns></returns>
        public Dictionary<FilePath, Dependencies> GetDependencies(string projectRootPath)
        {
            if (_dependencies == null)
            {
                _dependencies = new Dictionary<FilePath, Dependencies>();

                List<FilePath> files = new List<FilePath>();
                foreach (var pattern in SearchPatterns)
                {
                    var foundFiles = Directory.GetFiles( projectRootPath, pattern, SearchOption.AllDirectories )
                        .Select( f => new FilePath( f ) )
                        .ToList();
                    files.AddRange(foundFiles);
                }

                foreach (var path in files)
                {
                    UpdateFileDependencies(path);
                }
            }

            return _dependencies;
        }

        /// <summary>
        /// Updates the dependencies for the given file
        /// </summary>
        public abstract void UpdateFileDependencies(FilePath path);
    }
}
