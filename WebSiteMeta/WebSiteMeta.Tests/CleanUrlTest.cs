using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using WebSiteMeta.Scraper;
using WebSiteMeta.Scraper.HttpClientWrapper;
using Xunit;

namespace WebSiteMeta.Tests
{
    public class CleanUrlTest
    {
        [Theory]
        [InlineData("www.test.com", "https://www.test.com")]
        [InlineData("Http://www.test.com", "http://www.test.com")]
        [InlineData("https://www.test.com", "https://www.test.com")]
        [InlineData("http://test.com", "http://test.com")]
        [InlineData("http://www.test.co.uk", "http://www.test.co.uk")]
        [InlineData("http://test.com?test=123", "http://test.com?test=123")]
        [InlineData(@"javascript:alert('xss')", @"https://javascript:alert('xss')")]
        [InlineData("blah", "https://blah")]
        [InlineData("htttp://www.test.com", "htttp://www.test.com")]
        [InlineData("ftp://www.test.com", "ftp://www.test.com")]
        [InlineData("www.it", "https://www.it")]
        public void CleanUrl_CleansCorrectly(string url, string expectedUrl)
        {
            // Arrange                        
            var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
            var scraper = new FindMetaData(httpClientWrapper);

            // Act
            var result = scraper.CleanUrl(url);

            // Assert
            Assert.Equal(expectedUrl, result);
        }
    }
}
