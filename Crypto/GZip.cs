using System.IO;
using System.IO.Compression;

namespace CrappyPrizm.Crypto
{
    internal static class GZip
    {
        public static byte[] Compress(byte[] input)
        {
            using MemoryStream result = new MemoryStream();
            using Stream compressionStream = new GZipStream(result, CompressionMode.Compress);
            compressionStream.Write(input, 0, input.Length);
            return result.ToArray();
        }
    }
}
