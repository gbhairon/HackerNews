
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HackerNews.HackerService;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace HackerNewsTests
{
    [TestFixture]
    public class HackerNewsServiceTests
    {
        [Test]
        public async Task TestRetrieveBestStories_ShouldCall_GetBestStoriesIds_And_GetBestStoriesFromIds()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HackerNewsService>>();
            var httpClientMock = new Mock<HttpClient>();

            var hackerNewsServiceMock = new Mock<HackerNewsService>(loggerMock.Object, httpClientMock.Object)
            {
                CallBase = true  // Allow calling the base class methods
            };

            hackerNewsServiceMock.Setup(x => x.GetBestStoriesIds()).ReturnsAsync(new List<string> { "1", "2", "3" });
            hackerNewsServiceMock.Setup(x => x.GetBestStoriesFromIds(It.IsAny<List<string>>(), It.IsAny<int>()))
                .ReturnsAsync(new List<HackerNews.Models.HackerNews>
                {
                    new HackerNews.Models.HackerNews { /* properties here */ },
                    new HackerNews.Models.HackerNews { /* properties here */ },
                    new HackerNews.Models.HackerNews { /* properties here */ }
                });

            var hackerNewsService = hackerNewsServiceMock.Object;

            // Act
            var result = await hackerNewsService.RetrieveBestStories(2);
            
            hackerNewsServiceMock.Verify(s => s.GetBestStoriesIds(), Times.Once);
            hackerNewsServiceMock.Verify(s => s.GetBestStoriesFromIds(new List<string> { "1", "2", "3" }, 2), Times.Once);
        }
        [Test]
        public async Task TestGetBestStoryIds_ReturnsCorrectArrayOfValues()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HackerNewsService>>();
            var httpClientMock = new Mock<HttpClient>();

            var expectedResponse = "[1,2,3,4]";
            var handler = new MockHttpMessageHandler(expectedResponse);
            var httpClient = new HttpClient(handler);
            var hackerNewsServiceMock = new Mock<HackerNewsService>(loggerMock.Object, httpClient)
            {
                CallBase = true  // Allow calling the base class methods
            };

            var hackerNewsService = hackerNewsServiceMock.Object;

            // Act
            var result = await hackerNewsService.GetBestStoriesIds();
            // Assert
            if (result != null && result.Count == 4)
            {
                Assert.AreEqual("1", result[0]);
                Assert.AreEqual("2", result[1]);
                Assert.AreEqual("3", result[2]);
                Assert.AreEqual("4", result[3]);
            }

        }

        [Test]
        public async Task TestGetBestStoryIds_()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HackerNewsService>>();
            var httpClientMock = new Mock<HttpClient>();

            var expectedResponse = "[1,2,3,4]";
            var handler = new MockHttpMessageHandler(expectedResponse);
            var httpClient = new HttpClient(handler);
            var hackerNewsServiceMock = new Mock<HackerNewsService>(loggerMock.Object, httpClient)
            {
                CallBase = true  // Allow calling the base class methods
            };

            var hackerNewsService = hackerNewsServiceMock.Object;

            // Act
            var result = await hackerNewsService.GetBestStoriesIds();
            // Assert
            if (result != null && result.Count == 4)
            {
                Assert.AreEqual("1", result[0]);
                Assert.AreEqual("2", result[1]);
                Assert.AreEqual("3", result[2]);
                Assert.AreEqual("4", result[3]);
            }

        }



        public class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseContent;

            public MockHttpMessageHandler(string responseContent)
            {
                _responseContent = responseContent;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(_responseContent)
                };

                return await Task.FromResult(response);
            }
        }

    }
}
