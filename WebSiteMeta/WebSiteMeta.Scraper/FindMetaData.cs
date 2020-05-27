using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSiteMeta.Scraper.HttpClientWrapper;

namespace WebSiteMeta.Scraper
{
    public class FindMetaData : IFindMetaData
    {
        private readonly IHttpClientWrapper _httpClientWrapper;        

        public FindMetaData(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        /// <summary>
        /// Scrape the site
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding">Override the encoding</param>
        /// <returns>An object representing the metadata that was found for the site</returns>
        public async Task<FindMetaDataResult> Run(string url, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;

            string cleanUrl = CleanUrl(url);
            if (!ValidateUrl(cleanUrl))
            {
                return Fail($"Invalid URL: {cleanUrl}");
            }
            
            var httpDataResult = await _httpClientWrapper.GetHttpData(cleanUrl, encoding);
            if (!httpDataResult.isSuccess)
            {
                return Fail($"Unable to make call to {cleanUrl}");
            }

            var pageSource = WebUtility.HtmlDecode(httpDataResult.data);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            var headNode = htmlDoc.DocumentNode.SelectSingleNode("//head");
            if (headNode == null) return Fail("Unable to find header node");

            string charset = GetCharset(headNode);
            if (charset != null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var charsetEncoding = Encoding.GetEncoding(charset);

                if (encoding == null || charsetEncoding.CodePage != encoding.CodePage)
                {
                    return await Run(url, charsetEncoding);
                }
            }
            else
            {
                charset = encoding.WebName;
            }

            var data = AssignResults(headNode, charset);

            return Success(data);
        }

        private Metadata AssignResults(HtmlNode headNode, string charset)
        {
            var data = new Metadata();
            data.Charset = charset;
            data.Title = GetTitle(headNode);
            data.Description = GetDescription(headNode);
            data.Url = GetUrl(headNode);

            data.Meta = GetMeta(headNode);

            return data;
        }

        private Dictionary<string, string> GetMeta(HtmlNode headNode)
        {
            var metaDataList = new Dictionary<string, string>();

            var nodes = headNode.SelectNodes($"//meta");
            foreach (var node in nodes)
            {
                string name = node.GetAttributeValue("name", string.Empty);
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = node.GetAttributeValue("property", string.Empty);
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }
                }

                if (metaDataList.ContainsKey(name)) continue; // Don't deal with multiple values

                string value = node.GetAttributeValue("content", string.Empty);                
                metaDataList.Add(name, value);
            }

            return metaDataList;
        }

        private string GetUrl(HtmlNode headNode)
        {
            var node = headNode.SelectSingleNode($"//link[@rel='canonical']");
            if (node != null)
            {
                string link = node.Attributes.FirstOrDefault(a => a.Name == "href")?.Value;
                if (!string.IsNullOrWhiteSpace(link)) return link;
            }

            return GetProperty(headNode, "meta", "property", "og:url");
        }

        private string GetDescription(HtmlNode headNode)
        {
            string description = GetProperty(headNode, "meta", "name", "description");
            if (!string.IsNullOrWhiteSpace(description)) return description;
            
            description = GetProperty(headNode, "meta", "property", "og:description");            

            return description;

        }

        private string GetCharset(HtmlNode htmlNode)
        {
            string charset = GetAttribute(htmlNode, "//meta/@charset");
            return charset;
        }

        private string GetTitle(HtmlNode headNode)
        {
            string title = GetProperty(headNode, "meta", "property", "og:site_name");
            if (!string.IsNullOrWhiteSpace(title)) return title;
            
            title = GetProperty(headNode, "title", null, null);
            if (!string.IsNullOrWhiteSpace(title)) return title;

            title = GetProperty(headNode, "meta", "property", "og:title");
            
            return title;
        }

        private string GetAttribute(HtmlNode headNode, string xPath)
        {            
            var node = headNode.SelectSingleNode($"{xPath}");
            return node?.Attributes?.FirstOrDefault()?.Value;
        }

        private string GetProperty(HtmlNode headNode, string type, string attribute, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                var node = headNode.SelectSingleNode($"//{type}");
                if (node != null) return node.InnerText;
            }
            else
            {
                var node = headNode.SelectSingleNode($"//{type}[translate(@{attribute}, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='{name.ToLower()}']");
                if (node != null)
                {
                    return node.Attributes.FirstOrDefault(a => a.Name == "content").Value;
                }
            }
            return null;
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

        public string CleanUrl(string url)
        {
            string cleanUrl = url;

            if (!cleanUrl.Contains("//"))
            {
                cleanUrl = $"https://{url}";
            }

            return cleanUrl.ToLowerInvariant();
        }

        public bool ValidateUrl(string url)
        {
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
