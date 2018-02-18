using System.Net.Http;
using System.Threading.Tasks;

namespace DinkumCoin.Api.Client.Http
{
    public interface IHttpClient
    {

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}