using Newtonsoft.Json;
using System.Numerics;
using Convert = CrappyPrizm.Tools.Convert;

namespace CrappyPrizm
{
    public class Account
    {
        #region Var
        [JsonProperty("account")]
        public BigInteger Id { get; }

        [JsonProperty("accountRS")]
        public string Address { get; }

        [JsonIgnore]
        public decimal Balance => Coins == -1 ? -1 : Convert.CoinsToAmount(Coins);

        [JsonProperty("balanceNQT")]
        public decimal Coins { get; }

        [JsonProperty("publicKey")]
        public string PublicKey { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public Account( [JsonProperty("account")]BigInteger id,
                        [JsonProperty("accountRS")]string address,
                        [JsonProperty("balanceNQT")]decimal coins,
                        [JsonProperty("publicKey")]string publicKey)
        {
            Id = id;
            Address = address;
            Coins = coins;
            PublicKey = publicKey;
        }

        public Account(BigInteger id, string address, string publicKey) : this(id, address, -1, publicKey) { }
        #endregion

        #region Functions
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Address;
        public override bool Equals(object? obj) => obj is Account account && account.Id == Id;
        #endregion
    }
}
