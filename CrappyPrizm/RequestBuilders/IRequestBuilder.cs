using System.Net.Http;
using System.Collections.Generic;

namespace CrappyPrizm.RequestBuilders
{
    public interface IRequestBuilder
    {
        HttpRequestMessage CreateRequest(HttpMethod method, string url, IEnumerable<KeyValuePair<string, string>>? content = null);
    }
}
