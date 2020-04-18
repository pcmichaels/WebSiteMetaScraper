using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteMeta.Scraper.HttpClientWrapper
{
    public class EmptyHttpClientWrapper : IHttpClientWrapper
    {
        private readonly string _toReturn;

        public EmptyHttpClientWrapper(string toReturn)
        {
            _toReturn = toReturn;
        }

        public Task<(bool, string)> GetHttpData(string url)
        {
            return Task.FromResult((true, _toReturn));
        }
    }
}
