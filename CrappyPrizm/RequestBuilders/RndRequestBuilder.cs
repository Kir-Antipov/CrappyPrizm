using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CrappyPrizm.RequestBuilders
{
    public class RndRequestBuilder : RequestBuilderBase
    {
        #region Var
        private static readonly RNGCryptoServiceProvider RandomNumberGenerator = new RNGCryptoServiceProvider();
        #endregion

        #region Functions
        public override HttpRequestMessage CreateRequest(HttpMethod method, string url, IEnumerable<KeyValuePair<string, string>>? content = null)
        {
            content ??= Array.Empty<KeyValuePair<string, string>>();

            Span<byte> bytes = stackalloc byte[4];
            RandomNumberGenerator.GetBytes(bytes);
            int rnd = BitConverter.ToInt32(bytes);

            return base.CreateRequest(method, url, content.Append(new KeyValuePair<string, string>("rnd", rnd.ToString())));
        }
        #endregion
    }
}
