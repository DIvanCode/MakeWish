using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendshipsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    first_user = table.Column<string>(type: "varchar", nullable: false),
                    second_user = table.Column<string>(type: "varchar", nullable: false),
                    is_confirmed = table.Column<bool>(type: "bool", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => new { x.first_user, x.second_user });
                    table.ForeignKey(
                        name: "FK_Friendships_Users_first_user",
                        column: x => x.first_user,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friendships_Users_second_user",
                        column: x => x.second_user,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_first_user",
                table: "Friendships",
                column: "first_user");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_second_user",
                table: "Friendships",
                column: "second_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friendships");
        }
    }
}
