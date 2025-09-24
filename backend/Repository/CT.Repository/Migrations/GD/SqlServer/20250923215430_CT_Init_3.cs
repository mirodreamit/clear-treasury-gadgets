using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CT.Repository.Migrations.GD.SqlServer
{
    /// <inheritdoc />
    public partial class CT_Init_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                schema: "GD",
                table: "Gadget");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "GD",
                table: "Gadget",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
