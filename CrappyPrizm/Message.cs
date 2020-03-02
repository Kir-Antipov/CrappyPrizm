using CrappyPrizm.JS;
using System.Security.Cryptography;
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
        public static implicit operator Message(string? text) => new Message(text ?? string.Empty);
        #endregion

        #region Functions
        public EncryptedMessage Encrypt(string publicKey, string secretPhrase)
        {
            using Engine engine = EnginePool.Get();
            byte[] sharedKey = Convert.PrivateAndPublicToSharedKey(Convert.SecretPhraseToPrivateKey(secretPhrase), Convert.HexToBytes(publicKey));
            byte[] compressed = engine.Deflate(Convert.StringToBytes(Text));

            byte[] salt = new byte[32];
            RandomNumberGenerator.GetBytes(salt);

            for (int i = 0; i < sharedKey.Length; ++i)
                sharedKey[i] ^= salt[i];

            byte[] iv = new byte[16];
            RandomNumberGenerator.GetBytes(iv);

            return new EncryptedMessage(engine.AesEncrypt(compressed, sharedKey, iv), salt, true, true);
        }

        public override string ToString() => Text;
        public override int GetHashCode() => Text.GetHashCode();
        public override bool Equals(object? obj) => obj is Message message && message.Text == Text;
        #endregion
    }
}
