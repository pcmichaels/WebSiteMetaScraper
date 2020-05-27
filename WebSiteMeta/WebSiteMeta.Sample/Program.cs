using System;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using WebSiteMeta.Scraper;
using WebSiteMeta.Scraper.HttpClientWrapper;

namespace WebSiteMeta.Sample
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            switch (args[0])
            {
                case "-t":
                    string[] testSites = GetTestSites();
                    await RunTests(testSites);
                    break;

                case "-50":
                    string[] testTop50Sites = GetTop50Sites();
                    await RunTests(testTop50Sites);
                    break;

                case "-s":
                    await RunTests(new[] { args[1] });
                    break;

                case "-p":
                    Console.Write("Enter site address, or multiple with a comma between (e.g. www.sun.com,www.pmichaels.net): ");
                    string tests = Console.ReadLine();
                    await RunTests(tests.Split(","));
                    break;
            }

            Console.ReadLine();
        }

        private static string[] GetTestSites()
        {
            return new[]
            {
                "www.pmichaels.net",
                "http://sun.com",
                "https://www.stackoverflow.com",
                "www.microsoft.com",
                "msdn.com",
                "<script>alert('xss')</script>",
                "linkedin.com",
                "facebook.com",
                "twitter.com",
                "https://github.com/pcmichaels/WebSiteMetaScraper",
                "www.amazon.co.uk",
                "https://www.nytimes.com/",
                "theguardian.com",
                "alexa.com",
                "baidu.com",
                "qq.com",
                "sohu.com"
            };
        }

        // Top 50 sites from 
        // https://en.wikipedia.org/wiki/List_of_most_popular_websites
        private static string[] GetTop50Sites()
        {
            return new[]
            {
                "google.com", 
                "youtube.com", 
                "tmall.com", 
                "facebook.com", 
                "baidu.com", 
                "qq.com", 
                "sohu.com", 
                "login.tmall.com", 
                "taobao.com", 
                "360.cn",
                "yahoo.com",
                "jd.com",
                "wikipedia.org",
                "amazon.com",
                "sina.com.cn",
                "weibo.com",
                "pages.tmall.com",
                "live.com",
                "reddit.com",
                "netflix.com",
                "zoom.us",
                "xinhuanet.com",
                "Okezone.com",
                "blogspot.com",
                "office.com",
                "microsoft.com",
                "vk.com",
                "csdn.net",
                "instagram.com",
                "alipay.com",
                "yahoo.co.jp",
                "Twitch.tv",
                "bing.com",
                "google.com.hk",
                "bongacams.com",
                "microsoftonline.com",
                "livejasmin.com",
                "tribunnews.com",
                "panda.tv",
                "twitter.com",
                "zhanqi.tv",
                "worldometers.info",
                "stackoverflow.com",
                "naver.com",
                "amazon.co.jp",
                "tianya.cn",
                "google.co.in",
                "aliexpress.com",
                "ebay.com",
                "mama.cn"
            };
        }

        private static async Task RunTests(string[] args)
        {
            foreach (var url in args)
            {
                OutputHeader($"Trying {url}");
                FindMetaDataResult result = await RunTest(url);
                if (result == null) continue;

                //https://stackoverflow.com/questions/33579661/encoding-getencoding-cant-work-in-uwp-app
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var charset = Encoding.GetEncoding(result.Metadata.Charset);

                OutputValue("Charset", result.Metadata.Charset, charset);
                OutputValue("Url", result.Metadata.Url, charset);
                OutputValue("Title", result.Metadata.Title, charset);
                OutputValue("Description", result.Metadata.Description, charset);   
                
                foreach (var meta in result.Metadata.Meta)
                {
                    OutputValue(meta.Key, meta.Value, charset);
                }
            }
        }

        private static void OutputHeader(string text)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"--=={text}==--");

        }

        private static void OutputValue(string key, string value, Encoding encoding)
        {
            Console.OutputEncoding = encoding;
            if (string.IsNullOrWhiteSpace(value))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{key}: NOT SET");
            }
            else
            {                
                Console.WriteLine($"{key}: {value}");
            }
            Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(@$"Url is valid");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(@$"Url is invalid");
                Console.ResetColor();
            }

            var result = await wsm.Run(cleanUrl);

            if (!result.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Errors occured during the call");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error);
                }
                Console.ResetColor();
                return null;
            }

            return result;
        }
    }
}
