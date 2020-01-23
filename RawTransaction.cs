using System;
using Newtonsoft.Json;

namespace CrappyPrizm
{
    internal class RawTransaction
    {
        #region Var
        [JsonProperty("broadcasted")]
        public bool Broadcasted { get; }

        [JsonProperty("transactionJSON")]
        public RawTransactionDetails Details { get; }

        [JsonProperty("unsignedTransactionBytes")]
        public string Bytes { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public RawTransaction(bool broadcasted,
                              [JsonProperty("transactionJSON")]RawTransactionDetails details,
                              [JsonProperty("unsignedTransactionBytes")]string bytes)
        {
            Broadcasted = broadcasted;
            Details = details;
            Bytes = bytes;
        }
        #endregion

        #region Functions
        public Transaction Prepare(BroadcastedTransaction broadcasted, string recipientPublicKey) => new Transaction
        (
            broadcasted.Id,
            broadcasted.Hash,
            new Account(Details.Sender, Details.SenderAddress, Details.SenderPublicKey),
            new Account(Details.Recipient, Details.RecipientAddress, recipientPublicKey),
            Details.Coins,
            Details.CoinsFee,
            new Block(Details.BlockId, Details.BlockHeight, Details.Height),
            DateTime.UnixEpoch.AddMilliseconds(Details.Timestamp * 1000L + 1532715479500L), // I don't know what the f*ck is it!!!
            DateTime.UtcNow.AddMinutes(Details.Deadline),
            Details.Confirmations,
            Details.Attachment.EncryptedMessage,
            Details.Attachment.EncryptedToSelfMessage
        );
        #endregion
    }
}
