using System;
using System.Net.Http;

namespace CrappyPrizm.API
{
    public class PrizmClient : ApiClientBase, IPrizmClient
    {
        #region Var
        protected const string BaseUrl = "https://wallet.prizm.space/";
        private static readonly HttpClient DefaultClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        #endregion

        #region Init
        public PrizmClient() : base(DefaultClient, false) { }

        public PrizmClient(string baseUrl) : base(baseUrl) { }

        public PrizmClient(HttpClient httpClient) : base(httpClient) { }

        public PrizmClient(HttpClient httpClient, bool disposeClient) : base(httpClient, disposeClient) { }
        #endregion
    }
}
