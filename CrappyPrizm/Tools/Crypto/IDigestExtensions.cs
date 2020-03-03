using Org.BouncyCastle.Crypto;

namespace CrappyPrizm.Tools.Crypto
{
    public static class IDigestExtensions
    {
        public static void Update(this IDigest digest, byte[] input) => digest.BlockUpdate(input, 0, input.Length);

        public static byte[] Digest(this IDigest digest)
        {
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
        }
        public static byte[] Digest(this IDigest digest, byte[] input)
        {
            digest.Update(input);
            return digest.Digest();
        }
    }
}
