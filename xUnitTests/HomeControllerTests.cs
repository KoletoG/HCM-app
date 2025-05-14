using FluentAssertions;
using Ganss.Xss;
using HCM_app.Controllers;
using HCM_app.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SharedModels;

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
        [Fact]
        public void Login_GETReturns_ReturnsView()
        {
            var result = _controller.Login();
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public void Register_GET_ReturnsView()
        {
            var result = _controller.Register();
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public void Error_ReturnsErrorView()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            var result = _controller.Error();
            var view = result as ViewResult;
            view.Should().NotBeNull();
            view.Model.Should().BeOfType<ErrorViewModel>();
        }
        [Fact]
        public async Task Register_POST_InvalidModel_ReturnsViewWithModel()
        {
            _controller.ModelState.AddModelError("Email", "Required");
            var model = new RegisterViewModel();
            var result = await _controller.Register(model);
            var view = result as ViewResult;
            view.Should().NotBeNull();
            view.Model.Should().Be(model);
        }
        [Theory]
        [InlineData("user@example.com", "password123")]
        public async Task Login_POST_ValidCredentials_ReturnsRedirect(string email, string password)
        {
            var model = new LoginViewModel { Email = email, Password = password };
            var sessionMock = new Mock<ISession>();
            var context = new DefaultHttpContext { Session = sessionMock.Object };
            _controller.ControllerContext = new ControllerContext { HttpContext = context };
            var result = await _controller.Login(model);
            var redirect = result as RedirectToActionResult;
            redirect.Should().NotBeNull();
            redirect.ActionName.Should().Be("Index");
        }
    }
}
