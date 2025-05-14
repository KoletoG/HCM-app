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
                    Id = "f9bb6005-13be-441f-ac07-51d1c30bf753",
                    FirstName = "Nelvin",
                    PasswordHash = "$2a$11$WZocNhN.MJxzlj3mz/BwPOFbHLEUQiIz4bEPj.OsdwJBVfVFtiTVy",
                    Password = "123123",
                    Email = "nelvin@email.com",
                    LastName = "Lorance",
                    JobTitle = "Human Resources",
                    Department = "HR",
                    Role = "Employee",
                    Salary = 70000
                },
                new UserDataModel
                {
                    Id = "5e799534-9d9b-4946-83e3-e10728756b44",
                    FirstName = "George",
                    PasswordHash = "$2a$11$WZocNhN.MJxzlj3mz/BwPOFbHLEUQiIz4bEPj.OsdwJBVfVFtiTVy",
                    Password = "123123",
                    Email = "admin@email.com",
                    LastName = "Johnson",
                    JobTitle = "HR Admin",
                    Department = "Human Resources",
                    Role = "HrAdmin",
                    Salary = 999999
                },
                new UserDataModel
                {
                    Id = "5fdb35d2-c603-4b07-a783-5ec05236f24e",
                    FirstName = "Chris",
                    PasswordHash = "$2a$11$H3xdw69rCOAfbcvCFoQbfOEUETlGefP/LlQH/AvfEgPu4vbeueEAC",
                    Password = "123123",
                    Email = "chris@email.com",
                    LastName = "Hemsworth",
                    JobTitle = "HR",
                    Department = "Human Resources",
                    Role = "Employee",
                    Salary = 200000
                },
                new UserDataModel
                {
                    Id = "969320d6-9770-4563-a40f-8274c70dfca8",
                    FirstName = "Scott",
                    PasswordHash = "$2a$11$P3.TYLdNnt5xzZrW/2YuB.FhZMXs.fC5ELJqb1RD7aBG2rrF6piTy",
                    Password = "123123",
                    Email = "scott@email.com",
                    LastName = "Marry",
                    JobTitle = "Software Engineer",
                    Department = "IT",
                    Role = "Employee",
                    Salary = 90000
                },
                new UserDataModel
                {
                    Id = "a27155e7-6576-4533-82c2-aa57c9599bfd",
                    FirstName = "Lenny",
                    PasswordHash = "$2a$11$irHeELEwpnvOuM61ZECN..lP.jVDvasyWnT3zEJi.7zHyQU1DDsvS",
                    Password = "123123",
                    Email = "lenny@email.com",
                    LastName = "Arkan",
                    JobTitle = "HR",
                    Department = "Human Resources",
                    Role = "Employee",
                    Salary = 50000
                },
                new UserDataModel
                {
                    Id = "f2a46a35-e51f-499f-b169-ea21aa78bb20",
                    FirstName = "Manny",
                    PasswordHash = "$2a$11$ewWndEf7eJjhzK22PfjZs.HvSiu2KWz492yicZxigElpSoT0tP7p2",
                    Password = "123123",
                    Email = "manager@email.com",
                    LastName = "Wonder",
                    JobTitle = "HR Manager",
                    Department = "Human Resources",
                    Role = "Manager",
                    Salary = 922222
                },
                new UserDataModel
                {
                    Id = "31815e61-2406-4703-9b34-bf9f06f0cbab",
                    FirstName = "Bob",
                    PasswordHash = "$2a$11$eVJ4rdurEyezkeBMLmqFIOPy5WsWqEagza5p.1izG9vTIgBJVJ7Ze",
                    Password = "123123",
                    Email = "bob@email.com",
                    LastName = "Bobson",
                    JobTitle = "Analysis",
                    Department = "IT",
                    Role = "Employee",
                    Salary = 23000
                },
                new UserDataModel
                {
                    Id = "2001497d-2995-4a1a-a3c7-c979d8393764",
                    FirstName = "Terry",
                    PasswordHash = "$2a$11$IE2aVv2PcArNUwv2y1BlkehB4STorUQn5NxsXeRLm2qXSCMwKTdDG",
                    Password = "123123",
                    Email = "terry@email.com",
                    LastName = "Davis",
                    JobTitle = "Software Engineer",
                    Department = "IT",
                    Role = "Employee",
                    Salary = 239000
                }
            );
        }
    }
}
