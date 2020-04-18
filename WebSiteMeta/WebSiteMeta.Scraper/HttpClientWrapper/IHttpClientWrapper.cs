using System.Threading.Tasks;

namespace WebSiteMeta.Scraper.HttpClientWrapper
{
    public interface IHttpClientWrapper
    {
        Task<(bool isSuccess, string data)> GetHttpData(string url);
    }
}