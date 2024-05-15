using System.Security.Cryptography;
using System.Text;
using File = System.IO.File;

namespace Sha256.oneThread
{
    public class HashFiles
    {
        private static ExcelHelper.ExcelCreator excelHelper = new();
        private static string _rootFileSrc = "";

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("You have to specified a source and a destination folder to compare them.");
                return;
            }

            _rootFileSrc = args[0];
            string dstDirectory = args[1];

            if (Directory.Exists(_rootFileSrc) && Directory.Exists(dstDirectory))
            {
                Console.WriteLine($"Source : {_rootFileSrc}");
                Console.WriteLine($"Destination : {dstDirectory}");

                SeekFiles(_rootFileSrc, dstDirectory);
                excelHelper.SaveExcel();
            }
            else
            {
                Console.Error.WriteLine("The directory specified could not be found.");
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
                Console.Error.WriteLine($"I/O exception : {e.Message}");
            }
            return null;
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

        private static bool SeekFiles(string rootSrc, string rootDst)
        {
            try
            {
                foreach (var file in Directory.GetFiles(rootSrc))
                {
                    var destinationFilePath = GetDestinationFilePath(_rootFileSrc, rootDst, file);
                    FileCompare(destinationFilePath, file);
                }

                foreach (var folder in Directory.GetDirectories(rootSrc))
                {
                    SeekFiles(folder, rootDst);

                }
                return true;
            }
            catch (UnauthorizedAccessException e)
            {
                LogFileError(rootSrc, e.Message, excelHelper);
                return false;
            }
            catch (Exception e)
            {
                LogFileError(rootSrc, e.Message, excelHelper);
                return false;
            }
        }

        private static string GetDestinationFilePath(string rootSrc, string rootDst, string file)
        {
            string relativePathDest = Path.GetRelativePath(rootSrc, file);
            return Path.Combine(@"\\?\", rootDst, relativePathDest);
        }

        public static void FileCompare(string destinationFilePath, string file)
        {
            try
            {
                if (!File.Exists(destinationFilePath))
                {
                    LogFileError(destinationFilePath, "File not found", excelHelper: excelHelper);
                    return;
                }

                string hashSrc = HashFile(file);
                string hashDest = HashFile(destinationFilePath);

                string fileSizeSrc = new FileInfo(file).Length.ToString();
                string fileSizeDst = new FileInfo(destinationFilePath).Length.ToString();
                string formatByte = $"{fileSizeDst} bytes";

                if (hashSrc != hashDest)
                {
                    LogFileError(destinationFilePath, "Not the same Hash", excelHelper, hashDest, formatByte);
                    return;
                }

                if (fileSizeSrc.Length != fileSizeDst.Length)
                {
                    LogFileError(destinationFilePath, "Not the same content", excelHelper, hashDest, formatByte);
                    return;
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Access to file {file} was denied: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while processing file {file}: {e.Message}");
            }
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
