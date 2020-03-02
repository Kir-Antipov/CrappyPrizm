using System;
using System.Linq;
using System.Text;
using System.Numerics;
using CrappyPrizm.Tools.Crypto;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Digests;

namespace CrappyPrizm.Tools
{
    public static class Convert
    {
        #region Var
        private const long F_ckinglyIrrationalEpochConstant = 1532715479500L;
        private const string AddressAlphabet = "PRZM23456789ABCDEFGHJKLNQSTUVWXY";

        private static readonly int[] GExp = new[] { 1, 2, 4, 8, 16, 5, 10, 20, 13, 26, 17, 7, 14, 28, 29, 31, 27, 19, 3, 6, 12, 24, 21, 15, 30, 25, 23, 11, 22, 9, 18, 1 };
        private static readonly int[] GLog = new[] { 0, 0, 1, 18, 2, 5, 19, 11, 3, 29, 6, 27, 20, 8, 12, 23, 4, 10, 30, 17, 7, 22, 28, 26, 21, 25, 9, 16, 13, 14, 24, 15 };
        private static readonly int[] CWMap = new[] { 3, 2, 1, 0, 7, 6, 5, 4, 13, 14, 15, 16, 12, 8, 9, 10, 11 };
        #endregion

        #region Functions
        public static decimal CoinsToAmount(long coins) => coins / 100m;
        public static long AmountToCoins(decimal amount) => (long)Math.Round(amount * 100, MidpointRounding.ToZero);

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
                throw new ArgumentException("Invalid length", nameof(hex));

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
            byte[] hash = new Sha256Digest().Digest(StringToBytes(secretPhrase));
            Curve25519.Clamp(hash);
            return hash;
        }

        public static byte[] SecretPhraseToPublicKey(string secretPhrase)
        {
            byte[] bytes = StringToBytes(secretPhrase);
            bytes = new Sha256Digest().Digest(bytes);

            byte[] publicKey = new byte[32];
            Curve25519.Keygen(publicKey, null, bytes);

            return publicKey;
        }

        public static BigInteger PublicKeyToAccountId(byte[] publicKey)
        {
            byte[] hash = new Sha256Digest().Digest(publicKey);
            BigInteger accountId = 0;
            for (int i = 7; i >= 0; --i)
                accountId = accountId * 256 + hash[i];
            return accountId;
        }

        public static string AccountIdToAddress(BigInteger accountId)
        {
            static int GMul(int a, int b)
            {
                if (a == 0 || b == 0) 
                    return 0;

                return GExp[(GLog[a] + GLog[b]) % 31];
            }

            if (accountId < 0)
                throw new ArgumentOutOfRangeException(nameof(accountId));

            string acc = accountId.ToString();
            int[] input = new int[acc.Length];
            int[] codeword = new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] output = new int[codeword.Length];
            int pos = 0;
            int length = acc.Length;

            for (int i = 0; i < length; ++i)
                input[i] = acc[i] - '0';

            int newLength;
            do
            {
                int divide = 0;
                newLength = 0;

                for (int i = 0; i < length; ++i)
                {
                    divide = divide * 10 + input[i];
                    if (divide >= 32)
                    {
                        input[newLength++] = divide >> 5;
                        divide &= 31;
                    }
                    else if (newLength > 0)
                    {
                        input[newLength++] = 0;
                    }
                }
                length = newLength;
                output[pos++] = divide;
            }
            while (newLength != 0);

            for (int i = 0; i < 13; ++i)
                codeword[i] = --pos >= 0 ? output[i] : 0;

            var p = new int[] { 0, 0, 0, 0 };

            for (int i = 12; i >= 0; --i)
            {
                int fb = codeword[i] ^ p[3];
                p[3] = p[2] ^ GMul(30, fb);
                p[2] = p[1] ^ GMul(6, fb);
                p[1] = p[0] ^ GMul(9, fb);
                p[0] = GMul(17, fb);
            }

            codeword[13] = p[0];
            codeword[14] = p[1];
            codeword[15] = p[2];
            codeword[16] = p[3];

            StringBuilder address = new StringBuilder(26).Append("PRIZM-");
            for (int i = 0; i < 17; i++)
            {
                address.Append(AddressAlphabet[codeword[CWMap[i]]]);
                if ((i & 3) == 3 && i < 13)
                    address.Append('-');
            }
            return address.ToString();
        }

        public static BigInteger AddressToAccountId(string address)
        {
            char[] chars = address.Skip(6).Where(x => x != '-').ToArray();

            if (chars.Length < 17)
                throw new FormatException("Unsupported format", new ArgumentException(nameof(address)));

            int[] codeword = new int[chars.Length];
            for (int i = 0; i < chars.Length; ++i)
                codeword[CWMap[i]] = AddressAlphabet.IndexOf(chars[i]);

			int length = 13;
            IEnumerable<char> stringId = Enumerable.Empty<char>();
            int[] input = new int[length];

            for (int i = 0; i < 13; i++)
                input[i] = codeword[12 - i];

            int newlen;
            do
            {
                int divide = 0;
                newlen = 0;

                for (int i = 0; i < length; i++)
                {
                    divide = divide * 32 + input[i];

                    if (divide >= 10)
                    {
                        input[newlen++] = divide / 10;
                        divide %= 10;
                    }
                    else if (newlen > 0)
                        input[newlen++] = 0;
                }

                length = newlen;
                stringId = stringId.Prepend((char)('0' + divide));
            }
            while (newlen != 0);

            return BigInteger.Parse(stringId.ToArray());
        }

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

        internal static DateTime TheirCrappyTimestampToDateTime(long timestamp) => DateTime.UnixEpoch.AddMilliseconds(timestamp * 1000L + F_ckinglyIrrationalEpochConstant);
        internal static long DateTimeToTheirCrappyTimestamp(DateTime date) => ((long)(date - DateTime.UnixEpoch).TotalMilliseconds - F_ckinglyIrrationalEpochConstant) / 1000L;
        #endregion
    }
}
