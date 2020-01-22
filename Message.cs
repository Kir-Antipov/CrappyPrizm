using System;
using CrappyPrizm.Crypto;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;

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

            Sha256Digest sha256 = new Sha256Digest();
            for (int i = 0; i < sharedKey.Length; ++i)
                sharedKey[i] ^= salt[i];
            sharedKey = sha256.Digest(sharedKey);

            byte[] iv = new byte[16];
            RandomNumberGenerator.GetBytes(iv);

            byte[] encrypted = new AesManaged().CreateEncryptor(sharedKey, iv).TransformFinalBlock(compressed, 0, compressed.Length);

            byte[] data = new byte[iv.Length + encrypted.Length];
            Array.Copy(iv, data, iv.Length);
            Array.Copy(encrypted, 0, data, iv.Length, encrypted.Length);

            return new EncryptedMessage(data, salt, true, true);
        }

        public override string ToString() => Text;
        public override int GetHashCode() => Text.GetHashCode();
        public override bool Equals(object? obj) => obj is Message message && message.Text == Text;
        #endregion
    }
}
