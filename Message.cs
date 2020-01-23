using System;
using CrappyPrizm.Tools;
using CrappyPrizm.Tools.Crypto;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Convert = CrappyPrizm.Tools.Convert;

namespace CrappyPrizm
{
    public class Message
    {
        #region Var
        public string Text { get; }

        private static readonly RNGCryptoServiceProvider RandomNumberGenerator = new RNGCryptoServiceProvider();
        #endregion

        #region Init
        public Message(string text) => Text = text;
        
        public static implicit operator string(Message message) => message.Text;
        public static implicit operator Message(string text) => new Message(text);
        #endregion

        #region Functions
        public EncryptedMessage Encrypt(string publicKey, string secretPhrase)
        {
            byte[] sharedKey = Convert.PrivateAndPublicToSharedKey(Convert.SecretPhraseToPrivateKey(secretPhrase), Convert.HexToBytes(publicKey));
            byte[] compressed = GZip.Compress(Convert.StringToBytes(Text));

            byte[] salt = new byte[32];
            RandomNumberGenerator.GetBytes(salt);

            for (int i = 0; i < sharedKey.Length; ++i)
                sharedKey[i] ^= salt[i];
            Sha256Digest sha256 = new Sha256Digest();
            sharedKey = sha256.Digest(sharedKey);

            return new EncryptedMessage(AesEncrypt(compressed, sharedKey), salt, true, true);
        }

        private static byte[] AesEncrypt(byte[] data, byte[] key)
        {
            byte[] iv = new byte[16];
            RandomNumberGenerator.GetBytes(iv);

            PaddedBufferedBlockCipher aes = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
            aes.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

            byte[] output = new byte[aes.GetOutputSize(data.Length)];
            int ciphertextLength = aes.ProcessBytes(data, 0, data.Length, output, 0);

            ciphertextLength += aes.DoFinal(output, ciphertextLength);
            byte[] result = new byte[iv.Length + ciphertextLength];

            Array.Copy(iv, 0, result, 0, iv.Length);
            Array.Copy(output, 0, result, iv.Length, ciphertextLength);
            return result;
        }

        public override string ToString() => Text;
        public override int GetHashCode() => Text.GetHashCode();
        public override bool Equals(object? obj) => obj is Message message && message.Text == Text;
        #endregion
    }
}
