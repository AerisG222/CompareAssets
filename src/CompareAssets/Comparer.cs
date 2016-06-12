using System;
using System.IO;
using Amazon.Glacier;


namespace CompareAssets
{
    class Comparer
    {
        readonly string _srcDir;
        readonly string _cmpDir;


        public Comparer(string sourceDirectory, string compareDirectory)
        {
            _srcDir = sourceDirectory;
            _cmpDir = compareDirectory;
        }


        public void Compare()
        {
            var levelIdentifier = new LevelIdentifier();

            var srcLevel = levelIdentifier.DetermineComparisonLevel(_srcDir);
            var cmpLevel = levelIdentifier.DetermineComparisonLevel(_cmpDir);

            if(srcLevel == ComparisonLevel.Unknown)
            {
                Console.WriteLine("Unable to determine the type of directory for comparison!");
                return;
            }

            if(srcLevel != cmpLevel)
            {
                Console.WriteLine("It looks like you are trying to compare directories at different levels.  Please try again.");
                return;
            }

            if(srcLevel == ComparisonLevel.All)
            {
                var srcYearDirs = Directory.GetDirectories(_srcDir);
                var cmpYearDirs = Directory.GetDirectories(_cmpDir);

                CompareYears(srcYearDirs, cmpYearDirs);
            }
            else if(srcLevel == ComparisonLevel.Year)
            {
                var srcCategoryDirs = Directory.GetDirectories(_srcDir);
                var cmpCategoryDirs = Directory.GetDirectories(_cmpDir);

                CompareCategories(srcCategoryDirs, cmpCategoryDirs);
            }
            else if(srcLevel == ComparisonLevel.Category)
            {
                CompareCategory(_srcDir, _cmpDir);
            }
        }


        bool CompareYears(string[] srcYearDirs, string[] cmpYearDirs)
        {
            return false;
        }


        bool CompareCategories(string[] srcCategoryDirs, string[] cmpCategoryDirs)
        {
            return false;
        }


        bool CompareCategory(string srcDir, string cmpDir)
        {
            return false;
        }


        bool CompareFiles(string srcFile, string cmpFile)
        {
            var srcHash = CalculateHash(srcFile);
            var cmpHash = CalculateHash(cmpFile);

            if(!string.Equals(srcHash, cmpHash, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Files did not match!  [{srcFile}] [{cmpFile}]");
                return false;
            }

            return true;
        }


        string CalculateHash(string file)
        {
            using(var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return TreeHashGenerator.CalculateTreeHash(fs);
            }
        }
    }
}
