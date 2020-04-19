﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteMeta.Scraper.HttpClientWrapper
{
    public class DefaultHttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        public DefaultHttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool, string)> GetHttpData(string url)
        {
            var result = await _httpClient.GetAsync(url);
            if (!result.IsSuccessStatusCode) return (false, string.Empty);

            string content = await result.Content.ReadAsStringAsync();
            return (true, content);
        }
    }
}