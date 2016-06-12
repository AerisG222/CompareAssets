using System;
using System.IO;
using System.Linq;


namespace CompareAssets
{
    class LevelIdentifier
    {
        public ComparisonLevel DetermineComparisonLevel(string dir)
        {
            var dirInfo = new DirectoryInfo(dir);

            if(CheckIfCategoryLevel(dirInfo))
            {
                return ComparisonLevel.Category;
            }

            if(CheckIfYearLevel(dirInfo))
            {
                return ComparisonLevel.Year;
            }

            if(CheckIfAllLevel(dirInfo))
            {
                return ComparisonLevel.All;
            }

            return ComparisonLevel.Unknown;
        }


        bool CheckIfCategoryLevel(DirectoryInfo dir)
        {
            var dirs = dir.GetDirectories();

            if(dirs.Count(x => string.Equals(dir.Name, "orig", StringComparison.OrdinalIgnoreCase) || 
                               string.Equals(dir.Name, "src", StringComparison.OrdinalIgnoreCase)) > 0)
            {
                return true;
            }

            return false;
        }


        bool CheckIfYearLevel(DirectoryInfo dir)
        {
            int year;

            if(int.TryParse(dir.Name, out year))
            {
                var dirs = dir.GetDirectories();

                if(dirs.Length > 0)
                {
                    if(CheckIfCategoryLevel(dirs[0]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        bool CheckIfAllLevel(DirectoryInfo dir)
        {
            var dirs = dir.GetDirectories();

            if(dirs.Length == 0)
            {
                return false;
            }

            return CheckIfYearLevel(dirs[0]);
        }
    }
}
