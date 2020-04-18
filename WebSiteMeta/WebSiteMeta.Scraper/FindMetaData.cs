using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace WebSiteMeta.Scraper
{
    public class FindMetaData
    {
        public Metadata MetaData { get; set; }

        public FindMetaData()
        {
            var httpClient = new HttpClient();
        }

        public FindMetaDataResult Run(string url)
        {
            if (!ValidateUrl(url))
            {
                return Fail($"Invalid URL: {url}");
            }

            var data = new Metadata();

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
