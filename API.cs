using System;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Numerics;
using CrappyPrizm.Exceptions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using Convert = CrappyPrizm.Tools.Convert;

namespace CrappyPrizm
{
    public static class API
    {
        #region Var
        private const string Url = "https://wallet.prizm.space/";
        private static readonly HttpClient Client = new HttpClient { BaseAddress = new Uri(Url) };

        private static readonly RNGCryptoServiceProvider RandomNumberGenerator = new RNGCryptoServiceProvider();
        #endregion

        #region Methods
        public static Account CreateAccount() => CreateAccount(256);

        public static Account CreateAccount(int secretLength)
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";

            byte[] symbols = new byte[secretLength];
            RandomNumberGenerator.GetBytes(symbols);

            string secretPhrase = new string(Array.ConvertAll(symbols, x => alphabet[(int)(x / 255f * (alphabet.Length - 1))]));
            return CreateAccount(secretPhrase);
        }

        public static Account CreateAccount(string secretPhrase)
        {
            byte[] publicKey = Convert.SecretPhraseToPublicKey(secretPhrase);
            BigInteger accountId = Convert.PublicKeyToAccountId(publicKey);
            string address = Convert.AccountIdToAddress(accountId);
            return new Account(accountId, address, publicKey: Convert.BytesToHex(publicKey), secretPhrase: secretPhrase);
        }

        public static Task<Account> GetAccountAsync(BigInteger accountId) => GetAccountAsync(Convert.AccountIdToAddress(accountId));
        public static Task<Account> GetAccountAsync(string address) => MakeRequestAsync<Account>("getAccount", ("account", address));

        public static IAsyncEnumerable<Transaction> GetTransactionsAsync(string address, int? minNumberOfConfirmations = null, DateTime? minDateUtc = null, TransactionType? type = null) => GetTransactionsAsync(Convert.AddressToAccountId(address), minNumberOfConfirmations, minDateUtc, type);
        public static IAsyncEnumerable<Transaction> GetTransactionsAsync(BigInteger accountId, int? minNumberOfConfirmations = null, DateTime? minDateUtc = null, TransactionType? type = null) => GetBlockchainTransactionsAsync(accountId, minNumberOfConfirmations ?? 0, Convert.DateTimeToTheirCrappyTimestamp(minDateUtc ?? DateTime.UnixEpoch), type ?? TransactionType.All).Select(x => x.Prepare());

        private static async IAsyncEnumerable<RawTransactionDetails> GetBlockchainTransactionsAsync(BigInteger accountId, int numberOfConfirmations, long timestamp, TransactionType type)
        {
            const int bucketSize = 20;
            int firstIndex = -bucketSize;
            int lastIndex = -1;
            numberOfConfirmations = Math.Max(numberOfConfirmations, 0);
            timestamp = Math.Max(timestamp, 0);

            do
            {
                firstIndex += bucketSize;
                lastIndex += bucketSize;
                RawTransactionsContainer container = await MakeRequestAsync<RawTransactionsContainer>("getBlockchainTransactions",      ("account", accountId.ToString()),
                                                                                                                                        ("numberOfConfirmations", numberOfConfirmations.ToString()),
                                                                                                                                        ("timestamp", timestamp.ToString()),
                                                                                                                                        ("firstIndex", firstIndex.ToString()),
                                                                                                                                        ("lastIndex", lastIndex.ToString()));
                bool iterated = false;
                foreach (RawTransactionDetails details in container.Transactions)
                {
                    iterated = true;
                    switch (type)
                    {
                        case TransactionType.Incoming:
                            if (details.Recipient == accountId)
                                yield return details;
                            break;
                        case TransactionType.Outgoing:
                            if (details.Sender == accountId)
                                yield return details;
                            break;
                        default:
                            yield return details;
                            break;
                    }
                }
                if (!iterated)
                    yield break;
            }
            while (true);
        }

        public static decimal ComputeComission(decimal amount)
        {
            if (amount < 12)
                return 0.05m;

            return Math.Min(Math.Round((6 + (amount - 12) / 2) / 100, 2, MidpointRounding.ToZero), 10);
        }

