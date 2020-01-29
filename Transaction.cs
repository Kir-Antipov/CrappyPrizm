using System;
using System.Numerics;
using Convert = CrappyPrizm.Tools.Convert;

namespace CrappyPrizm
{
    public class Transaction
    {
        #region Var
        public BigInteger Id { get; }

        public string Hash { get; }

        public long Coins { get; }
        public long FeeInCoins { get; }

        public decimal Amount => Convert.CoinsToAmount(Coins);
        public decimal Fee => Convert.CoinsToAmount(FeeInCoins);

        public Account From { get; }
        public Account To { get; }

        public Block Block { get; }

        public DateTime Date { get; }

        public DateTime Deadline { get; }

        public int Confirmations { get; }

        public EncryptedMessage? EncryptedMessage { get; }
        public EncryptedMessage? SelfEncryptedMessage { get; }
        #endregion

        #region Init
        public Transaction(BigInteger id, string hash, Account from, Account to, long coins, long feeInCoins, Block block, DateTime date, DateTime deadline, int confirmations, EncryptedMessage? encryptedMessage = null, EncryptedMessage? selfEncryptedMessage = null)
        {
            Id = id;
            Hash = hash;
            From = from;
            To = to;
            Coins = coins;
            FeeInCoins = feeInCoins;
            Block = block;
            Date = date;
            Deadline = deadline;
            Confirmations = confirmations;
            EncryptedMessage = encryptedMessage;
            SelfEncryptedMessage = selfEncryptedMessage;
        }
        #endregion

        #region Functions
        public override string ToString() => Id.ToString();
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object? obj) => obj is Transaction transaction && transaction.Id == Id;
        #endregion
    }
}
