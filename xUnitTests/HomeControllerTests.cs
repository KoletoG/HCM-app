using FluentAssertions;
using Ganss.Xss;
using HCM_app.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace xUnitTests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<IHttpClientFactory> _mockFactory;
        private readonly Mock<IHtmlSanitizer> _mockSanitizer;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockFactory = new Mock<IHttpClientFactory>();
            _mockSanitizer = new Mock<IHtmlSanitizer>();
            _mockCache = new Mock<IMemoryCache>();
            var mockClient = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(mockClient.Object);
            _mockFactory.Setup(f => f.CreateClient("AuthAPI")).Returns(httpClient);
            _mockFactory.Setup(f => f.CreateClient("CRUDAPI")).Returns(httpClient);
            _mockSanitizer.Setup(s => s.AllowedTags).Returns(new HashSet<string>());
            _controller = new HomeController(
                _mockLogger.Object,
                _mockFactory.Object,
                _mockSanitizer.Object,
                _mockCache.Object);
        }

        [Fact]
        public void Index_Returns_View()
        {
            var result = _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewName.Should().BeNull();
        }
    }
}
