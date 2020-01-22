using System;
using System.Text;
using CrappyPrizm.Crypto;
using System.Security.Cryptography;

namespace CrappyPrizm
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

        // FIX
        public static byte[] StringToBytes(string str)
        {
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; ++i)
                bytes[i] = (byte)str[i];
            return bytes;
        }

        public static byte[] PrivateAndPublicToSharedKey(byte[] privateKey, byte[] publicKey) => Curve25519.Curve(privateKey, publicKey);

        public static byte[] SecretPhraseToPrivateKey(string secretPhrase)
        {
            using SHA256Managed sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(StringToBytes(secretPhrase));
            Curve25519.Clamp(hash);
            return hash;
        }

        public static string SecretPhraseToPrivateKeyHex(string secretPhrase) => BytesToHex(SecretPhraseToPrivateKey(secretPhrase));
        #endregion
    }
}
