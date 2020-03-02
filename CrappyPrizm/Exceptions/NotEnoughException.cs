namespace CrappyPrizm.Exceptions
{
    [ErrorCodeLinker(ErrorCode.NotEnoughAssets, ErrorCode.NotEnoughCurrency)]
    public class NotEnoughException : APIException
    {
        public NotEnoughException(string? errorDescription, ErrorCode errorCode) : base(errorDescription, errorCode) { }
    }
}
