using System;
using CrappyPrizm.API;
using System.Numerics;
using CrappyPrizm.Models;
using System.Threading.Tasks;
using CrappyPrizm.RequestBuilders;
using System.Security.Cryptography;
using Convert = CrappyPrizm.Tools.Convert;

namespace CrappyPrizm.Endpoints
{
    public class AccountEndpoint : PrizmEndpointBase, IAccountEndpoint
    {
        #region Var
        private const int DefaultSecretPhraseLength = 256;

        private static readonly RNGCryptoServiceProvider RandomNumberGenerator = new RNGCryptoServiceProvider();
        #endregion

        #region Init
        public AccountEndpoint(IApiClient apiClient) : base(apiClient) { }

        public AccountEndpoint(IApiClient apiClient, IRequestBuilder requestBuilder) : base(apiClient, requestBuilder) { }
        #endregion

        #region Functions
        public Account CreateAccount() => CreateAccount(DefaultSecretPhraseLength);

        public Account CreateAccount(int secretLength)
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";

            Span<byte> symbols = stackalloc byte[secretLength];
            RandomNumberGenerator.GetBytes(symbols);

            Span<char> secretPhrase = stackalloc char[secretLength];
            for (int i = 0; i < secretLength; ++i)
                secretPhrase[i] = alphabet[symbols[i] * (alphabet.Length - 1) / byte.MaxValue];

            return CreateAccount(new string(secretPhrase));
        }

        public Account CreateAccount(string secretPhrase) => Account.FromSecretPhrase(secretPhrase);

        public Task<Account> GetAccountAsync(BigInteger accountId) => GetAccountAsync(Convert.AccountIdToAddress(accountId));
        public Task<Account> GetAccountAsync(string address) => SendRequestAsync<Account>("getAccount", ("account", address));
        #endregion
    }
}
