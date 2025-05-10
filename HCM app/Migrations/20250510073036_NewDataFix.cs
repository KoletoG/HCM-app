using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HCM_app.Migrations
{
    /// <inheritdoc />
    public partial class NewDataFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserDataModel",
                keyColumn: "Id",
                keyValue: "3b15153b-9d74-48bd-951f-a9ccdb60be55");

            migrationBuilder.DeleteData(
                table: "UserDataModel",
                keyColumn: "Id",
                keyValue: "67766b45-df0b-47df-aa34-ccd42f41a622");

            migrationBuilder.DeleteData(
                table: "UserDataModel",
                keyColumn: "Id",
                keyValue: "95ba2835-f31f-4f1b-92a5-c2c994834555");

            migrationBuilder.DeleteData(
                table: "UserDataModel",
                keyColumn: "Id",
                keyValue: "aff067fa-1533-4bf0-9d66-9c2a5ec68e03");

            migrationBuilder.DeleteData(
                table: "UserDataModel",
                keyColumn: "Id",
                keyValue: "c371a2bd-2054-4d70-ae74-220ab964db23");

            migrationBuilder.InsertData(
                table: "UserDataModel",
                columns: new[] { "Id", "Department", "Email", "FirstName", "JobTitle", "LastName", "Password", "PasswordHash", "Role", "Salary" },
                values: new object[,]
                {
                    { "1", "Human Resources", "john.doe@company.com", "John", "HR Manager", "Doe", "john123", "$2a$11$TGF/8C6lvCd2NwvIzyy7MO5zEVU0HbFi6Lcszz3vJ5Jb7IYgU4WQ6", 2, 60000.0 },
                    { "2", "IT", "alice.smith@company.com", "Alice", "Software Developer", "Smith", "alice123", "$2a$11$6gHDwIT5oF7kPv/WsZ5xSuJZgYiy0qqosBHT5hyvTjxESfCLOEBoK", 0, 55000.0 },
                    { "3", "IT", "bob.brown@company.com", "Bob", "IT Manager", "Brown", "bob123", "$2a$11$WkN0zZ6xNmBxLKNZKb7Lz.CDuvOl/aI6uR5Y31ECR6sIzLrz0xuU2", 1, 70000.0 },
                    { "4", "Analytics", "eve.white@company.com", "Eve", "Data Analyst", "White", "eve123", "$2a$11$uFxI9zGB3EPPylxJzDJ01O9mxvhvn4tNzXn2uFub02YjKl3K5q6a6", 0, 50000.0 },
                    { "5", "Finance", "charlie.black@company.com", "Charlie", "Finance Manager", "Black", "charlie123", "$2a$11$ZOfVLQjZas3zF7TFoTeZp.7V/qew5cqvPcNCQs5tQZ0Txlri3F5FS", 1, 65000.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserDataModel",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "UserDataModel",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "UserDataModel",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "UserDataModel",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.DeleteData(
                table: "UserDataModel",
                keyColumn: "Id",
                keyValue: "5");

            migrationBuilder.InsertData(
                table: "UserDataModel",
                columns: new[] { "Id", "Department", "Email", "FirstName", "JobTitle", "LastName", "Password", "PasswordHash", "Role", "Salary" },
                values: new object[,]
                {
                    { "3b15153b-9d74-48bd-951f-a9ccdb60be55", "IT", "bob.brown@company.com", "Bob", "IT Manager", "Brown", "bob123", "$2a$11$0oeRcWuGYln89tEN0ZGRL.yTTSXHpsE893hkQdBC0alOcct8CIGlS", 1, 70000.0 },
                    { "67766b45-df0b-47df-aa34-ccd42f41a622", "IT", "alice.smith@company.com", "Alice", "Software Developer", "Smith", "alice123", "$2a$11$5IkuPwvTCoV1E0f07isFT.sc5ODdMHBEkZ3LAdkp9p8Ht9Ia5TbhO", 0, 55000.0 },
                    { "95ba2835-f31f-4f1b-92a5-c2c994834555", "Analytics", "eve.white@company.com", "Eve", "Data Analyst", "White", "eve123", "$2a$11$VvcM/raekBkc706DIzVKEOeiWwf3P7UzVtpyPjmoFd3PGZGNfNyga", 0, 50000.0 },
                    { "aff067fa-1533-4bf0-9d66-9c2a5ec68e03", "Finance", "charlie.black@company.com", "Charlie", "Finance Manager", "Black", "charlie123", "$2a$11$8JL/YIJPTM2Y5zu8dBQtY.0BaBm5fCVlXCVWf3atMHfHU2BACPy.i", 1, 65000.0 },
                    { "c371a2bd-2054-4d70-ae74-220ab964db23", "Human Resources", "john.doe@company.com", "John", "HR Manager", "Doe", "john123", "$2a$11$CAsvsc6Wb/zLl9kVw56PXuDHkqnuYL0DuGXg6bOfxq4bCVTSHwdZy", 2, 60000.0 }
                });
        }
    }
}
