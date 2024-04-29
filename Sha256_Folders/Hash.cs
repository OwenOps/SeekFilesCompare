using System.IO;
using System.Security.Cryptography;
using File = System.IO.File;

namespace Sha256.Sha256_Folders
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
                foreach (string filePath in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
                {
                    HashFile(filePath);
                }
            }
            catch (UnauthorizedAccessException)
            {
                errorLine.AddContent($"Access denied for the file : {directory}");
            }
            catch (DirectoryNotFoundException)
            {
                errorLine.AddContent($"File not found : {directory}");
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
                using (SHA256 mySha256 = SHA256.Create())
                using (FileStream fileStream = File.Open(filePath, FileMode.Open))
                {
                    byte[] hashValue = mySha256.ComputeHash(fileStream);
                    fileWriter.AddContent($"{Path.GetFileName(filePath)}: ");
                    PrintByteArray(hashValue);
                }
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

        public static void PrintByteArray(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                fileWriter.AddContent($"{array[i]:X2}");
            }
            fileWriter.AddContent("\n");
        }
    }
}
