using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteMeta.Scraper.HttpClientWrapper
{
    /// <summary>
    /// An implementation of the client wrapper that makes a call 
    /// using the httpClient that has been passed in
    /// </summary>
    public class DefaultHttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        public DefaultHttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool, string)> GetHttpData(string url)
        {
            HttpResponseMessage result;

            try
            {
                result = await _httpClient.GetAsync(url);
                if (!result.IsSuccessStatusCode) return (false, string.Empty);
            }
            catch (HttpRequestException ex)
            {
                return (false, ex.Message);
            }

            string content = await result.Content.ReadAsStringAsync();
            return (true, content);
        }
    }
}
