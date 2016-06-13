using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


        public bool Compare()
        {
            var levelIdentifier = new LevelIdentifier();

            var srcLevel = levelIdentifier.DetermineComparisonLevel(_srcDir);
            var cmpLevel = levelIdentifier.DetermineComparisonLevel(_cmpDir);

            if(srcLevel == ComparisonLevel.Unknown)
            {
                Console.WriteLine("Unable to determine the type of directory for comparison!");
                return false;
            }

            if(srcLevel != cmpLevel)
            {
                Console.WriteLine("It looks like you are trying to compare directories at different levels.  Please try again.");
                return false;
            }

            if(srcLevel == ComparisonLevel.All)
            {
                Console.WriteLine("Comparing all...");
                return CompareAll(_srcDir, _cmpDir);
            }
            else if(srcLevel == ComparisonLevel.Year)
            {
                Console.WriteLine("Comparing year...");
                return CompareYear(_srcDir, _cmpDir);
            }
            else if(srcLevel == ComparisonLevel.Category)
            {
                Console.WriteLine("Comparing category...");
                return CompareCategory(_srcDir, _cmpDir);
            }

            throw new Exception("Should not get here");
        }


        bool CompareAll(string srcDir, string cmpDir)
        {
            var srcYearDirs = Directory.GetDirectories(_srcDir);
            var cmpYearDirs = Directory.GetDirectories(_cmpDir);

            return CompareYears(srcYearDirs, cmpYearDirs);
        }


        bool CompareYears(string[] srcYearDirs, string[] cmpYearDirs)
        {
            var result = true;
            var count = Math.Max(srcYearDirs.Length, cmpYearDirs.Length);
            
            var cmp = new DirectoryListComparison(srcYearDirs, cmpYearDirs);
            cmp.Compare();

            if(cmp.NotInCompare.Count() > 0 || cmp.NotInSource.Count() > 0)
            {
                ReportListDifferences(cmp);
                result = false;
            }

            foreach(var dir in cmp.Common)
            {
                var srcDir = srcYearDirs.Single(x => x.EndsWith(dir));
                var cmpDir = cmpYearDirs.Single(x => x.EndsWith(dir));

                if(!CompareYear(srcDir, cmpDir))
                {
                    result = false;
                }
            }

            return result;
        }


        bool CompareYear(string srcDir, string cmpDir)
        {
            Console.WriteLine($"    ** {new DirectoryInfo(srcDir).Name} **");

            var srcCategoryDirs = Directory.GetDirectories(_srcDir);
            var cmpCategoryDirs = Directory.GetDirectories(_cmpDir);

            return CompareCategories(srcCategoryDirs, cmpCategoryDirs);
        }


        bool CompareCategories(string[] srcCategoryDirs, string[] cmpCategoryDirs)
        {
            var result = true;
            var count = Math.Max(srcCategoryDirs.Length, cmpCategoryDirs.Length);
            
            var cmp = new DirectoryListComparison(srcCategoryDirs, cmpCategoryDirs);
            cmp.Compare();

            if(cmp.NotInCompare.Count() > 0 || cmp.NotInSource.Count() > 0)
            {
                ReportListDifferences(cmp);
                result = false;
            }

            foreach(var dir in cmp.Common)
            {
                var srcDir = srcCategoryDirs.Single(x => x.EndsWith(dir));
                var cmpDir = cmpCategoryDirs.Single(x => x.EndsWith(dir));

                if(!CompareCategory(srcDir, cmpDir))
                {
                    result = false;
                }
            }

            return result;
        }


        bool CompareCategory(string srcDir, string cmpDir)
        {
            var result = true;
            var match = 0;
            var totalCount = 0;

            Console.WriteLine($"        ** {new DirectoryInfo(srcDir).Name} **");

            // old approach kept the original in the 'raw' or 'orig', so this takes a little more work
            var srcFiles = GetSourceFiles(srcDir).ToArray();
            // new approach always has the untouched original in the 'src' directory
            var cmpFiles = Directory.GetFiles(Path.Combine(cmpDir, "src"));

            var cmp = new FileListComparison(srcFiles, cmpFiles);
            cmp.Compare();

            if(cmp.NotInCompare.Count() > 0 || cmp.NotInSource.Count() > 0)
            {
                ReportListDifferences(cmp);
                result = false;
            }

            foreach(var file in cmp.Common)
            {
                totalCount++;

                var srcFile = srcFiles.Single(x => x.EndsWith(file));
                var cmpFile = cmpFiles.Single(x => x.EndsWith(file));

                if(CompareFiles(srcFile, cmpFile))
                {
                    match++;
                }
                else
                {
                    result = false;
                }
            }

            Console.WriteLine($"            => {match} of {totalCount} files match");

            return result;
        }


        bool CompareFiles(string srcFile, string cmpFile)
        {
            var srcHash = CalculateHash(srcFile);
            var cmpHash = CalculateHash(cmpFile);

            if(!string.Equals(srcHash, cmpHash, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"            no match:  [{Path.GetFileName(srcFile)}]");
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


        void ReportListDifferences(ListComparison cmp)
        {
            string type = cmp is DirectoryListComparison ? "directories" : "files";

            if(cmp.NotInCompare.Count() > 0)
            {
                Console.WriteLine($"            * {type} not found in compare:");

                foreach(var item in cmp.NotInCompare)
                {
                    Console.WriteLine($"                - {item}");
                }
            }

            if(cmp.NotInSource.Count() > 0)
            {
                Console.WriteLine($"            * {type} not found in source:");

                foreach(var item in cmp.NotInSource)
                {
                    Console.WriteLine($"                - {item}");
                }
            }
        }


        IEnumerable<string> GetSourceFiles(string srcDir)
        {
            var list = new List<string>();
            IEnumerable<string> rawFiles = new List<string>();
            IEnumerable<string> origFiles = new List<string>();
            IEnumerable<string> rawFileNames = new List<string>();
            IEnumerable<string> origFileNames = new List<string>();

            var rawDir = Path.Combine(srcDir, "raw");
            var origDir = Path.Combine(srcDir, "orig");
            
            if(Directory.Exists(rawDir))
            {
                rawFiles = Directory.GetFiles(rawDir).ToList();
                rawFileNames = GetFileNames(rawFiles);
            }

            if(Directory.Exists(origDir))
            {
                origFiles = Directory.GetFiles(origDir).ToList();
                origFileNames = GetFileNames(origFiles);
            }

            // add all raw files
            list.AddRange(rawFiles);

            var uniqueOrigFileNames = origFileNames.Except(rawFileNames).ToList();

            // add all orig files that are not based on raw files
            list.AddRange(origFiles.Where(x => uniqueOrigFileNames.Contains(Path.GetFileNameWithoutExtension(x))));

            return list;
        }


        IEnumerable<string> GetFileNames(IEnumerable<string> files)
        {
            foreach(var file in files)
            {
                yield return Path.GetFileNameWithoutExtension(file);
            }
        }
    }
}
