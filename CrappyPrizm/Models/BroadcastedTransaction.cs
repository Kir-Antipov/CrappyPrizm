using Newtonsoft.Json;
using System.Numerics;

namespace CrappyPrizm.Models
{
    internal class BroadcastedTransaction
    {
        #region Var
        [JsonProperty("transaction")]
        public BigInteger Id { get; }
        
        [JsonProperty("fullHash")]
        public string Hash { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public BroadcastedTransaction([JsonProperty("transaction")]BigInteger id, [JsonProperty("fullHash")]string hash)
        {
            Id = id;
            Hash = hash;
        }
        #endregion

        #region Functions
        public override string ToString() => Hash;
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object? obj) => obj is BroadcastedTransaction transaction && transaction.Id == Id;
        #endregion
    }
}
