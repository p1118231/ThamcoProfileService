using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThamcoProfiles.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PaymentAddress = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Password = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Auth0UserId = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
