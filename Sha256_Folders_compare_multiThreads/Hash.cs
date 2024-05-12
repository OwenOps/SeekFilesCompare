using Sha256.oneThread;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Sha256.Sha256_Folders_Compare_multiThreads;

public class HashFiles
{
    static WriteFile.ErrorFileWriter errorLine = new();
    static ExcelHelper.ExcelCreator excelHelper = new();
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
            errorLine.AddContent($"I/O exception : {e.Message}");
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
        string[] Files = System.IO.Directory.GetFiles(rootSrc);
        string[] Folders = System.IO.Directory.GetDirectories(rootSrc);
        var currentFolder = "";
        var currentFile = "";

        try
        {
            foreach (var file in Files)
            {
                currentFile = file;
                var destinationFilePath = getDestinationFilePath(_rootFileSrc, rootDst, file);
                FileCompare(destinationFilePath, file);
            }

            foreach (var folder in Folders)
            {
                currentFolder = folder;
                SeekFiles(folder, rootDst);
            }

            return true;
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine($"Access to file {currentFile} was denied: {e.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing file {currentFile}: {ex.Message}");
            return false;
        }
    }

    private static string getDestinationFilePath(string rootSrc, string rootDst, string file)
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
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing file {file}: {ex.Message}");
        }
    }

    /*public static void FileCompare(string srcFile, string dstFile)
    {
        ExcelHelper.ExcelCreator excelHelper = new();
        try
        {
            var dir1 = Directory.EnumerateFiles(srcFile, "*", SearchOption.AllDirectories);

            Parallel.ForEach(dir1, (file) =>
            {
                try
                {
                    string relativePathDest = Path.GetRelativePath(srcFile, file);
                    string destinationFilePath = Path.Combine(@"\\?\", dstFile, relativePathDest);
                    var tmp = file;
                    var tmp2 = dir1;

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
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while processing file {file}: {ex.Message}");
                }
            });
        }
        catch (Exception e)
        {
            var errorMessage = e.InnerException.Message;
            Console.Error.WriteLine(errorMessage);
            errorLine.AddContent(errorMessage);
        }
        finally
        {
            excelHelper.SaveExcel();
        }
    }*/

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