using System;
using System.Net.Http;

namespace CrappyPrizm.API
{
    public interface IApiClient : IDisposable
    {
        HttpClient HttpClient { get; }
    }
}
