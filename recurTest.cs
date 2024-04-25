/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace recursiveFolder
{
    public class recursiveFolder
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No Specified folder");
                return;
            }

            Console.WriteLine($@"{args[0]}");
            ShowAllFoldersUnder($@"{args[0]}", 0);
        }

        private static void ShowAllFoldersUnder(string path, int indent)
        {
            try
            {
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint)
                    != FileAttributes.ReparsePoint)
                {
                    foreach (string folder in Directory.GetDirectories(path))
                    {
                        Console.WriteLine("{0}{1}", new string(' ', indent), Path.GetFileName(folder));
                        ShowAllFoldersUnder(folder, indent + 2);
                    }
                }
            }
            catch (UnauthorizedAccessException) { }
        }
    }
}
*/