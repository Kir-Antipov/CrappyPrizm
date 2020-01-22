using System;
using System.IO;
using System.IO.Compression;

namespace CrappyPrizm.Crypto
{
    internal static class GZip
    {
        public static byte[] Compress(byte[] input)
        {
            using MemoryStream result = new MemoryStream();
            byte[] lengthBytes = BitConverter.GetBytes(input.Length);
            result.Write(lengthBytes, 0, 4);

            using Stream compressionStream = new GZipStream(result, CompressionMode.Compress);
            compressionStream.Write(input, 0, input.Length);
            compressionStream.Flush();

            return result.ToArray();
        }

        public static byte[] Decompress(byte[] input)
        {
            using Stream source = new MemoryStream(input);
            byte[] lengthBytes = new byte[4];
            source.Read(lengthBytes, 0, 4);
            int length = BitConverter.ToInt32(lengthBytes, 0);
            using Stream decompressionStream = new GZipStream(source, CompressionMode.Decompress);
            
            byte[] result = new byte[length];
            decompressionStream.Read(result, 0, length);
            return result;
        }
    }
}
