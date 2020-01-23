using Newtonsoft.Json;
using System.Collections.Generic;

namespace CrappyPrizm
{
    internal class RawTransactionsContainer
    {
        #region Var
        public IEnumerable<RawTransactionDetails> Transactions { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public RawTransactionsContainer(IEnumerable<RawTransactionDetails> transactions) => Transactions = transactions;
        #endregion
    }
}
