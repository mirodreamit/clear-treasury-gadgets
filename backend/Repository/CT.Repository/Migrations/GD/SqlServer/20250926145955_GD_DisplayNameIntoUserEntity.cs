using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CT.Repository.Migrations.GD.SqlServer
{
    /// <inheritdoc />
    public partial class GD_DisplayNameIntoUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "GD",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "GD",
                table: "User");
        }
    }
}
