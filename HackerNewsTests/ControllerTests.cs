using NUnit.Framework;
using HackerNews.Controllers;
using Moq;
using HackerNews.HackerService;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsTests
{
    public class ControllerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task HackerNewsControllerAction_ShouldReturnBadRequest()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HackerNewsController>>();
            var httpClient = new HttpClient();
            var serviceMock = new Mock<IHackerNewsService>();

            HackerNewsController controller = new HackerNewsController(loggerMock.Object, serviceMock.Object, httpClient);

            // Act
            var result = await controller.GetAsync(-1);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            Assert.AreEqual(400, (result.Result as ObjectResult)?.StatusCode);
        }
        // create test to assert that the service gets called when the number passed in is positive
        [Test]
        public async Task HackerNewsControllerAction_ShouldCallMethodOnService()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HackerNewsController>>();
            var httpClient = new HttpClient();
            var serviceMock = new Mock<IHackerNewsService>();

            HackerNewsController controller = new HackerNewsController(loggerMock.Object, serviceMock.Object, httpClient);

            // Act
            var result = await controller.GetAsync(1);

            // Assert
            serviceMock.Verify(s => s.RetrieveBestStories(1), Times.Once);
        }
    }
}