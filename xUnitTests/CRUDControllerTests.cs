using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDHCM_API.Controllers;
using CRUDHCM_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SharedModels;

namespace xUnitTests
{
    public class CRUDControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            return new ApplicationDbContext(options);
        }

        private CRUDController GetController(ApplicationDbContext context)
        {
            var logger = new Mock<ILogger<CRUDController>>();
            return new CRUDController(context, logger.Object);
        }
        /// <summary>
        /// Tests AddUser
        /// </summary>
        [Fact]
        public async Task AddUser_ValidData_ReturnsCreatedResult()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var user = new UserDataModel
            {
                Id = "10",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Salary = 50000,
                JobTitle = "Developer",
                Department = "IT",
                Password = "securePass!",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("securePass!"),
                Role = "Employee"
            };

            var result = await controller.AddUser(user);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedUser = Assert.IsType<UserDataModel>(createdResult.Value);
            Assert.Equal("Test", returnedUser.FirstName);
        }

        /// <summary>
        /// Tests AddUser
        /// </summary>
        [Fact]
        public async Task AddUser_MissingRequiredField_ReturnsBadRequest()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var user = new UserDataModel
            {
                Id = "11",
                LastName = "User",
                Email = "incomplete@example.com"
                // FirstName is missing
            };

            var result = await controller.AddUser(user);

            // Since validation attributes aren't automatically enforced in unit tests,
            // you need to simulate model validation if needed.
            Assert.IsType<BadRequestObjectResult>(result);
        }
        /// <summary>
        /// Tests UpdatePassword
        /// </summary>
        [Fact]
        public async Task UpdatePassword_ChangesPasswordHash()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var user = new UserDataModel
            {
                Id = "20",
                FirstName = "Pass",
                LastName = "Changer",
                Email = "pass@change.com",
                Department="IT",
                JobTitle="Software Engineer",
                Role="Employee",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpass")
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var changeModel = new ChangePassViewModel("20", "newSecurePassword");

            var result = await controller.UpdatePassword(changeModel);
            Assert.IsType<NoContentResult>(result);

            var updatedUser = await context.Users.FindAsync("20");
            Assert.True(BCrypt.Net.BCrypt.Verify("newSecurePassword", updatedUser.PasswordHash));
        }


    }
}
