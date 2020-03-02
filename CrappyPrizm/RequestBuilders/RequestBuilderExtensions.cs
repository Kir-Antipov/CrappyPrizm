using System.Linq;
using System.Net.Http;
using System.Collections.Generic;

namespace CrappyPrizm.RequestBuilders
{
    public static class RequestBuilderExtensions
    {
        public static HttpRequestMessage CreateRequest(this IRequestBuilder builder, HttpMethod method, string url, params KeyValuePair<string, string>[] content) => builder.CreateRequest(method, url, content);

        public static HttpRequestMessage CreateRequest(this IRequestBuilder builder, HttpMethod method, string url, params KeyValuePair<string, object?>[] content) => builder.CreateRequest(method, url, content.Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString() ?? string.Empty)));
        public static HttpRequestMessage CreateRequest(this IRequestBuilder builder, HttpMethod method, string url, IEnumerable<KeyValuePair<string, object?>> content) => builder.CreateRequest(method, url, content.Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString() ?? string.Empty)));

        public static HttpRequestMessage CreateRequest(this IRequestBuilder builder, HttpMethod method, string url, params (string key, string value)[] content) => builder.CreateRequest(method, url, content.Select(x => new KeyValuePair<string, string>(x.key, x.value)));
        public static HttpRequestMessage CreateRequest(this IRequestBuilder builder, HttpMethod method, string url, IEnumerable<(string key, string value)> content) => builder.CreateRequest(method, url, content.Select(x => new KeyValuePair<string, string>(x.key, x.value)));
        
        public static HttpRequestMessage CreateRequest(this IRequestBuilder builder, HttpMethod method, string url, params (string key, object? value)[] content) => builder.CreateRequest(method, url, content.Select(x => new KeyValuePair<string, string>(x.key, x.value?.ToString() ?? string.Empty)));
        public static HttpRequestMessage CreateRequest(this IRequestBuilder builder, HttpMethod method, string url, IEnumerable<(string key, object? value)> content) => builder.CreateRequest(method, url, content.Select(x => new KeyValuePair<string, string>(x.key, x.value?.ToString() ?? string.Empty)));
    }
}
