using CrappyPrizm.API;
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
    }
}
