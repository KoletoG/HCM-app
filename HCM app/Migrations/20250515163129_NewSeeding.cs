using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HCM_app.Migrations
{
    /// <inheritdoc />
    public partial class NewSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "5");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Department", "Email", "FirstName", "JobTitle", "LastName", "PasswordHash", "Role", "Salary" },
                values: new object[,]
                {
                    { "2001497d-2995-4a1a-a3c7-c979d8393764", "IT", "terry@email.com", "Terry", "Software Engineer", "Davis", "$2a$11$IE2aVv2PcArNUwv2y1BlkehB4STorUQn5NxsXeRLm2qXSCMwKTdDG", "Employee", 239000.0 },
                    { "31815e61-2406-4703-9b34-bf9f06f0cbab", "IT", "bob@email.com", "Bob", "Analysis", "Bobson", "$2a$11$eVJ4rdurEyezkeBMLmqFIOPy5WsWqEagza5p.1izG9vTIgBJVJ7Ze", "Employee", 23000.0 },
                    { "5e799534-9d9b-4946-83e3-e10728756b44", "Human Resources", "admin@email.com", "George", "HR Admin", "Johnson", "$2a$11$WZocNhN.MJxzlj3mz/BwPOFbHLEUQiIz4bEPj.OsdwJBVfVFtiTVy", "HrAdmin", 999999.0 },
                    { "5fdb35d2-c603-4b07-a783-5ec05236f24e", "Human Resources", "chris@email.com", "Chris", "HR", "Hemsworth", "$2a$11$H3xdw69rCOAfbcvCFoQbfOEUETlGefP/LlQH/AvfEgPu4vbeueEAC", "Employee", 200000.0 },
                    { "969320d6-9770-4563-a40f-8274c70dfca8", "IT", "scott@email.com", "Scott", "Software Engineer", "Marry", "$2a$11$P3.TYLdNnt5xzZrW/2YuB.FhZMXs.fC5ELJqb1RD7aBG2rrF6piTy", "Employee", 90000.0 },
                    { "a27155e7-6576-4533-82c2-aa57c9599bfd", "Human Resources", "lenny@email.com", "Lenny", "HR", "Arkan", "$2a$11$irHeELEwpnvOuM61ZECN..lP.jVDvasyWnT3zEJi.7zHyQU1DDsvS", "Employee", 50000.0 },
                    { "f2a46a35-e51f-499f-b169-ea21aa78bb20", "Human Resources", "manager@email.com", "Manny", "HR Manager", "Wonder", "$2a$11$ewWndEf7eJjhzK22PfjZs.HvSiu2KWz492yicZxigElpSoT0tP7p2", "Manager", 922222.0 },
                    { "f9bb6005-13be-441f-ac07-51d1c30bf753", "HR", "nelvin@email.com", "Nelvin", "Human Resources", "Lorance", "$2a$11$WZocNhN.MJxzlj3mz/BwPOFbHLEUQiIz4bEPj.OsdwJBVfVFtiTVy", "Employee", 70000.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "2001497d-2995-4a1a-a3c7-c979d8393764");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "31815e61-2406-4703-9b34-bf9f06f0cbab");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "5e799534-9d9b-4946-83e3-e10728756b44");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "5fdb35d2-c603-4b07-a783-5ec05236f24e");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "969320d6-9770-4563-a40f-8274c70dfca8");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "a27155e7-6576-4533-82c2-aa57c9599bfd");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "f2a46a35-e51f-499f-b169-ea21aa78bb20");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "f9bb6005-13be-441f-ac07-51d1c30bf753");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Department", "Email", "FirstName", "JobTitle", "LastName", "PasswordHash", "Role", "Salary" },
                values: new object[,]
                {
                    { "1", "Human Resources", "john.doe@company.com", "John", "HR Manager", "Doe", "john123", "HrAdmin", 60000.0 },
                    { "2", "IT", "alice.smith@company.com", "Alice", "Software Developer", "Smith", "$2a$11$6gHDwIT5oF7kPv/WsZ5xSuJZgYiy0qqosBHT5hyvTjxESfCLOEBoK", "Employee", 55000.0 },
                    { "3", "IT", "bob.brown@company.com", "Bob", "IT Manager", "Brown", "$2a$11$WkN0zZ6xNmBxLKNZKb7Lz.CDuvOl/aI6uR5Y31ECR6sIzLrz0xuU2", "Employee", 70000.0 },
                    { "4", "Analytics", "eve.white@company.com", "Eve", "Data Analyst", "White", "$2a$11$uFxI9zGB3EPPylxJzDJ01O9mxvhvn4tNzXn2uFub02YjKl3K5q6a6", "Manager", 50000.0 },
                    { "5", "Finance", "charlie.black@company.com", "Charlie", "Finance Manager", "Black", "$2a$11$ZOfVLQjZas3zF7TFoTeZp.7V/qew5cqvPcNCQs5tQZ0Txlri3F5FS", "Manager", 65000.0 }
                });
        }
    }
}
