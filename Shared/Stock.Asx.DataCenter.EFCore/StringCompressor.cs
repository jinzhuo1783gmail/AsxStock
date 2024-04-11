using System;
using System.IO;
using System.IO.Compression;
using System.Text;

public class StringCompressor
{
    public static byte[] CompressString(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        byte[] inputData = Encoding.UTF8.GetBytes(input);
        using (MemoryStream output = new MemoryStream())
        {
            using (GZipStream gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(inputData, 0, inputData.Length);
            }
            return output.ToArray();
        }
    }

    public static string DecompressString(byte[] compressedData)
    {
        if (compressedData == null || compressedData.Length == 0)
        {
            return null;
        }

        using (MemoryStream input = new MemoryStream(compressedData))
        {
            using (GZipStream gzip = new GZipStream(input, CompressionMode.Decompress))
            {
                using (StreamReader reader = new StreamReader(gzip, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}