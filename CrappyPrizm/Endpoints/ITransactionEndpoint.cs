using System;
using System.Numerics;
using CrappyPrizm.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CrappyPrizm.Endpoints
{
    public interface ITransactionEndpoint : IEndpoint
    {
        IAsyncEnumerable<Transaction> GetTransactionsAsync(string address, int? minNumberOfConfirmations = null, DateTime? minDateUtc = null, TransactionType? type = null, int? bucketSize = null);
        IAsyncEnumerable<Transaction> GetTransactionsAsync(BigInteger accountId, int? minNumberOfConfirmations = null, DateTime? minDateUtc = null, TransactionType? type = null, int? bucketSize = null);

        decimal ComputeComission(decimal amount);

        Task<Transaction> SendAsync(string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message? comment = null);
        Task<Transaction> SendAsync(string secretPhrase, string recipientPublicKey, decimal amount, Message? comment = null);
        Task<Transaction> SendAsync(string publicKey, string secretPhrase, string recipient, string recipientPublicKey, decimal amount, Message? comment = null);

        Task<Transaction> SendAllAsync(string secretPhrase, string recipient, string recipientPublicKey, Message? comment = null);
        Task<Transaction> SendAllAsync(string secretPhrase, string recipientPublicKey, Message? comment = null);
        Task<Transaction> SendAllAsync(string publicKey, string secretPhrase, string recipient, string recipientPublicKey, Message? comment = null);
    }
}
