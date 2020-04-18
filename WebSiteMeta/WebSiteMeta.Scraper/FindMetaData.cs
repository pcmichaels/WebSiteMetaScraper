using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSiteMeta.Scraper.HttpClientWrapper;

namespace WebSiteMeta.Scraper
{
    public class FindMetaData
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public Metadata MetaData { get; set; }
        

        public FindMetaData(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<FindMetaDataResult> Run(string url)
        {
            if (!ValidateUrl(url.ToLowerInvariant()))
            {
                return Fail($"Invalid URL: {url}");
            }

            var data = new Metadata();
            var httpDataResult = await _httpClientWrapper.GetHttpData(url);
            if (!httpDataResult.isSuccess)
            {
                return Fail($"Unable to make call to {url}");
            }

            // Parse here

            return Success(data);
        }

        private FindMetaDataResult Success(Metadata data)
        {
            var findMetaDataResult = new FindMetaDataResult();            
            findMetaDataResult.IsSuccess = true;
            findMetaDataResult.Metadata = data;
            return findMetaDataResult;

        }

        private FindMetaDataResult Fail(string error)
        {
            var findMetaDataResult = new FindMetaDataResult();
            findMetaDataResult.Errors = new string[] { error };
            findMetaDataResult.IsSuccess = false;
            return findMetaDataResult;
        }

        private bool ValidateUrl(string url)
        {
            if (!url.Contains("//"))
            {
                url = $"https://{url}";
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return false;

            Uri uri;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uri);
            if (!result) return false;

            if (uri.Scheme == Uri.UriSchemeHttp 
                || uri.Scheme == Uri.UriSchemeHttps)
            {
                var regex = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
                return regex.IsMatch(url);
            }
            return false;
        }
    }
}
