using System;
using System.IO;
using System.IO.Compression;
using System.Text;


namespace GZipCompress
{
    public class GZipCompressor
    {
        public static byte[] Compress(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, System.IO.Compression.CompressionLevel.Optimal))
                {
                    gzipStream.Write(bytes, 0, bytes.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        decompressStream.CopyTo(outputStream);
                    }
                    return outputStream.ToArray();
                }
            }
        }

        public static string DecompressString(string compressedText)
        {

            byte[] zipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(zipBuffer, 0);
                memoryStream.Write(zipBuffer, 4, zipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var zipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    zipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static string CompressString(string text)
        {

            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                zipStream.Write(buffer, 0, buffer.Length);
            }
            memoryStream.Position = 0;
            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var zipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, zipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, zipBuffer, 0, 4);
            return Convert.ToBase64String(zipBuffer);
        }
    }
}

