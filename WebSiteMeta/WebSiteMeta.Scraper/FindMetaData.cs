﻿using HtmlAgilityPack;
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
    public class FindMetaData
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
       
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

            var pageSource = WebUtility.HtmlDecode(httpDataResult.data);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            var headNode = htmlDoc.DocumentNode.SelectSingleNode("//head");
            data.Title = GetTitle(headNode);
            data.Description = GetDescription(headNode);
            data.Url = GetUrl(headNode);

            return Success(data);
        }

        private string GetUrl(HtmlNode headNode)
        {
            var node = headNode.SelectSingleNode($"//link[@rel='canonical']");
            string link = node.Attributes.FirstOrDefault(a => a.Name == "href")?.Value;
            if (!string.IsNullOrWhiteSpace(node.InnerText)) return node.InnerText;

            return GetProperty(headNode, "meta", "property", "og:url");
        }

        private string GetDescription(HtmlNode headNode)
        {
            string description = GetProperty(headNode, "meta", "name", "description");
            if (!string.IsNullOrWhiteSpace(description)) return description;
            
            description = GetProperty(headNode, "meta", "property", "og:description");            

            return description;

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

        private string GetProperty(HtmlNode headNode, string type, string attribute, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                var node = headNode.SelectSingleNode($"//{type}");
                return node.InnerText;
            }
            else
            {
                var node = headNode.SelectSingleNode($"//{type}[@{attribute}='{name}']");
                return node.Attributes.FirstOrDefault(a => a.Name == "content").Value;
            }            
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
