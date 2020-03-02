namespace CrappyPrizm.Exceptions
{
    [ErrorCodeLinker(ErrorCode.UnknownAccount)]
    public class UnknownAccountException : APIException
    {
        public UnknownAccountException(string? errorDescription, ErrorCode errorCode) : base(errorDescription, errorCode) { }
    }
}
