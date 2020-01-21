using Newtonsoft.Json;

namespace CrappyPrizm
{
    public class Account
    {
        #region Var
        [JsonProperty("account")]
        public long Id { get; }

        [JsonProperty("accountRS")]
        public string Address { get; }

        [JsonProperty("balanceNQT")]
        public decimal Balance { get; }

        [JsonProperty("publicKey")]
        public string PublicKey { get; }

        public string? PrivateKey { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public Account( [JsonProperty("account")]long id,
                        [JsonProperty("accountRS")]string address,
                        [JsonProperty("balanceNQT")]decimal balance,
                        [JsonProperty("publicKey")]string publicKey)
        {
            Id = id;
            Address = address;
            Balance = balance;
            PublicKey = publicKey;
            PrivateKey = null;
        }

        public Account(long id, string address, decimal balance, string publicKey, string privateKey)
        {
            Id = id;
            Address = address;
            Balance = balance;
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
