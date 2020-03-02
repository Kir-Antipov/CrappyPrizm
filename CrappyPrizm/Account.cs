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
        public long Coins { get; }

        [JsonProperty("publicKey")]
        public string PublicKey { get; }

        [JsonProperty("secretPhrase")]
        public string SecretPhrase { get; }
        #endregion

        #region Init
        [JsonConstructor]
        public Account( [JsonProperty("account")]BigInteger id,
                        [JsonProperty("accountRS")]string? address = null,
                        [JsonProperty("balanceNQT")]long coins = -1,
                        string? publicKey = null,
                        string? secretPhrase = null)
        {
            Id = id;
            Address = address ?? string.Empty;
            Coins = coins < 0 ? -1 : coins;
            PublicKey = publicKey ?? string.Empty;
            SecretPhrase = secretPhrase ?? string.Empty;
        }
        #endregion

        #region Functions
        public static bool operator ==(Account a, Account b) => a?.Id == b?.Id; 
        public static bool operator !=(Account a, Account b) => a?.Id != b?.Id; 

        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Address;
        public override bool Equals(object? obj) => obj is Account account && account.Id == Id;
        #endregion
    }
}
