namespace CrappyPrizm.Exceptions
{
    [ErrorCodeLinker(ErrorCode.NotAllowed, ErrorCode.Disabled, ErrorCode.NotAvailable)]
    public class ForbiddenException : APIException
    {
        public ForbiddenException(string? errorDescription, ErrorCode errorCode) : base(errorDescription, errorCode) { }
    }
}
