using System;
using System.Linq;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;

namespace CrappyPrizm.Exceptions
{
    public class APIException : Exception
    {
        #region Var
        public ErrorCode Code { get; }

        private static readonly Dictionary<ErrorCode, IntPtr> Exceptions;
        #endregion

        #region Init
        static APIException()
        {
            Exceptions = Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(x => typeof(APIException).IsAssignableFrom(x) && !x.IsAbstract)
                .Select(x => new { Type = x, x.GetCustomAttribute<ErrorCodeLinkerAttribute>()?.Codes })
                    .Where(x => x.Codes is { })
                    .SelectMany(x => x.Codes.Select(code => new KeyValuePair<ErrorCode, IntPtr>(code, x.Type.TypeHandle.Value)))
                        .Aggregate(new Dictionary<ErrorCode, IntPtr>(), (a, b) => { a[b.Key] = b.Value; return a; });
        }

        [JsonConstructor]
        public APIException(string? errorDescription, ErrorCode errorCode) : base(errorDescription) 
        {
            Code = errorCode;

            if (Exceptions.TryGetValue(errorCode, out IntPtr ptr))
                unsafe
                {
                    // `this` changes its type to the type of the child exception which describes the occurred error
                    APIException it = this;
                    TypedReference typedReference = __makeref(it);
                    IntPtr* pointer = (IntPtr*)(**(IntPtr**)&typedReference).ToPointer();
                    pointer[0] = ptr;
                }
        }
        #endregion
    }
}
