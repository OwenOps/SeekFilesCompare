using SeekFilesCompare.mail;
using SeekFilesCompare.timer;

namespace SeekFilesCompare.multiThreads
{
    public class SeekFiles
    {
        private static ExcelHelper.ExcelCreator excelHelper = new();
        private const int MAX_THREADS = 25;
        public static string RootFileSrc = "";

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("You have to specified a source and a destination folder to compare them.");
                return;
            }

            RootFileSrc = args[0];
            string dstDirectory = args[1];

            TimerSeek.Start();

            if (Directory.Exists(RootFileSrc) && Directory.Exists(dstDirectory))
            {
                Console.WriteLine($"Source : {RootFileSrc}");
                Console.WriteLine($"Destination : {dstDirectory}");

                Seek(RootFileSrc, dstDirectory);
                excelHelper.SaveExcel();
            }
            else
            {
                Console.Error.WriteLine("The directory specified could not be found.");
            }

            TimerSeek.Stop();

            Console.WriteLine("\n----------The Program is finished----------");
            //SendMail.SendAMail();
        }

        private static bool Seek(string rootSrc, string rootDst)
        {
            try
            {
                foreach (var file in Directory.GetFiles(rootSrc))
                {
                    var destinationFilePath = GetDestinationFilePath(RootFileSrc, rootDst, file);
                    FileCompare(destinationFilePath, file);
                }

                string[] folders = Directory.GetDirectories(rootSrc);
                Parallel.ForEach(folders, new ParallelOptions { MaxDegreeOfParallelism = MAX_THREADS }, (folder) =>
                {
                    Seek(folder, rootDst);
                });

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

        public static void FileCompare(string destinationFilePath, string file)
        {
            try
            {
                Console.WriteLine(file);
                if (!File.Exists(destinationFilePath))
                {
                    LogFileError(destinationFilePath, "File not found", excelHelper: excelHelper);
                    return;
                }

                string hashSrc = HashFiles.HashFile(file);
                string hashDest = HashFiles.HashFile(destinationFilePath);

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

        private static string GetDestinationFilePath(string rootSrc, string rootDst, string file)
        {
            string relativePathDest = Path.GetRelativePath(rootSrc, file);
            return Path.Combine(rootDst, relativePathDest);
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
