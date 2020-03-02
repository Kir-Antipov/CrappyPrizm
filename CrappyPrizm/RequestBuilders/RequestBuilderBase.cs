using System.Net.Http;
using System.Collections.Generic;

namespace CrappyPrizm.RequestBuilders
{
    public abstract class RequestBuilderBase : IRequestBuilder
    {
        public virtual HttpRequestMessage CreateRequest(HttpMethod method, string url, IEnumerable<KeyValuePair<string, string>>? content = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            if (content is { })
                request.Content = new FormUrlEncodedContent(content);
            return request;
        }
    }
}
