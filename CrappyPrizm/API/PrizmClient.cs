using System;
using System.Net.Http;
using CrappyPrizm.Endpoints;

namespace CrappyPrizm.API
{
    public class PrizmClient : ApiClientBase, IPrizmClient
    {
        #region Var
        public IAccountEndpoint Accounts { get; }

        public ITransactionEndpoint Transactions { get; }

        protected const string BaseUrl = "https://wallet.prizm.space/";
        private static readonly HttpClient DefaultClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        #endregion

        #region Init
        public PrizmClient() : this(DefaultClient, false) { }

        public PrizmClient(string baseUrl) : this(new HttpClient { BaseAddress = new Uri(baseUrl) }, true) { }

        public PrizmClient(HttpClient httpClient) : this(httpClient, false) { }

        public PrizmClient(HttpClient httpClient, bool disposeClient) : base(httpClient, disposeClient) 
        {
            Accounts = new AccountEndpoint(this);
            Transactions = new TransactionEndpoint(this);
        }
        #endregion
    }
}
