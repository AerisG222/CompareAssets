using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace CompareAssets
{
    class FileListComparison
        : ListComparison
    {
        string[] _srcFiles;
        string[] _cmpFiles;


        public FileListComparison(string[] srcFiles, string[] cmpFiles)
        {
            _srcFiles = srcFiles;
            _cmpFiles = cmpFiles;
        }


        public override void Compare()
        {
            var srcFiles = GetFileNames(_srcFiles);
            var cmpFiles = GetFileNames(_cmpFiles);
            
            NotInCompare = srcFiles.Except(cmpFiles);
            NotInSoruce = cmpFiles.Except(srcFiles);
            Common = srcFiles.Intersect(cmpFiles);
        }


        IEnumerable<string> GetFileNames(string[] files)
        {
            foreach(var file in files)
            {
                yield return new FileInfo(file).Name;
            }
        }
    }
}
