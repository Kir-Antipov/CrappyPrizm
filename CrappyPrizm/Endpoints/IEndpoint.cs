using CrappyPrizm.API;
using CrappyPrizm.RequestBuilders;

namespace CrappyPrizm.Endpoints
{
    public interface IEndpoint
    {
        IApiClient ApiClient { get; }

        IRequestBuilder RequestBuilder { get; }

        string EndpointUrl { get; }
    }
}
