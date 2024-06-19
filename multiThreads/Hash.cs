using System.Security.Cryptography;
using System.Text;

namespace SeekFilesCompare.multiThreads
{
    public class HashFiles
    {
        public static string HashFile(string filePath)
        {
            try
            {
                using (SHA256 mySha256 = SHA256.Create())
                using (var stream = new BufferedStream(File.OpenRead(filePath), 1_000_000))
                {
                    byte[] hashValue = mySha256.ComputeHash(stream);
                    return PrintByteArray(hashValue);
                }
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
    }
}
