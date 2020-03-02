using System;

namespace CrappyPrizm.Exceptions
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    internal sealed class ErrorCodeLinkerAttribute : Attribute
    {
        #region Var
        public ErrorCode[] Codes { get; }
        #endregion

        #region Init
        public ErrorCodeLinkerAttribute(params ErrorCode[] codes) => Codes = codes;
        #endregion
    }
}
