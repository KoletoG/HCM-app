using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCM_app.Migrations
{
    /// <inheritdoc />
    public partial class NewData3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDataModel",
                table: "UserDataModel");

            migrationBuilder.RenameTable(
                name: "UserDataModel",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "UserDataModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDataModel",
                table: "UserDataModel",
                column: "Id");
        }
    }
}
