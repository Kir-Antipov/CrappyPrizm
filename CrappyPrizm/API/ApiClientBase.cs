using System;
using System.Net.Http;

namespace CrappyPrizm.API
{
    public abstract class ApiClientBase : IApiClient
    {
        #region Var
        public HttpClient HttpClient { get; }
        private bool DisposeClient { get; }
        #endregion

        #region Init
        public ApiClientBase(string baseUrl) : this(new HttpClient { BaseAddress = new Uri(baseUrl) }, true) { }

        public ApiClientBase(HttpClient httpClient) : this(httpClient, false) { }

        public ApiClientBase(HttpClient httpClient, bool disposeClient)
        {
            HttpClient = httpClient;
            DisposeClient = disposeClient;
        }
        #endregion

        #region Functions
        public virtual void Dispose()
        {
            if (DisposeClient)
                HttpClient.Dispose();
        }
        #endregion
    }
}
