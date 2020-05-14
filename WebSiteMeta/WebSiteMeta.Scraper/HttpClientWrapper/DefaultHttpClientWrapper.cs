using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task<(bool, string)> GetHttpData(string url, Encoding encoding)
        {
            HttpResponseMessage result;

            try
            {
                result = await _httpClient.GetAsync(url);
                if (!result.IsSuccessStatusCode)
                {
                    if (result.StatusCode == System.Net.HttpStatusCode.Moved 
                        || result.StatusCode == System.Net.HttpStatusCode.MovedPermanently
                        || result.StatusCode == System.Net.HttpStatusCode.Found)
                    {
                        return await GetHttpData(result.Headers.Location.AbsoluteUri.ToString(), encoding);
                    }
                    
                    return (false, string.Empty);
                }
            }
            catch (HttpRequestException ex)
            {
                return (false, ex.Message);
            }

            var stream = await result.Content.ReadAsStreamAsync();

            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                string content = await reader.ReadToEndAsync();
                return (true, content);
            }
        }
    }
}
