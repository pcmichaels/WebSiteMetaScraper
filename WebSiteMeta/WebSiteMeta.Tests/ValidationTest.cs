using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using WebSiteMeta.Scraper;
using WebSiteMeta.Scraper.HttpClientWrapper;
using Xunit;

namespace WebSiteMeta.Tests
{
    public class ValidationTest
    {
        [Theory]
        [InlineData("www.test.com", false)]
        [InlineData("Http://www.test.com", false)]
        [InlineData("https://www.test.com", true)]
        [InlineData("http://test.com", true)]
        [InlineData("http://www.test.co.uk", true)]
        [InlineData("http://test.com?test=123", true)]
        [InlineData("javascript:alert('xss')", false)]
        [InlineData("blah", false)]
        [InlineData("htttp://www.test.com", false)]
        [InlineData("ftp://www.test.com", false)]
        [InlineData("www.it", false)]
        public void ValidateUrl_ValidatesCorrectly(string url, bool isValid)
        {
            // Arrange                        
            var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
            var scraper = new FindMetaData(httpClientWrapper);

            // Act
            var result = scraper.ValidateUrl(url);

            // Assert
            Assert.Equal(isValid, result);
        }
    }
}
