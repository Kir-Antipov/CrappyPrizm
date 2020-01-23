namespace CrappyPrizm.Exceptions
{
    [ErrorCodeLinker(ErrorCode.InvalidRequest, ErrorCode.MissingData, ErrorCode.InvalidData)]
    public class InvalidRequestException : APIException
    {
        public InvalidRequestException(string? errorDescription, ErrorCode errorCode) : base(errorDescription, errorCode) { }
    }
}
