using CrappyPrizm.JS;
using Newtonsoft.Json;
using Convert = CrappyPrizm.Tools.Convert;

namespace CrappyPrizm
{
    public class EncryptedMessage
    {
        #region Var
        [JsonIgnore]
        public byte[] Data { get; }

        [JsonIgnore]
        public byte[] Salt { get; }

        [JsonProperty("data")]
        public string HexData => Convert.BytesToHex(Data);

        [JsonProperty("nonce")]
        public string HexSalt => Convert.BytesToHex(Salt);

        [JsonProperty("isCompressed")]
        public bool IsCompressed { get; }

        [JsonProperty("isText")]
        public bool IsText { get; }
        #endregion

        #region Init
        public EncryptedMessage(byte[] data, byte[] salt, bool isCompressed, bool isText)
        {
            Data = data;
            Salt = salt;
            IsCompressed = isCompressed;
            IsText = isText;
        }

        [JsonConstructor]
        public EncryptedMessage(string data, [JsonProperty("nonce")]string salt, bool isCompressed, bool isText)
        {
            Data = Convert.HexToBytes(data);
            Salt = Convert.HexToBytes(salt);
            IsCompressed = isCompressed;
            IsText = isText;
        }
        #endregion

        #region Functions
        public Message Decrypt(string publicKey, string secretPhrase)
        {
            using Engine engine = EnginePool.Get();
            byte[] sharedKey = Convert.PrivateAndPublicToSharedKey(Convert.SecretPhraseToPrivateKey(secretPhrase), Convert.HexToBytes(publicKey));
            for (int i = 0; i < Salt.Length; ++i)
                sharedKey[i] ^= Salt[i];

            byte[] decrypted = engine.AesDecrypt(Data, sharedKey);
            if (IsCompressed)
                decrypted = engine.Inflate(decrypted);

            return new Message(IsText ? Convert.BytesToString(decrypted) : Convert.BytesToHex(decrypted));
        }
        #endregion
    }
}
