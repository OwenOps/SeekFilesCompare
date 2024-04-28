using System.Security.Cryptography;

namespace testRecursive;

public class HashFiles
{
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
        }
        else
        {
            Console.WriteLine("The directory specified could not be found.");
        }
    }

    private static void ProcessDirectory(string directory)
    {
        try
        {
            foreach (string filePath in Directory.GetFiles(directory))
            {
                HashFile(filePath);
            }

            //Recursive
            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                ProcessDirectory(subDirectory);
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Access denied for the file : {directory}");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"File not found : {directory}");
        }
    }

    private static void HashFile(string filePath)
    {
        try
        {
            using (SHA256 mySHA256 = SHA256.Create())
            using (FileStream fileStream = File.Open(filePath, FileMode.Open))
            {
                byte[] hashValue = mySHA256.ComputeHash(fileStream);
                Console.Write($"{Path.GetFileName(filePath)}: ");
                PrintByteArray(hashValue);
            }
        }
        catch (IOException e)
        {
            Console.WriteLine($"I/O exception : {e.Message}");
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine($"Access denied : {e.Message}");
        }
    }

    public static void PrintByteArray(byte[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Console.Write($"{array[i]:X2}");
            if ((i % 4) == 3) Console.Write(" ");
        }
        Console.WriteLine();
    }
}