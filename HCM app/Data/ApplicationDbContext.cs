using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace HCM_app.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<UserDataModel> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDataModel>().HasData(
                new UserDataModel
                {
                    Id = "1", // Static ID
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@company.com",
                    JobTitle = "HR Manager",
                    Salary = 60000,
                    Department = "Human Resources",
                    Password = "john123",
                    PasswordHash = "$2a$11$TGF/8C6lvCd2NwvIzyy7MO5zEVU0HbFi6Lcszz3vJ5Jb7IYgU4WQ6", // Pre-hashed
                    Role = UserRole.HrAdmin
                },
                new UserDataModel
                {
                    Id = "2",
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice.smith@company.com",
                    JobTitle = "Software Developer",
                    Salary = 55000,
                    Department = "IT",
                    Password = "alice123",
                    PasswordHash = "$2a$11$6gHDwIT5oF7kPv/WsZ5xSuJZgYiy0qqosBHT5hyvTjxESfCLOEBoK",
                    Role = UserRole.Employee
                },
                new UserDataModel
                {
                    Id = "3",
                    FirstName = "Bob",
                    LastName = "Brown",
                    Email = "bob.brown@company.com",
                    JobTitle = "IT Manager",
                    Salary = 70000,
                    Department = "IT",
                    Password = "bob123",
                    PasswordHash = "$2a$11$WkN0zZ6xNmBxLKNZKb7Lz.CDuvOl/aI6uR5Y31ECR6sIzLrz0xuU2",
                    Role = UserRole.Manager
                },
                new UserDataModel
                {
                    Id = "4",
                    FirstName = "Eve",
                    LastName = "White",
                    Email = "eve.white@company.com",
                    JobTitle = "Data Analyst",
                    Salary = 50000,
                    Department = "Analytics",
                    Password = "eve123",
                    PasswordHash = "$2a$11$uFxI9zGB3EPPylxJzDJ01O9mxvhvn4tNzXn2uFub02YjKl3K5q6a6",
                    Role = UserRole.Employee
                },
                new UserDataModel
                {
                    Id = "5",
                    FirstName = "Charlie",
                    LastName = "Black",
                    Email = "charlie.black@company.com",
                    JobTitle = "Finance Manager",
                    Salary = 65000,
                    Department = "Finance",
                    Password = "charlie123",
                    PasswordHash = "$2a$11$ZOfVLQjZas3zF7TFoTeZp.7V/qew5cqvPcNCQs5tQZ0Txlri3F5FS",
                    Role = UserRole.Manager
                }
            );
        }
    }
}
