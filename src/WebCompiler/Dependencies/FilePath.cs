using System;
using System.IO;

namespace WebCompiler
{
    public class FilePath : IEquatable<FilePath>
    {
        public FilePath(string path)
        {
            Normalized = path.ToLowerInvariant().Replace( "/", Path.DirectorySeparatorChar.ToString() );
            Original = path;
        }

        public string Normalized { get; }
        public string Original { get; }

        public bool Equals( FilePath other )
        {
            if ( ReferenceEquals( null, other ) )
            {
                return false;
            }

            if ( ReferenceEquals( this, other ) )
            {
                return true;
            }

            return Normalized == other.Normalized;
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) )
            {
                return false;
            }

            if ( ReferenceEquals( this, obj ) )
            {
                return true;
            }

            if ( obj.GetType() != GetType() )
            {
                return false;
            }

            return Equals( (FilePath)obj );
        }

        public override int GetHashCode()
        {
            return Normalized != null ? Normalized.GetHashCode() : 0;
        }

        public static bool operator ==( FilePath left, FilePath right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( FilePath left, FilePath right )
        {
            return !Equals( left, right );
        }
    }
}
