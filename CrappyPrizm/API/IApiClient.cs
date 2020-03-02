using System;
using System.Net.Http;
using CrappyPrizm.Endpoints;

namespace CrappyPrizm.API
{
    public interface IApiClient : IDisposable
    {
        HttpClient HttpClient { get; }

        TEndpoint Get<TEndpoint>() where TEndpoint : class, IEndpoint;
    }
}
