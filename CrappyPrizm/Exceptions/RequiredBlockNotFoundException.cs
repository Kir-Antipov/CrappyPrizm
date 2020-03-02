namespace CrappyPrizm.Exceptions
{
    [ErrorCodeLinker(ErrorCode.RequiredBlockNotFound, ErrorCode.RequiredLastBlockNotFound)]
    public class RequiredBlockNotFoundException : APIException
    {
        public RequiredBlockNotFoundException(string? errorDescription, ErrorCode errorCode) : base(errorDescription, errorCode) { }
    }
}
