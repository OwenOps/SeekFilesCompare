using SeekFilesCompare.mail;
using SeekFilesCompare.timer;

namespace SeekFilesCompare.multiThreads
{
    public class SeekFiles
    {
        private static ExcelHelper.ExcelCreator _excelHelper = new();
        private static readonly int MaxThreads = 15;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(MaxThreads, MaxThreads);

        public static string RootFileSrc = "";
        private static int _numberOfFiles = 0;
        private static int _numberOfFilesProcessed = 0;

        public static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("You have to specified a source and a destination folder to compare them.");
                return;
            }

            RootFileSrc = args[0];
            string dstDirectory = args[1];
            _numberOfFiles = GetFileCount(RootFileSrc);

            ExcelHelper.SetNameDirectory(RootFileSrc);
            TimerSeek.Start();

            bool goodEnding;
            if (Directory.Exists(RootFileSrc) && Directory.Exists(dstDirectory))
            {
                Console.WriteLine($"Source : {RootFileSrc}");
                Console.WriteLine($"Destination : {dstDirectory}");

                goodEnding = await Seek(RootFileSrc, dstDirectory);
                _excelHelper.SaveExcel();
            }
            else
            {
                Console.Error.WriteLine("The directory specified could not be found.");
                return;
            }

            TimerSeek.Stop();

            Console.WriteLine(goodEnding
                ? $"\n---------- The program ended well {GetNumberFilesProcessedString()} ----------"
                : $"\n---------- The program did not end well {GetNumberFilesProcessedString()} ----------");

            SendMail.SendAMail();
        }

        private static async Task<bool> Seek(string rootSrc, string rootDst)
        {
            try
            {
                var files = Directory.GetFiles(rootSrc);
                var tasks = new List<Task>();

                foreach (var file in files)
                {
                    await _semaphore.WaitAsync();
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            var destinationFilePath = GetDestinationFilePath(RootFileSrc, rootDst, file);
                            await FileCompare(destinationFilePath, file);
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    });
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);

                var folders = Directory.GetDirectories(rootSrc);
                var folderTasks = folders.Select(folder => Seek(folder, rootDst)).ToList();

                await Task.WhenAll(folderTasks);

                return true;
            }
            catch (UnauthorizedAccessException e)
            {
                LogFileError(rootSrc, e.Message, _excelHelper);
                return false;
            }
            catch (Exception e)
            {
                LogFileError(rootSrc, e.Message, _excelHelper);
                return false;
            }
        }

        public static async Task FileCompare(string destinationFilePath, string file)
        {
            try
            {
                if (!File.Exists(destinationFilePath))
                {
                    LogFileError(destinationFilePath, "File not found", excelHelper: _excelHelper);
                    return;
                }

                string fileSizeSrc = new FileInfo(file).Length.ToString();
                string fileSizeDst = new FileInfo(destinationFilePath).Length.ToString();
                string formatByte = $"{fileSizeDst} bytes";

                _numberOfFilesProcessed = Interlocked.Increment(ref _numberOfFilesProcessed);
                Console.WriteLine($"{file} : ({GetNumberFilesProcessedString()} | size : {formatByte})");

                string hashSrc = await HashFiles.HashFile(file);
                string hashDest = await HashFiles.HashFile(destinationFilePath);

                if (hashSrc != hashDest)
                {
                    LogFileError(destinationFilePath, "Not the same Hash", _excelHelper, hashDest, formatByte);
                    return;
                }

                if (fileSizeSrc.Length != fileSizeDst.Length)
                {
                    LogFileError(destinationFilePath, "Not the same content", _excelHelper, hashDest, formatByte);
                    return;
                }

                if (new FileInfo(file).LastWriteTimeUtc != new FileInfo(destinationFilePath).LastWriteTimeUtc)
                {
                    LogFileError(destinationFilePath, "Not the same write time", _excelHelper, hashDest, formatByte);
                    return;
                }

                if (new FileInfo(file).CreationTimeUtc != new FileInfo(destinationFilePath).CreationTimeUtc)
                {
                    LogFileError(destinationFilePath, "Not the same creation time", _excelHelper, hashDest, formatByte);
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
            return Path.Combine($@"\\{{Environment.MachineName}}", rootDst, relativePathDest);
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

        private static String GetNumberFilesProcessedString()
        {
            return $"{_numberOfFilesProcessed}/{_numberOfFiles}";
        }

        private static int GetFileCount(string directory)
        {
            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Length;
        }
    }
}