using System;
using System.Linq;
using CrappyPrizm.API;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using CrappyPrizm.RequestBuilders;

namespace CrappyPrizm.Endpoints
{
    public abstract class EndpointBase : IEndpoint
    {
        #region Var
        public IApiClient ApiClient { get; }
        public IRequestBuilder RequestBuilder { get; }
        public string EndpointUrl { get; }
        #endregion

        #region Init
        protected EndpointBase(IApiClient apiClient, IRequestBuilder requestBuilder, string endpointUrl)
        {
            ApiClient = apiClient;
            RequestBuilder = requestBuilder;
            EndpointUrl = endpointUrl;
        }
        #endregion

        #region Functions
        protected virtual Task<HttpResponseMessage> SendRequestAsync(string requestType, HttpMethod? httpMethod = null, IEnumerable<KeyValuePair<string, string>>? content = null)
        {
            httpMethod ??= HttpMethod.Post;

            content ??= Array.Empty<KeyValuePair<string, string>>();
            content = content.Append(new KeyValuePair<string, string>("requestType", requestType));

            HttpRequestMessage request = RequestBuilder.CreateRequest(httpMethod, EndpointUrl, content);
            return ApiClient.HttpClient.SendAsync(request);
        }

        protected Task<HttpResponseMessage> SendRequestAsync(string requestType, params KeyValuePair<string, string>[] content) => SendRequestAsync(requestType, null, (IEnumerable<KeyValuePair<string, string>>)content);
        protected Task<HttpResponseMessage> SendRequestAsync(string requestType, HttpMethod httpMethod, params KeyValuePair<string, string>[] content) => SendRequestAsync(requestType, httpMethod, (IEnumerable<KeyValuePair<string, string>>)content);
        
        protected Task<HttpResponseMessage> SendRequestAsync(string requestType, params KeyValuePair<string, object?>[] content) => SendRequestAsync(requestType, null, content.Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString() ?? string.Empty)));
        protected Task<HttpResponseMessage> SendRequestAsync(string requestType, HttpMethod httpMethod, params KeyValuePair<string, object?>[] content) => SendRequestAsync(requestType, httpMethod, content.Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString() ?? string.Empty)));
        
        protected Task<HttpResponseMessage> SendRequestAsync(string requestType, params (string key, object? value)[] content) => SendRequestAsync(requestType, null, content.Select(x => new KeyValuePair<string, string>(x.key, x.value?.ToString() ?? string.Empty)));
        protected Task<HttpResponseMessage> SendRequestAsync(string requestType, HttpMethod httpMethod, params (string key, object? value)[] content) => SendRequestAsync(requestType, httpMethod, content.Select(x => new KeyValuePair<string, string>(x.key, x.value?.ToString() ?? string.Empty)));
        
        protected Task<HttpResponseMessage> SendRequestAsync(string requestType, params (string key, string value)[] content) => SendRequestAsync(requestType, null, content.Select(x => new KeyValuePair<string, string>(x.key, x.value)));
        protected Task<HttpResponseMessage> SendRequestAsync(string requestType, HttpMethod httpMethod, params (string key, string value)[] content) => SendRequestAsync(requestType, httpMethod, content.Select(x => new KeyValuePair<string, string>(x.key, x.value)));
        #endregion
    }
}
