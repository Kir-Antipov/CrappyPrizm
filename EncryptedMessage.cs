using System;
using Newtonsoft.Json;
using CrappyPrizm.Crypto;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

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
            byte[] sharedKey = Convert.PrivateAndPublicToSharedKey(Convert.SecretPhraseToPrivateKey(secretPhrase), Convert.HexToBytes(publicKey));
            for (int i = 0; i < Salt.Length; ++i)
                sharedKey[i] ^= Salt[i];
            Sha256Digest sha256 = new Sha256Digest();
            sharedKey = sha256.Digest(sharedKey);

            byte[] decrypted = AesDecrypt(Data, sharedKey);
            if (IsCompressed)
                decrypted = GZip.Decompress(decrypted);

            return new Message(IsText ? Convert.BytesToString(decrypted) : Convert.BytesToHex(decrypted));
        }

        public static byte[] AesDecrypt(byte[] ciphered, byte[] key)
        {
            if (ciphered.Length == 0 || ciphered.Length % 16 != 0)
                throw new ArgumentException(nameof(ciphered));

            byte[] iv = new byte[16];
            Array.Copy(ciphered, iv, iv.Length);

            byte[] data = new byte[ciphered.Length - 16];
            Array.Copy(ciphered, iv.Length, data, 0, data.Length);

            PaddedBufferedBlockCipher aes = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));
            aes.Init(false, new ParametersWithIV(new KeyParameter(key), iv));
            byte[] output = new byte[aes.GetOutputSize(data.Length)];
            int plaintextLength = aes.ProcessBytes(data, 0, data.Length, output, 0);
            plaintextLength += aes.DoFinal(output, plaintextLength);
            byte[] result = new byte[plaintextLength];
            Array.Copy(output, 0, result, 0, result.Length);
            return result;
        }
        #endregion
    }
}
