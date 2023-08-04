using System.Collections.Generic;

namespace WebCompiler
{
    /// <summary>
    /// Contains dependency information (on what file is the current file dependent, what other files are dependent on this file) for a file
    /// </summary>
    public class Dependencies
    {
        /// <summary>
        /// Contains all files the current file is dependent ont
        /// </summary>
        public HashSet<FilePath> DependentOn { get; set; } = new HashSet<FilePath>();

        /// <summary>
        /// Contains all files that are dependent on this file
        /// </summary>
        public HashSet<FilePath> DependentFiles { get; set; } = new HashSet<FilePath>();
    }
}