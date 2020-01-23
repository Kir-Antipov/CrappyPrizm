using System;
using System.Text;
using CrappyPrizm.Tools.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace CrappyPrizm.Tools
{
    public static class Convert
    {
        #region Functions
        public static decimal CoinsToAmount(decimal coins) => coins / 100;
        public static decimal AmountToCoins(decimal amount) => amount * 100;

        public static string BytesToHex(byte[] bytes)
        {
            const string alphabet = "0123456789abcdef";

            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte b = bytes[i];
                hex.Append(alphabet[b >> 4]);
                hex.Append(alphabet[b & 0xF]);
            }
            return hex.ToString();
        }

        public static byte[] HexToBytes(string hex)
        {
            static int GetHexVal(char hex)
            {
                int val = hex;
                // 58 == '9' + 1
                // 97 == 'a'
                // 87 == 'a' - 10
                // 55 == 'A' - 10
                return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
            }

            if (hex.Length % 2 == 1)
                throw new ArgumentException(nameof(hex), "Invalid length");

            byte[] bytes = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; ++i)
                bytes[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            return bytes;
        }

        public static byte[] StringToBytes(string str) => Encoding.UTF8.GetBytes(str);
        public static string BytesToString(byte[] bytes) => Encoding.UTF8.GetString(bytes);

        public static byte[] PrivateAndPublicToSharedKey(byte[] privateKey, byte[] publicKey) => Curve25519.Curve(privateKey, publicKey);

        public static byte[] SecretPhraseToPrivateKey(string secretPhrase)
        {
            Sha256Digest sha = new Sha256Digest();
            byte[] hash = sha.Digest(StringToBytes(secretPhrase));
            Curve25519.Clamp(hash);
            return hash;
        }

        public static string SecretPhraseToPrivateKeyHex(string secretPhrase) => BytesToHex(SecretPhraseToPrivateKey(secretPhrase));

        public static byte[] UnsignedBytesToSigned(byte[] unsignedBytes, string secretPhrase)
        {
            byte[] P = new byte[32];
            byte[] s = new byte[32];

            Sha256Digest digest = new Sha256Digest();
            Curve25519.Keygen(P, s, digest.Digest(StringToBytes(secretPhrase)));

            byte[] m = digest.Digest(unsignedBytes);

            digest.Update(m);
            byte[] x = digest.Digest(s);
            byte[] Y = new byte[32];
            Curve25519.Keygen(Y, null, x);

            digest.Update(m);
            byte[] h = digest.Digest(Y);

            byte[] v = new byte[32];
            Curve25519.Sign(v, h, x, s);

            byte[] signature = new byte[64];
            Array.Copy(v, 0, signature, 0, 32);
            Array.Copy(h, 0, signature, 32, 32);

            byte[] signedBytes = new byte[unsignedBytes.Length];
            Array.Copy(unsignedBytes, signedBytes, unsignedBytes.Length);
            Array.Copy(signature, 0, signedBytes, 96, signature.Length);

            return signedBytes;
        }
        #endregion
    }
}
