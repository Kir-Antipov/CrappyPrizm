using System;
using System.Linq;
using CrappyPrizm.API;
using Newtonsoft.Json;
using System.Numerics;
using CrappyPrizm.Models;
using CrappyPrizm.Exceptions;
using System.Threading.Tasks;
using System.Collections.Generic;
using CrappyPrizm.RequestBuilders;
using Convert = CrappyPrizm.Tools.Convert;

namespace CrappyPrizm.Endpoints
{
    public class TransactionEndpoint : PrizmEndpointBase, ITransactionEndpoint
    {
        #region Var
        private readonly IAccountEndpoint Accounts;
        #endregion

        #region Init
        public TransactionEndpoint(IApiClient apiClient) : base(apiClient) => Accounts = new AccountEndpoint(apiClient);

        public TransactionEndpoint(IApiClient apiClient, IRequestBuilder requestBuilder) : base(apiClient, requestBuilder) => Accounts = new AccountEndpoint(apiClient, requestBuilder);
        #endregion

        #region Functions
        public IAsyncEnumerable<Transaction> GetTransactionsAsync(string address, int? minNumberOfConfirmations = null, DateTime? minDateUtc = null, TransactionType? type = null, int? bucketSize = null) => GetTransactionsAsync(Convert.AddressToAccountId(address), minNumberOfConfirmations, minDateUtc, type, bucketSize);
        
        public IAsyncEnumerable<Transaction> GetTransactionsAsync(BigInteger accountId, int? minNumberOfConfirmations = null, DateTime? minDateUtc = null, TransactionType? type = null, int? bucketSize = null) => GetBlockchainTransactionsAsync(accountId, minNumberOfConfirmations ?? 0, Convert.DateTimeToTheirCrappyTimestamp(minDateUtc ?? DateTime.UnixEpoch), type ?? TransactionType.All, bucketSize ?? 25).Select(x => x.Prepare());

        private async IAsyncEnumerable<RawTransactionDetails> GetBlockchainTransactionsAsync(BigInteger accountId, int numberOfConfirmations, long timestamp, TransactionType type, int bucketSize)
        {
            int firstIndex = -bucketSize;
            int lastIndex = -1;
            numberOfConfirmations = Math.Max(numberOfConfirmations, 0);
            timestamp = Math.Max(timestamp, 0);

            do
            {
                firstIndex += bucketSize;
                lastIndex += bucketSize;
                RawTransactionsContainer container = await SendRequestAsync<RawTransactionsContainer>("getBlockchainTransactions",  ("account", accountId),
                                                                                                                                    ("numberOfConfirmations", numberOfConfirmations),
                                                                                                                                    ("timestamp", timestamp),
                                                                                                                                    ("firstIndex", firstIndex),
                                                                                                                                    ("lastIndex", lastIndex));
                if (container.Transactions.Length == 0)
                    yield break;

                IEnumerable<RawTransactionDetails> transactions = type switch
                {
                    TransactionType.Incoming => container.Transactions.Where(x => x.Recipient == accountId),
                    TransactionType.Outgoing => container.Transactions.Where(x => x.Sender == accountId),
                    TransactionType.Paramining => container.Transactions.Where(x => x.Sender == Account.Genesis.Id),
                    _ => container.Transactions
                };
                foreach (RawTransactionDetails details in transactions)
                    yield return details;
            }
            while (true);
        }

        public decimal ComputeComission(decimal amount)
        {
            if (amount < 12)
                return 0.05m;

            return Math.Min(Math.Round((6 + (amount - 12) / 2) / 100 - 0.005m, 2, MidpointRounding.AwayFromZero), 10);
        }

        public Task<Transaction> SendAsync(string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message? comment = null) => SendAsync(Convert.BytesToHex(Convert.SecretPhraseToPublicKey(secretPhrase)), secretPhrase, recipient, recipientPublicKey, amount, comment);

        public Task<Transaction> SendAsync(string secretPhrase, string recipientPublicKey, decimal amount, Message? comment = null) => SendAsync(Convert.BytesToHex(Convert.SecretPhraseToPublicKey(secretPhrase)), secretPhrase, Convert.AccountIdToAddress(Convert.PublicKeyToAccountId(Convert.HexToBytes(recipientPublicKey))), recipientPublicKey, amount, comment);

        public async Task<Transaction> SendAsync(string publicKey, string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message? comment = null)
        {
            RawTransaction raw = await SendMoneyAsync(publicKey, secretPhrase, recipient, recipientPublicKey, amount, comment ?? string.Empty);
            BroadcastedTransaction broadcasted = await BroadcastTransactionAsync(JsonConvert.SerializeObject(raw.Details.Attachment), Convert.BytesToHex(Convert.UnsignedBytesToSigned(Convert.HexToBytes(raw.Bytes), secretPhrase)));

            return raw.Prepare(broadcasted, recipientPublicKey);
        }

        public Task<Transaction> SendAllAsync(string secretPhrase, string recipient, string recipientPublicKey, Message? comment = null) => SendAllAsync(Convert.BytesToHex(Convert.SecretPhraseToPublicKey(secretPhrase)), secretPhrase, recipient, recipientPublicKey, comment);
        
        public Task<Transaction> SendAllAsync(string secretPhrase, string recipientPublicKey, Message? comment = null) => SendAllAsync(Convert.BytesToHex(Convert.SecretPhraseToPublicKey(secretPhrase)), secretPhrase, Convert.AccountIdToAddress(Convert.PublicKeyToAccountId(Convert.HexToBytes(recipientPublicKey))), recipientPublicKey, comment);
        
        public async Task<Transaction> SendAllAsync(string publicKey, string secretPhrase, string recipient, string recipientPublicKey, Message? comment = null)
        {
            Account account = await Accounts.GetAccountAsync(Convert.PublicKeyToAccountId(Convert.HexToBytes(publicKey)));
            decimal amount = account.Balance - ComputeComission(account.Balance);
            if (amount <= 0)
                throw new NotEnoughFundsException("Wallet balance less than or equal to transfer fee", ErrorCode.NotEnoughFunds);

            amount += 0.01m;
            if (amount + ComputeComission(amount) > account.Balance)
                amount -= 0.01m;

            return await SendAsync(publicKey, secretPhrase, recipient, recipientPublicKey, amount, comment);
        }

        private Task<RawTransaction> SendMoneyAsync(string publicKey, string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message comment)
        {
            EncryptedMessage encryptedMessage = comment.Encrypt(recipientPublicKey, secretPhrase);
            EncryptedMessage encryptedToSelfMessage = comment.Encrypt(publicKey, secretPhrase);

            return SendRequestAsync<RawTransaction>("sendMoney",    ("deadline", "1440"),
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

        private Task<BroadcastedTransaction> BroadcastTransactionAsync(string attachment, string bytes) => SendRequestAsync<BroadcastedTransaction>("broadcastTransaction", ("prunableAttachmentJSON", attachment), ("transactionBytes", bytes));
        #endregion
    }
}
