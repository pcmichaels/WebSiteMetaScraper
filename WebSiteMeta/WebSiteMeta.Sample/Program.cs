using System;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using WebSiteMeta.Scraper;
using WebSiteMeta.Scraper.HttpClientWrapper;

namespace WebSiteMeta.Sample
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            foreach (var url in args)
            {
                FindMetaDataResult result = await RunTest(url);
                if (result == null) continue;

                Console.WriteLine($"Url: {result.Metadata.Url}");
                Console.WriteLine($"Title: {result.Metadata.Title}");
                Console.WriteLine($"Description: {result.Metadata.Description}");
            }

            Console.ReadLine();
        }

        private static async Task<FindMetaDataResult> RunTest(string url)
        {
            var httpClient = new HttpClient();
            var httpClientWrapper = new DefaultHttpClientWrapper(httpClient);

            var wsm = new FindMetaData(httpClientWrapper);

            var cleanUrl = wsm.CleanUrl(url);

            bool isValid = wsm.ValidateUrl(cleanUrl);
            if (isValid)
            {
                Console.WriteLine(@$"Url is valid");
            }
            else
            {
                Console.WriteLine(@$"Url is invalid");
            }

            var result = await wsm.Run(cleanUrl);

            if (!result.IsSuccess)
            {
                Console.WriteLine("Errors occured during the call");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error);
                }
                return null;
            }

            return result;
        }
    }
}
