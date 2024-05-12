using System.Security.Cryptography;
using System.Text;
using File = System.IO.File;

namespace Sha256.oneThread
{
    public class HashFiles
    {
        static WriteFile.ErrorFileWriter errorLine = new();

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("You have to specified a source and a destination folder to compare them.");
                return;
            }

            string srcDirectory = args[0];
            string dstDirectory = args[1];

            if (Directory.Exists(srcDirectory) && Directory.Exists(dstDirectory))
            {
                Console.WriteLine($"Source : {srcDirectory}");
                Console.WriteLine($"Destination : {dstDirectory}");

                FileCompare(srcDirectory, dstDirectory);

                Console.WriteLine("\nThe program is finished.");
            }
            else
            {
                errorLine.AddContent("The directory specified could not be found.");
            }
        }

        private static string HashFile(string filePath)
        {
            try
            {
                using SHA256 mySha256 = SHA256.Create();
                using FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                byte[] hashValue = mySha256.ComputeHash(fileStream);
                return PrintByteArray(hashValue);
            }
            catch (IOException e)
            {
                errorLine.AddContent($"I/O exception : {e.Message}");
            }
            return null;
        }

        public static void FileCompare(string srcFile, string dstFile)
        {
            var lastFileError = "";

            ExcelHelper.ExcelCreator excelHelper = new();
            try
            {
                foreach (var file in Directory.EnumerateFiles(srcFile, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        string relativePathDest = Path.GetRelativePath(srcFile, file);
                        string destinationFilePath = Path.Combine(@"\\?\", dstFile, relativePathDest);

                        if (!File.Exists(destinationFilePath))
                        {
                            LogFileError(destinationFilePath, "File not found", excelHelper: excelHelper);
                            continue;
                        }

                        string hashSrc = HashFile(file);
                        string hashDest = HashFile(destinationFilePath);

                        string fileSizeSrc = new FileInfo(file).Length.ToString();
                        string fileSizeDst = new FileInfo(destinationFilePath).Length.ToString();
                        string formatByte = $"{fileSizeDst} bytes";

                        if (hashSrc != hashDest)
                        {
                            LogFileError(destinationFilePath, "Not the same Hash", excelHelper, hashDest, formatByte);
                            continue;
                        }

                        if (fileSizeSrc.Length != fileSizeDst.Length)
                        {
                            LogFileError(destinationFilePath, "Not the same content", excelHelper, hashDest, formatByte);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while processing file {file}: {ex.Message}");
                    }
                    finally
                    {
                        lastFileError = file;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                LogFileError(lastFileError, $"Error last source file : {e.Message}", excelHelper);
            }
            finally
            {
                excelHelper.SaveExcel();
            }
        }

        public static string PrintByteArray(byte[] array)
        {
            StringBuilder sb = new();
            foreach (var t in array)
            {
                sb.Append($"{t:X2}");
            }
            return sb.ToString();
        }

        private static void LogFileError(string destinationFilePath, string errorType, ExcelHelper excelHelper, string? hashFile = null, string? fileSize = null)
        {
            if (hashFile == null && fileSize == null)
            {
                excelHelper.WriteContent([destinationFilePath, "None", "None", errorType,]);
            }
            else
            {
                excelHelper.WriteContent([destinationFilePath, hashFile, fileSize, errorType,]);
            }
        }
    }
}
