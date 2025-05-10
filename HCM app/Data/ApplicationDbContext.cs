using System;
using Microsoft.EntityFrameworkCore;
using SharedModels;
namespace HCM_app.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserDataModel>().HasData(
                  new UserDataModel
                  {
                      Id = Guid.NewGuid().ToString(),
                      FirstName = "John",
                      LastName = "Doe",
                      Email = "john.doe@company.com",
                      JobTitle = "HR Manager",
                      Salary = 60000,
                      Department = "Human Resources",
                      Password = "john123",
                      PasswordHash = BCrypt.Net.BCrypt.HashPassword("john123"),
                      Role = UserRole.HrAdmin
                  },
                new UserDataModel
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice.smith@company.com",
                    JobTitle = "Software Developer",
                    Salary = 55000,
                    Department = "IT",
                    Password = "alice123",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("alice123"),
                    Role = UserRole.Employee
                },
                new UserDataModel
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Bob",
                    LastName = "Brown",
                    Email = "bob.brown@company.com",
                    JobTitle = "IT Manager",
                    Salary = 70000,
                    Department = "IT",
                    Password = "bob123",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("bob123"),
                    Role = UserRole.Manager
                },
                new UserDataModel
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Eve",
                    LastName = "White",
                    Email = "eve.white@company.com",
                    JobTitle = "Data Analyst",
                    Salary = 50000,
                    Department = "Analytics",
                    Password = "eve123",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("eve123"),
                    Role = UserRole.Employee
                },
                new UserDataModel
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Charlie",
                    LastName = "Black",
                    Email = "charlie.black@company.com",
                    JobTitle = "Finance Manager",
                    Salary = 65000,
                    Department = "Finance",
                    Password = "charlie123",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("charlie123"),
                    Role = UserRole.Manager
                }
                );
        }
        }
}
