using Newtonsoft.Json;

namespace CrappyPrizm
{
    internal class RawTransactionsContainer
    {
        #region Var
        public RawTransactionDetails[] Transactions { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public RawTransactionsContainer(RawTransactionDetails[] transactions) => Transactions = transactions;
        #endregion
    }
}
