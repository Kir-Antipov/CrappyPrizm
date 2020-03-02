using System.Linq;
using CrappyPrizm.API;
using Newtonsoft.Json;
using System.Net.Http;
using CrappyPrizm.Exceptions;
using System.Threading.Tasks;
using System.Collections.Generic;
using CrappyPrizm.RequestBuilders;

namespace CrappyPrizm.Endpoints
{
    public class PrizmEndpointBase : EndpointBase
    {
        #region Var
        protected const string BaseEndpointUrl = "/prizm";

        private static readonly IRequestBuilder DefaultRequestBuilder = new RndRequestBuilder();
        #endregion

        #region Init
        public PrizmEndpointBase(IApiClient apiClient) : base(apiClient, DefaultRequestBuilder, BaseEndpointUrl) { }

        public PrizmEndpointBase(IApiClient apiClient, IRequestBuilder requestBuilder) : base(apiClient, requestBuilder, BaseEndpointUrl) { }
        #endregion

        #region Functions
        protected async Task<T> SendRequestAsync<T>(string requestType, HttpMethod? httpMethod = null, IEnumerable<KeyValuePair<string, string>>? content = null)
        {
            HttpResponseMessage response = await base.SendRequestAsync(requestType, httpMethod, content);
            string json = await response.Content.ReadAsStringAsync();
            return json.Contains("errorCode") ? throw JsonConvert.DeserializeObject<APIException>(json) : JsonConvert.DeserializeObject<T>(json);
        }

        protected Task<T> SendRequestAsync<T>(string requestType, params KeyValuePair<string, string>[] content) => SendRequestAsync<T>(requestType, null, (IEnumerable<KeyValuePair<string, string>>)content);
        protected Task<T> SendRequestAsync<T>(string requestType, HttpMethod httpMethod, params KeyValuePair<string, string>[] content) => SendRequestAsync<T>(requestType, httpMethod, (IEnumerable<KeyValuePair<string, string>>)content);

        protected Task<T> SendRequestAsync<T>(string requestType, params KeyValuePair<string, object?>[] content) => SendRequestAsync<T>(requestType, null, content.Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString() ?? string.Empty)));
        protected Task<T> SendRequestAsync<T>(string requestType, HttpMethod httpMethod, params KeyValuePair<string, object?>[] content) => SendRequestAsync<T>(requestType, httpMethod, content.Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString() ?? string.Empty)));

        protected Task<T> SendRequestAsync<T>(string requestType, params (string key, object? value)[] content) => SendRequestAsync<T>(requestType, null, content.Select(x => new KeyValuePair<string, string>(x.key, x.value?.ToString() ?? string.Empty)));
        protected Task<T> SendRequestAsync<T>(string requestType, HttpMethod httpMethod, params (string key, object? value)[] content) => SendRequestAsync<T>(requestType, httpMethod, content.Select(x => new KeyValuePair<string, string>(x.key, x.value?.ToString() ?? string.Empty)));

        protected Task<T> SendRequestAsync<T>(string requestType, params (string key, string value)[] content) => SendRequestAsync<T>(requestType, null, content.Select(x => new KeyValuePair<string, string>(x.key, x.value)));
        protected Task<T> SendRequestAsync<T>(string requestType, HttpMethod httpMethod, params (string key, string value)[] content) => SendRequestAsync<T>(requestType, httpMethod, content.Select(x => new KeyValuePair<string, string>(x.key, x.value)));
        #endregion
    }
}
