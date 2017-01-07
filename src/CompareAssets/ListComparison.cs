using System.Collections.Generic;


namespace CompareAssets
{
    abstract class ListComparison
    {
        public IEnumerable<string> NotInCompare { get; protected set; }
        public IEnumerable<string> NotInSource { get; protected set; }
        public IEnumerable<string> Common { get; protected set; }

        public abstract void Compare();
    } 
}
