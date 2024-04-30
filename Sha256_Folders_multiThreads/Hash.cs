using System.IO;
using System.Security.Cryptography;
using System.Text;
using File = System.IO.File;

namespace Sha256.Sha256_Folders_multiThreads
{
    public class HashFiles
    {
        static WriteFile.ErrorFileWriter errorLine = new();
        static WriteFile.OptimizedFileWriter fileWriter = new();

        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No Specified folder");
                return;
            }

            string directory = args[0];
            if (Directory.Exists(directory))
            {
                Console.WriteLine($"Root : {directory}");
                ProcessDirectory(directory);

                errorLine.SaveToFile();
                fileWriter.SaveToFile();
            }
            else
            {
                errorLine.AddContent("The directory specified could not be found.");
            }
        }

        private static void ProcessDirectory(string directory)
        {
            try
            {
                var filePath = Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories);

                Parallel.ForEach(filePath, HashFile);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message;
                Console.Error.WriteLine(error);
                errorLine.AddContent($"An error occurred : {error}");
            }
        }

        private static void HashFile(string filePath)
        {
            if (new FileInfo(filePath).Length == 0)
            {
                fileWriter.AddContent("Empty File");
                return;
            }

            try
            {
                Console.WriteLine("{0}, Thread Id= {1}", filePath, Thread.CurrentThread.ManagedThreadId);

                using SHA256 mySha256 = SHA256.Create();
                using FileStream fileStream = File.Open(filePath, FileMode.Open);
                byte[] hashValue = mySha256.ComputeHash(fileStream);
                PrintByteArray(hashValue, filePath);
            }
            catch (IOException e)
            {
                errorLine.AddContent($"I/O exception : {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                errorLine.AddContent($"Access denied : {e.Message}");
            }
        }

        public static void PrintByteArray(byte[] array, string filePath)
        {
            StringBuilder sb = new ();
            foreach (var t in array)
            {
                sb.Append($"{t:X2}");
            }

            fileWriter.AddContent($"{Path.GetFileName(filePath)}: {sb}\n");
        }
    }
}
