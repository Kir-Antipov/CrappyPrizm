using Newtonsoft.Json;
using System.Numerics;

namespace CrappyPrizm
{
    internal class RawTransactionDetails
    {
        #region Var
        [JsonProperty("amountNQT")]
        public decimal Coins { get; }

        [JsonProperty("attachment")]
        public RawTransactionAttachment Attachment { get; }

        [JsonProperty("deadline")]
        public int Deadline { get; }

        [JsonProperty("ecBlockHeight")]
        public int BlockHeight { get; }

        [JsonProperty("ecBlockId")]
        public BigInteger BlockId { get; }

        [JsonProperty("feeNQT")]
        public decimal CoinsFee { get; }

        [JsonProperty("height")]
        public int Height { get; }

        [JsonProperty("recipient")]
        public BigInteger Recipient { get; }

        [JsonProperty("recipientRS")]
        public string RecipientAddress { get; }

        [JsonProperty("sender")]
        public BigInteger Sender { get; }

        [JsonProperty("senderPublicKey")]
        public string SenderPublicKey { get; }

        [JsonProperty("senderRS")]
        public string SenderAddress { get; }

        [JsonProperty("subType")]
        public int SubType { get; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; }

        [JsonProperty("type")]
        public int Type { get; }

        [JsonProperty("version")]
        public int Version { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public RawTransactionDetails([JsonProperty("amountNQT")]decimal coins,
                                     RawTransactionAttachment attachment,
                                     int deadline,
                                     [JsonProperty("ecBlockHeight")]int blockHeight,
                                     [JsonProperty("ecBlockId")]BigInteger blockId,
                                     [JsonProperty("feeNQT")]decimal coinsFee,
                                     int height,
                                     BigInteger recipient,
                                     [JsonProperty("recipientRS")]string recipientAddress,
                                     BigInteger sender,
                                     string senderPublicKey,
                                     [JsonProperty("senderRS")]string senderAddress,
                                     int subType,
                                     long timestamp,
                                     int type,
                                     int version)
        {
            Coins = coins;
            Attachment = attachment;
            Deadline = deadline;
            BlockHeight = blockHeight;
            BlockId = blockId;
            CoinsFee = coinsFee;
            Height = height;
            Recipient = recipient;
            RecipientAddress = recipientAddress;
            Sender = sender;
            SenderPublicKey = senderPublicKey;
            SenderAddress = senderAddress;
            SubType = subType;
            Timestamp = timestamp;
            Type = type;
            Version = version;
        }
        #endregion
    }
}
