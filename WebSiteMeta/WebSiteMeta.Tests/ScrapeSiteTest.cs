using System;
using WebSiteMeta.Scraper;
using Xunit;

namespace WebSiteMeta.Tests
{
    public class ScrapeSiteTest
    {
        [Theory]
        [InlineData("www.test.com")]
        [InlineData("Http://www.test.com")]
        [InlineData("https://www.test.com")]
        [InlineData("http://test.com")]
        [InlineData("http://www.test.co.uk")]
        [InlineData("http://test.com?test=123")]
        public void RunScrape_ValidUrl_Runs(string url)
        {
            // Arrange
            var scraper = new FindMetaData();

            // Act
            var result = scraper.Run("www.test.com");

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Theory]
        [InlineData("javascript:alert('xss')")]
        [InlineData("blah")]
        [InlineData("htttp://www.test.com")]
        [InlineData("ftp://www.test.com")]        
        public void RunScrape_InvalidUrl_Fails(string url)
        {
            // Arrange
            var scraper = new FindMetaData();

            // Act
            var result = scraper.Run(url);

            // Assert
            Assert.False(result.IsSuccess);
        }

    }
}
