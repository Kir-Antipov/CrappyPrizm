namespace CrappyPrizm.Exceptions
{
    [ErrorCodeLinker(ErrorCode.NotEnoughFunds)]
    public class NotEnoughFundsException : NotEnoughException
    {
        public NotEnoughFundsException(string? errorDescription, ErrorCode errorCode) : base(errorDescription, errorCode) { }
    }
}
