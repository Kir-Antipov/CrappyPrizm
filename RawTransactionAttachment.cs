using Newtonsoft.Json;

namespace CrappyPrizm
{
    internal class RawTransactionAttachment
    {
        #region Var
        [JsonProperty("encryptedMessage")]
        public EncryptedMessage EncryptedMessage { get; }

        [JsonProperty("encryptToSelfMessage")]
        public EncryptedMessage EncryptedToSelfMessage { get; }

        [JsonProperty("recipientPublicKey")]
        public string RecipientPublicKey { get; }

        [JsonProperty("version.EncryptedMessage")]
        public int EncryptedMessageVersion { get; }

        [JsonProperty("version.EncryptToSelfMessage")]
        public int EncryptedToSelfMessageVersion { get; }

        [JsonProperty("version.OrdinaryPayment")]
        public int OrdinaryPayment { get; }

        [JsonProperty("version.EncryptedMessage")]
        public int PublicKeyAnnouncement { get; }

        [JsonProperty("message")]
        public Message? Message { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public RawTransactionAttachment(EncryptedMessage encryptedMessage,
                                        [JsonProperty("encryptToSelfMessage")]EncryptedMessage encryptedToSelfMessage,
                                        string recipientPublicKey,
                                        [JsonProperty("version.EncryptedMessage")]int encryptedMessageVersion,
                                        [JsonProperty("version.EncryptToSelfMessage")]int encryptedToSelfMessageVersion,
                                        [JsonProperty("version.OrdinaryPayment")]int ordinaryPayment,
                                        [JsonProperty("version.PublicKeyAnnouncement")]int publicKeyAnnouncement,
                                        Message? message)
        {
            EncryptedMessage = encryptedMessage;
            EncryptedToSelfMessage = encryptedToSelfMessage;
            RecipientPublicKey = recipientPublicKey;
            EncryptedMessageVersion = encryptedMessageVersion;
            EncryptedToSelfMessageVersion = encryptedToSelfMessageVersion;
            OrdinaryPayment = ordinaryPayment;
            PublicKeyAnnouncement = publicKeyAnnouncement;
            Message = message;
        }
        #endregion
    }
}
