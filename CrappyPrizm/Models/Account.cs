using Newtonsoft.Json;
using System.Numerics;
using Convert = CrappyPrizm.Tools.Convert;

namespace CrappyPrizm.Models
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

        [JsonIgnore]
        public bool IsGenesis => Id == Genesis.Id;

        public static readonly Account Genesis = new Account(8562459348922351959, "PRIZM-TE8N-B3VM-JJQH-5NYJB", publicKey: "491a0d3714a326a3acb3ffe90a61220a17d0b92c747b8036e2e115eeff21dd72");
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

        public static Account FromSecretPhrase(string secretPhrase)
        {
            byte[] publicKey = Convert.SecretPhraseToPublicKey(secretPhrase);
            BigInteger accountId = Convert.PublicKeyToAccountId(publicKey);
            string address = Convert.AccountIdToAddress(accountId);
            return new Account(accountId, address, publicKey: Convert.BytesToHex(publicKey), secretPhrase: secretPhrase);
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
