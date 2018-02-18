using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DinkumCoin.Api.Client.Http
{
    public class HttpClient : IHttpClient, IDisposable
    {
        private System.Net.Http.HttpClient Client { get; }

        public HttpClient(){
            Client = new System.Net.Http.HttpClient();
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        =>  Client.SendAsync(request);

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
