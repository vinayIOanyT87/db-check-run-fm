using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Wingware
{
    public sealed class WebApi : BaseWebApi
    {
        public WebApi(HttpClient httpClient) : base(httpClient) { }
        public string UserName;
        public string Password;

        public async Task<TOut> GetAuthenticatedURLAsync<TOut>(string url, object payload)
        {
            return await PostAsync<TOut>(HttpMethod.Post, url, GetAuthorizationHeadersForBasicAuth(), payload);
        }

        public Dictionary<string, string> GetAuthorizationHeadersForBasicAuth()
        {
            var headers = new Dictionary<string, string>();
            var basicAuth = GetBasicAuth(UserName, Password);
            headers.Add("Authorization", basicAuth);
            return headers;
        }

        private string GetBasicAuth(string UserName, string Password)
        {
            var byteArray = Encoding.ASCII.GetBytes($"{UserName}:{Password}");
            var authString = Convert.ToBase64String(byteArray);
            return $"Basic {authString}";
        }
    }
}
