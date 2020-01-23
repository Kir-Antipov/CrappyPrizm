using System;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Convert = CrappyPrizm.Tools.Convert;

namespace CrappyPrizm
{
    public static class API
    {
        #region Var
        private const string Url = "https://wallet.prizm.space/";
        private static readonly HttpClient Client = new HttpClient { BaseAddress = new Uri(Url) };
        #endregion

        #region Methods
        public static Task<Account?> GetAccountAsync(BigInteger accountId) => GetAccountAsync(Convert.AccountIdToAddress(accountId));
        public static Task<Account?> GetAccountAsync(string address) => MakeRequestAsync<Account?>("getAccount", ("account", address));

        public static IAsyncEnumerable<Transaction> GetTransactionsAsync(string address, int numberOfConfirmations = 0) => GetTransactionsAsync(Convert.AddressToAccountId(address), numberOfConfirmations);
        public static IAsyncEnumerable<Transaction> GetTransactionsAsync(BigInteger accountId, int numberOfConfirmations = 0) => GetBlockchainTransactionsAsync(accountId, numberOfConfirmations).Select(x => x.Prepare());

        private static async IAsyncEnumerable<RawTransactionDetails> GetBlockchainTransactionsAsync(BigInteger accountId, int numberOfConfirmations)
        {
            const int bucketSize = 20;
            int firstIndex = -bucketSize;
            int lastIndex = -1;
            if (numberOfConfirmations < 0)
                numberOfConfirmations = 0;

            do
            {
                firstIndex += bucketSize;
                lastIndex += bucketSize;
                RawTransactionsContainer? container = await MakeRequestAsync<RawTransactionsContainer?>("getBlockchainTransactions",    ("account", accountId.ToString()),
                                                                                                                                        ("numberOfConfirmations", numberOfConfirmations.ToString()),
                                                                                                                                        ("firstIndex", firstIndex.ToString()),
                                                                                                                                        ("lastIndex", lastIndex.ToString()));
                if (container is null)
                    yield break;
                bool iterated = false;
                foreach (RawTransactionDetails details in container.Transactions)
                {
                    iterated = true;
                    yield return details;
                }
                if (!iterated)
                    yield break;
            }
            while (true);
        }

        public static Task<Transaction?> SendAsync(string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message? comment = null) => SendAsync(Convert.BytesToHex(Convert.SecretPhraseToPublicKey(secretPhrase)), secretPhrase, recipient, recipientPublicKey, amount, comment);
        public static Task<Transaction?> SendAsync(string secretPhrase, string recipientPublicKey, decimal amount, Message? comment = null) => SendAsync(Convert.BytesToHex(Convert.SecretPhraseToPublicKey(secretPhrase)), secretPhrase, Convert.AccountIdToAddress(Convert.PublicKeyToAccountId(Convert.HexToBytes(recipientPublicKey))), recipientPublicKey, amount, comment);
        public static async Task<Transaction?> SendAsync(string publicKey, string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message? comment = null)
        {
            RawTransaction? raw = await SendMoneyAsync(publicKey, secretPhrase, recipient, recipientPublicKey, amount, comment ?? "");
            if (raw is null)
                return null;

            BroadcastedTransaction? broadcasted = await BroadcastTransactionAsync(JsonConvert.SerializeObject(raw.Details.Attachment), Convert.BytesToHex(Convert.UnsignedBytesToSigned(Convert.HexToBytes(raw.Bytes), secretPhrase)));
            if (broadcasted is null)
                return null;

            return raw.Prepare(broadcasted, recipientPublicKey);
        }

        private static Task<RawTransaction?> SendMoneyAsync(string publicKey, string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message comment)
        {
            EncryptedMessage encryptedMessage = comment.Encrypt(recipientPublicKey, secretPhrase);
            EncryptedMessage encryptedToSelfMessage = comment.Encrypt(publicKey, secretPhrase);


            return MakeRequestAsync<RawTransaction?>("sendMoney",   ("deadline", "1440"),
                                                                    ("amountNQT", Convert.AmountToCoins(amount).ToString("G29")),
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

        private static Task<BroadcastedTransaction?> BroadcastTransactionAsync(string attachment, string bytes) => MakeRequestAsync<BroadcastedTransaction?>("broadcastTransaction", ("prunableAttachmentJSON", attachment), ("transactionBytes", bytes));
        #endregion

        #region Helpers
        private static async Task<T> MakeRequestAsync<T>(string type, params (string key, string value)[] parameters)
        {
            HttpResponseMessage response = await MakeRequestAsync(type, parameters);
            string json = await response.Content.ReadAsStringAsync();
            return json.Contains("error") ? default : JsonConvert.DeserializeObject<T>(json);
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
