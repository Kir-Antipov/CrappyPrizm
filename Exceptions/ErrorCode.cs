namespace CrappyPrizm.Exceptions
{
    public enum ErrorCode
    {
        InvalidRequest = 1,

        MissingData = 3,

        InvalidData = 4,

        NotForging = 5,
        UnknownAccount = 5,

        NotEnoughFunds = 6,
        NotEnoughAssets = 6,
        NotEnoughCurrency = 6,

        NotAllowed = 7,

        DecryptionFailed = 8,
        AlreadyDelivered = 8,
        DuplicateRefund = 8,
        GoodsNotDelivired = 8,
        NoMessage = 8,
        HeightNotAvailable = 8,
        CantDeleteCurrency = 8,
        PollFinished = 8,
        PhasingTransactionFinished = 8,

        NotAvailable = 9,

        HashesMismatch = 10,

        RequiredBlockNotFound = 13,

        RequiredLastBlockNotFound = 14,

        PrunedTransaction = 15,

        Disabled = 16
    }
}