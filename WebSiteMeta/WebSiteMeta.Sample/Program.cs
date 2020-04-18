using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebSiteMeta.Scraper;
using WebSiteMeta.Scraper.HttpClientWrapper;

namespace WebSiteMeta.Sample
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var httpClientWrapper = new DefaultHttpClientWrapper(httpClient);

            var wsm = new FindMetaData(httpClientWrapper);
            await wsm.Run(args[0]);
            var result = wsm.MetaData;
            Console.WriteLine($"Url: {result.Url}");
            Console.WriteLine($"Title: {result.Title}");
            Console.WriteLine($"Description: {result.Description}");                        
        }
    }
}
