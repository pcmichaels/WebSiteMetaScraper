using NSubstitute;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebSiteMeta.Scraper;
using WebSiteMeta.Scraper.HttpClientWrapper;
using Xunit;

namespace WebSiteMeta.Tests
{
    public class ScrapeSiteTest
    {
        FindMetaData _scraper;

        private void SetupTest(string testFile)
        {
            string text = File.ReadAllText(testFile);

            var httpClientWrapper = Substitute.For<IHttpClientWrapper>();
            httpClientWrapper.GetHttpData(Arg.Any<string>(), Arg.Any<Encoding>())
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
            SetupTest(@"SampleSites/simpletest.txt");

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
            SetupTest(@"SampleSites/simpletest.txt");

            // Act
            var result = await _scraper.Run(url);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RunScrape_SimpleTest_ParseSite()
        {
            // Arrange
            SetupTest(@"SampleSites/simpletest.txt");

            // Act
            var result = await _scraper.Run("www.test.com");

            // Assert
            Assert.Equal("Test site name", result.Metadata.Title);
            Assert.Equal("Test site description", result.Metadata.Description);
            Assert.Equal("https://testsite.com", result.Metadata.Url);
        }

        [Fact]
        public async Task RunScrape_SimpleTestMixedCase_ParseSite()
        {
            // Arrange
            SetupTest(@"SampleSites/simpletestMixedCase.txt");

            // Act
            var result = await _scraper.Run("www.test.com");

            // Assert
            Assert.Equal("Test site name", result.Metadata.Title);
            Assert.Equal("Test site description", result.Metadata.Description);
            Assert.Equal("https://testsite.com", result.Metadata.Url);
        }

        [Fact]
        public async Task RunScrape_Wordpress_CallsUrl()
        {
            // Arrange
            SetupTest(@"SampleSites/pmichaels.net.txt");

            // Act
            var result = await _scraper.Run("www.test.com");

            // Assert
            Assert.Equal("The Long Walk", result.Metadata.Title);
            Assert.Equal("A blog about one man's journey through code... and some pictures of the Peak District", result.Metadata.Description);
            Assert.Equal("https://www.pmichaels.net/", result.Metadata.Url);
        }

        [Fact]
        public async Task RunScrape_Chinese_DetectCharset()
        {
            // Arrange
            SetupTest(@"SampleSites/qq.com.txt");

            // Act
            var result = await _scraper.Run("www.test.com");

            // Assert
            Assert.Equal("gb2312", result.Metadata.Charset);
            Assert.Equal("腾讯首页", result.Metadata.Title);
        }

        [Fact]
        public async Task RunScrape_Chinese_DetectCharset_Utf8()
        {
            // Arrange
            SetupTest(@"SampleSites/sohu.com.txt");

            // Act
            var result = await _scraper.Run("www.test.com");

            // Assert
            Assert.Equal("utf-8", result.Metadata.Charset);
            Assert.Equal("搜狐", result.Metadata.Title);
        }

        [Fact]
        public async Task RunScrape_SimpleTest_MetaValues()
        {
            // Arrange
            SetupTest(@"SampleSites/simpletest.txt");

            // Act
            var result = await _scraper.Run("www.test.com");

            // Assert
            Assert.Equal(9, result.Metadata.Meta.Count);
        }

        [Fact]
        public async Task RunScrape_YouTube()
        {
            // Arrange
            SetupTest(@"SampleSites/YouTube.txt");

            // Act
            var result = await _scraper.Run("www.test.com");

            // Assert
            Assert.Equal("YouTube", result.Metadata.Title);            
        }

    }
}
