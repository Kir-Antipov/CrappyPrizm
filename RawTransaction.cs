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
    }
}