        public static Task<Transaction> SendAsync(string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message? comment = null) => SendAsync(Convert.BytesToHex(Convert.SecretPhraseToPublicKey(secretPhrase)), secretPhrase, recipient, recipientPublicKey, amount, comment);
        public static Task<Transaction> SendAsync(string secretPhrase, string recipientPublicKey, decimal amount, Message? comment = null) => SendAsync(Convert.BytesToHex(Convert.SecretPhraseToPublicKey(secretPhrase)), secretPhrase, Convert.AccountIdToAddress(Convert.PublicKeyToAccountId(Convert.HexToBytes(recipientPublicKey))), recipientPublicKey, amount, comment);
        public static async Task<Transaction> SendAsync(string publicKey, string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message? comment = null)
        {
            RawTransaction raw = await SendMoneyAsync(publicKey, secretPhrase, recipient, recipientPublicKey, amount, comment ?? "");
            BroadcastedTransaction broadcasted = await BroadcastTransactionAsync(JsonConvert.SerializeObject(raw.Details.Attachment), Convert.BytesToHex(Convert.UnsignedBytesToSigned(Convert.HexToBytes(raw.Bytes), secretPhrase)));

            return raw.Prepare(broadcasted, recipientPublicKey);
        }

        public static Task<Transaction> SendAllAsync(string secretPhrase, string recipient, string recipientPublicKey, Message? comment = null) => SendAllAsync(Convert.BytesToHex(Convert.SecretPhraseToPublicKey(secretPhrase)), secretPhrase, recipient, recipientPublicKey, comment);
        public static Task<Transaction> SendAllAsync(string secretPhrase, string recipientPublicKey, Message? comment = null) => SendAllAsync(Convert.BytesToHex(Convert.SecretPhraseToPublicKey(secretPhrase)), secretPhrase, Convert.AccountIdToAddress(Convert.PublicKeyToAccountId(Convert.HexToBytes(recipientPublicKey))), recipientPublicKey, comment);
        public static async Task<Transaction> SendAllAsync(string publicKey, string secretPhrase, string recipient, string recipientPublicKey, Message? comment = null)
        {
            Account account = await GetAccountAsync(Convert.PublicKeyToAccountId(Convert.HexToBytes(publicKey)));
            decimal amount = account.Balance - ComputeComission(account.Balance);
            if (amount <= 0)
                throw new NotEnoughFundsException("Wallet balance less than or equal to transfer fee", ErrorCode.NotEnoughFunds);

            amount += 0.01m;
            if (amount + ComputeComission(amount) > account.Balance)
                amount -= 0.01m;

            return await SendAsync(publicKey, secretPhrase, recipient, recipientPublicKey, amount, comment);
        }

        private static Task<RawTransaction> SendMoneyAsync(string publicKey, string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message comment)
        {
            EncryptedMessage encryptedMessage = comment.Encrypt(recipientPublicKey, secretPhrase);
            EncryptedMessage encryptedToSelfMessage = comment.Encrypt(publicKey, secretPhrase);


            return MakeRequestAsync<RawTransaction>("sendMoney",    ("deadline", "1440"),
                                                                    ("amountNQT", Convert.AmountToCoins(amount).ToString()),
                                                                    ("feeNQT", "5"),
                                                                    ("messageToEncryptIsText", "true"),
                                                                    ("messageToEncryptToSelfIsText", "true"),
                                                                    ("permanent_message", "1"),
                                                                    ("phased", "2"),
                                                                    ("phasingHashedSecret", ""),
                                                                    ("phasingHashedSecretAlgorithm", "2"),
                                                                    ("phasingLinkedFullHash", ""),
                                                                    ("publicKey", publicKey),
                                                                    ("recipient", recipient),
                                                                    ("recipientPublicKey", recipientPublicKey),
                                                                    ("encryptToSelfMessageData", encryptedToSelfMessage.HexData),
                                                                    ("encryptToSelfMessageNonce", encryptedToSelfMessage.HexSalt),
                                                                    ("encryptedMessageData", encryptedMessage.HexData),
                                                                    ("encryptedMessageNonce", encryptedMessage.HexSalt));
        }

        private static Task<BroadcastedTransaction> BroadcastTransactionAsync(string attachment, string bytes) => MakeRequestAsync<BroadcastedTransaction>("broadcastTransaction", ("prunableAttachmentJSON", attachment), ("transactionBytes", bytes));
        #endregion

        #region Helpers
        private static async Task<T> MakeRequestAsync<T>(string type, params (string key, string value)[] parameters)
        {
            HttpResponseMessage response = await MakeRequestAsync(type, parameters);
            string json = await response.Content.ReadAsStringAsync();
            return json.Contains("errorCode") ? throw JsonConvert.DeserializeObject<APIException>(json) : JsonConvert.DeserializeObject<T>(json);
        }

        private static Task<HttpResponseMessage> MakeRequestAsync(string type, params (string key, string value)[] parameters)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(
                parameters.Select(param => new KeyValuePair<string, string>(param.key, param.value))
                .Append(new KeyValuePair<string, string>("rnd", new Random().Next(int.MinValue, 0).ToString()))
            );
            string requestUri = $"/prizm?requestType={type}";
            return Client.PostAsync(requestUri, content);
        }
        #endregion
    }
}
