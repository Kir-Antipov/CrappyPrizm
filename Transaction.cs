using Newtonsoft.Json;
using System.Numerics;

namespace CrappyPrizm
{
    public class Transaction
    {
        #region Var
        [JsonProperty("transaction")]
        public BigInteger Id { get; }
        
        [JsonProperty("fullHash")]
        public string Hash { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public Transaction([JsonProperty("transaction")]BigInteger id, [JsonProperty("fullHash")]string hash)
        {
            Id = id;
            Hash = hash;
        }
        #endregion

        #region Functions
        public override string ToString() => Hash;
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object? obj) => obj is Transaction transaction && transaction.Id == Id;
        #endregion
    }
}
