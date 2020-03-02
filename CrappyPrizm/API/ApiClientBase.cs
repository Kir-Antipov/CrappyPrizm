using System;
using System.Net.Http;
using System.Reflection;
using CrappyPrizm.Endpoints;
using System.Collections.Concurrent;

namespace CrappyPrizm.API
{
    public abstract class ApiClientBase : IApiClient
    {
        #region Var
        public HttpClient HttpClient { get; }

        private readonly bool DisposeClient;

        private readonly ConcurrentDictionary<IntPtr, IEndpoint> Endpoints;
        #endregion

        #region Init
        public ApiClientBase(string baseUrl) : this(new HttpClient { BaseAddress = new Uri(baseUrl) }, true) { }

        public ApiClientBase(HttpClient httpClient) : this(httpClient, false) { }

        public ApiClientBase(HttpClient httpClient, bool disposeClient)
        {
            HttpClient = httpClient;
            DisposeClient = disposeClient;
            Endpoints = new ConcurrentDictionary<IntPtr, IEndpoint>();
        }
        #endregion

        #region Functions
        protected IEndpoint? TryGetEndpoint(IntPtr typeHandle) => Endpoints.TryGetValue(typeHandle, out IEndpoint endpoint) ? endpoint : default;
        
        protected void SetEndpoint(IntPtr typeHandle, IEndpoint endpoint) => Endpoints.AddOrUpdate(typeHandle, endpoint, (a, b) => endpoint);

        public virtual TEndpoint Get<TEndpoint>() where TEndpoint : class, IEndpoint
        {
            Type endpointType = typeof(TEndpoint);
            IntPtr typeHandle = typeof(TEndpoint).TypeHandle.Value;
            IEndpoint? endpoint = TryGetEndpoint(typeHandle);
            if (endpoint is TEndpoint result)
                return result;

            ConstructorInfo constructor = endpointType.GetConstructor(new[] { typeof(IApiClient) });
            if (constructor is null)
                throw new TypeInitializationException(endpointType.FullName, new MissingMethodException("A constructor accepting `IApiClient` is required"));

            result = (TEndpoint)constructor.Invoke(new object[] { this });
            SetEndpoint(typeHandle, result);
            return result;
        }

        public virtual void Dispose()
        {
            if (DisposeClient)
                HttpClient.Dispose();
        }
        #endregion
    }
}
