using NSubstitute;
using System;
using System.IO;
using System.Threading.Tasks;
using WebSiteMeta.Scraper;
using WebSiteMeta.Scraper.HttpClientWrapper;
using Xunit;

namespace WebSiteMeta.Tests
{
    public class ScrapeSiteTest
    {
        FindMetaData _scraper;
        public ScrapeSiteTest()
        {
            string text = File.ReadAllText(@"SampleSites\simpletest.txt");

            var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
            httpClientWrapper.GetHttpData(Arg.Any<string>())
                .Returns((true, text));
            _scraper = new FindMetaData(httpClientWrapper);
        }

        [Theory]
        [InlineData("www.test.com")]
        [InlineData("Http://www.test.com")]
        [InlineData("https://www.test.com")]
        [InlineData("http://test.com")]
        [InlineData("http://www.test.co.uk")]
        [InlineData("http://test.com?test=123")]
        public async Task RunScrape_ValidUrl_Runs(string url)
        {
            // Arrange            

            // Act
            var result = await _scraper.Run(url);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Theory]
        [InlineData("javascript:alert('xss')")]
        [InlineData("blah")]
        [InlineData("htttp://www.test.com")]
        [InlineData("ftp://www.test.com")]        
        public async Task RunScrape_InvalidUrl_Fails(string url)
        {
            // Arrange            

            // Act
            var result = await _scraper.Run(url);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RunScrape_CallsUrl()
        {
            // Arrange                        

            // Act
            var result = await _scraper.Run("www.test.com");

            // Assert
            Assert.Equal("Test site name", result.Metadata.Title);
            Assert.Equal("Test site description", result.Metadata.Description);
            Assert.Equal("https://testsite.com", result.Metadata.Url);
        }

    }
}
