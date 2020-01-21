using Newtonsoft.Json;
using System.Numerics;

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
        public decimal Balance => Convert.CoinsToAmount(Coins);

        [JsonProperty("balanceNQT")]
        public decimal Coins { get; }

        [JsonProperty("publicKey")]
        public string PublicKey { get; }

        public string? PrivateKey { get; }
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
            PrivateKey = null;
        }

        public Account(BigInteger id, string address, decimal coins, string publicKey, string privateKey)
        {
            Id = id;
            Address = address;
            Coins = coins;
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
        #endregion

        #region Functions
        public Account With(string privateKey) => new Account(Id, Address, Balance, PublicKey, privateKey);

        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Address;
        public override bool Equals(object? obj) => obj is Account account && account.Id == Id;
        #endregion
    }
}
