using System;
using WebSiteMeta.Scraper;

namespace WebSiteMeta.Sample
{
    public class Program
    {
        static void Main(string[] args)
        {
            var wsm = new FindMetaData();
            wsm.Run(args[0]);
            var result = wsm.MetaData;
            Console.WriteLine($"Url: {result.Url}");
            Console.WriteLine($"Title: {result.Title}");
            Console.WriteLine($"Description: {result.Description}");                        
        }
    }
}
