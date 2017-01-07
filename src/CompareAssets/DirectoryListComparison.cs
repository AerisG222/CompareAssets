using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace CompareAssets
{
    class DirectoryListComparison
        : ListComparison
    {
        string[] _srcDirs;
        string[] _cmpDirs;


        public DirectoryListComparison(string[] srcDirs, string[] cmpDirs)
        {
            _srcDirs = srcDirs;
            _cmpDirs = cmpDirs;
        }


        public override void Compare()
        {
            var srcDirs = GetDirectoryNames(_srcDirs);
            var cmpDirs = GetDirectoryNames(_cmpDirs);
            
            NotInCompare = srcDirs.Except(cmpDirs);
            NotInSource = cmpDirs.Except(srcDirs);
            Common = srcDirs.Intersect(cmpDirs);
        }


        IEnumerable<string> GetDirectoryNames(string[] dirs)
        {
            foreach(var dir in dirs)
            {
                yield return new DirectoryInfo(dir).Name;
            }
        }
    }
}
