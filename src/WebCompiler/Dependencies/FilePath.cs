using System;

namespace WebCompiler
{
    public class FilePath : IEquatable<FilePath>
    {
        public FilePath(string path)
        {
            Lowercase = path.ToLowerInvariant();
            Original = path;
        }

        public string Lowercase { get; }
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

            return Lowercase == other.Lowercase;
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
            return Lowercase != null ? Lowercase.GetHashCode() : 0;
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
