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
            var result = await wsm.Run(args[0]);
            
            if (!result.IsSuccess)
            {
                Console.WriteLine("Errors occured dueing the call");
                foreach(var error in result.Errors)
                {
                    Console.WriteLine(error);
                }
                return;
            }

            Console.WriteLine($"Url: {result.Metadata.Url}");
            Console.WriteLine($"Title: {result.Metadata.Title}");
            Console.WriteLine($"Description: {result.Metadata.Description}");

            Console.ReadLine();
        }
    }
}
