using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Collections.Generic;
using System;
using Moq.Protected;
using AuthAPIHCM.Controllers;
using AuthAPIHCM.Data;
using AuthAPIHCM.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthAPIHCM.Tests
{
    public class AuthControllerTests
    {
        private AuthController CreateController(HttpResponseMessage response, out Mock<IAuthService> mockAuthService)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://localhost:7261/")
            };

            var logger = new Mock<ILogger<AuthController>>();
            mockAuthService = new Mock<IAuthService>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);

            var controller = new AuthController(dbContext, mockAuthService.Object, logger.Object);
            typeof(AuthController).GetField("_clientCRUD", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                  ?.SetValue(controller, httpClient);

            return controller;
        }

        /// <summary>
        /// Testing For Login method
        /// </summary>
        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            var user = new UserDataModel
            {
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json")
            };

            var controller = CreateController(response, out var mockAuthService);
            mockAuthService.Setup(s => s.GenerateJwtToken(It.IsAny<UserDataModel>()))
                           .Returns("fake-token");

            var loginModel = new LoginViewModel { Email = user.Email, Password = "password123" };

            var result = await controller.Login(loginModel);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("fake-token", okResult.Value);
        }

        /// <summary>
        /// Testing For Login method
        /// </summary>
        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var user = new UserDataModel
            {
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password")
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json")
            };

            var controller = CreateController(response, out _);

            var loginModel = new LoginViewModel { Email = user.Email, Password = "wrong-password" };

            var result = await controller.Login(loginModel);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials", unauthorized.Value);
        }

        /// <summary>
        /// Testing For Login method
        /// </summary>
        [Fact]
        public async Task Login_UserNotFound_ReturnsNotFound()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            };

            var controller = CreateController(response, out _);

            var result = await controller.Login(new LoginViewModel
            {
                Email = "unknown@example.com",
                Password = "password"
            });

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("A user with this email doesn't exist.", notFound.Value);
        }

        /// <summary>
        /// Testing For Register method
        /// </summary>
        [Fact]
        public async Task Register_ValidUser_ReturnsNoContent()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            };

            var controller = CreateController(response, out _);

            var result = await controller.Register(new RegisterViewModel
            {
                Email = "employee@example.com",
                Password = "pass123",
                FirstName = "Jane",
                LastName = "Doe",
                Department = "IT",
                Salary = 30000,
                JobTitle = "Analyst"
            });

            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Testing For Register method
        /// </summary>
        [Fact]
        public async Task Register_InvalidModel_ReturnsBadRequest()
        {
            var controller = CreateController(new HttpResponseMessage(), out _);
            controller.ModelState.AddModelError("Email", "Required");

            var result = await controller.Register(new RegisterViewModel());

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<RegisterViewModel>(badRequest.Value);
        }
    }
}

