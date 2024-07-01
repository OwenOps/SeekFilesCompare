using System.Security.Cryptography;
using System.Text;

namespace SeekFilesCompare.multiThreads
{
    public class HashFiles
    {
        public static async Task<string> HashFile(string filePath)
        {
            try
            {
                using (SHA256 mySha256 = SHA256.Create())
                using (var stream = new BufferedStream(File.OpenRead(filePath), 1_000_000))
                {
                    byte[] hashValue = await mySha256.ComputeHashAsync(stream);
                    return PrintByteArray(hashValue);
                }
            }
            catch (IOException e)
            {
                Console.Error.WriteLine($"I/O exception : {e.Message}");
            }

            return "";
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
