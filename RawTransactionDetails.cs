using System;
using Newtonsoft.Json;
using System.Numerics;
using Convert = CrappyPrizm.Tools.Convert;

namespace CrappyPrizm
{
    internal class RawTransactionDetails
    {
        #region Var
        [JsonProperty("transaction")]
        public BigInteger Id { get; }

        [JsonProperty("fullHash")]
        public string? Hash { get; }

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

        [JsonProperty("confirmations")]
        public int Confirmations { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public RawTransactionDetails([JsonProperty("transaction")]BigInteger id,
                                     [JsonProperty("fullHash")]string? hash,
                                     [JsonProperty("amountNQT")]decimal coins,
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
                                     int version,
                                     int confirmations)
        {
            Id = id;
            Hash = hash;
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
            Confirmations = confirmations;
        }
        #endregion

        #region Functions
        public Transaction Prepare() => new Transaction
        (
            Id,
            Hash ?? string.Empty,
            new Account(Sender, SenderAddress, publicKey: SenderPublicKey),
            new Account(Recipient, RecipientAddress),
            Coins,
            CoinsFee,
            new Block(BlockId, BlockHeight, Height),
            Convert.TheirCrappyTimestampToDateTime(Timestamp),
            DateTime.UtcNow.AddMinutes(Deadline),
            Confirmations,
            Attachment.EncryptedMessage,
            Attachment.EncryptedToSelfMessage
        );
        #endregion
    }
}
