using System;
using System.IO;


namespace CompareAssets
{
    public class Program
    {
        static void Main(string[] args)
        {
            bool executeCompare = true;

            if(args.Length != 2)
            {
                ShowUsage();
                Environment.Exit(1);
            }

            if(!Directory.Exists(args[0]))
            {
                Console.WriteLine($"Source directory [{args[0]}] does not exist.");
                executeCompare = false;
            }

            if(!Directory.Exists(args[1]))
            {
                Console.WriteLine($"Compare directory [{args[1]}] does not exist.");
                executeCompare = false;
            }

            if(executeCompare)
            {
                var comparer = new Comparer(args[0], args[1]);

                comparer.Compare();
            }
        }


        static void ShowUsage()
        {
            Console.WriteLine("CompareAssets <src_dir> <compare_dir>");
            Console.WriteLine("  where:");
            Console.WriteLine("    src_dir = directory containing source images.  this can be the photo root, a specific year, or category");
            Console.WriteLine("    compare_dir = directory to compare - at the same level as the source directory");
        }
    }
}
