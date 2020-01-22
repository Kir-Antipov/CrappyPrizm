using System;
using System.Text;

namespace CrappyPrizm
{
    public static class Convert
    {
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
    }
}
