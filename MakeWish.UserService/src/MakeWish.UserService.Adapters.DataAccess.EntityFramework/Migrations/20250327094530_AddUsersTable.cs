using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "varchar", maxLength: 64, nullable: false),
                    password_hash = table.Column<string>(type: "varchar", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "varchar", maxLength: 64, nullable: false),
                    surname = table.Column<string>(type: "varchar", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_email",
                table: "Users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
