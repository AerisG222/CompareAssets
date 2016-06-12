using System;
using System.Collections.Generic;


namespace CompareAssets
{
    abstract class ListComparison
    {
        public IEnumerable<string> NotInCompare { get; protected set; }
        public IEnumerable<string> NotInSoruce { get; protected set; }
        public IEnumerable<string> Common { get; protected set; }

        public abstract void Compare();
    } 
}