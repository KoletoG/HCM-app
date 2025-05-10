using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HCM_app.Migrations
{
    /// <inheritdoc />
    public partial class TryFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salary = table.Column<double>(type: "float", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
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
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
