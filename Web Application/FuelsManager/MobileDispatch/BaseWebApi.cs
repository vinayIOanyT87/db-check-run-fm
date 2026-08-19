using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FuelsManager.Wingware
{
    public abstract class BaseWebApi
    {
        private readonly HttpClient _httpClient;
        public BaseWebApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TOut> PostAsync<TOut>(HttpMethod httpMethod, string url, Dictionary<string, string> headers, object body)
        {
            HttpResponseMessage response = null;
            using (var request = new HttpRequestMessage(httpMethod, url))
            {
                AddHeaders(request, headers);
                AddBody(body, request);
                response = await _httpClient.SendAsync(request);
            }

            return await response.Content.ReadAsAsync<TOut>();
        }

        private void AddHeaders(HttpRequestMessage request, Dictionary<string, string> headers)
        {
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (headers == null) return;

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        private static void AddBody(object param, HttpRequestMessage request)
        {
            if (param != null)
            {
                var content = JsonConvert.SerializeObject(param);
                request.Content = new StringContent(content);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
        }
    }
}
