using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CT.Repository.Migrations.GD.SqlServer
{
    /// <inheritdoc />
    public partial class GD_Authentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedByUserId",
                schema: "GD",
                table: "Gadget",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "User",
                schema: "GD",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gadget_LastModifiedByUserId",
                schema: "GD",
                table: "Gadget",
                column: "LastModifiedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gadget_User_LastModifiedByUserId",
                schema: "GD",
                table: "Gadget",
                column: "LastModifiedByUserId",
                principalSchema: "GD",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gadget_User_LastModifiedByUserId",
                schema: "GD",
                table: "Gadget");

            migrationBuilder.DropTable(
                name: "User",
                schema: "GD");

            migrationBuilder.DropIndex(
                name: "IX_Gadget_LastModifiedByUserId",
                schema: "GD",
                table: "Gadget");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                schema: "GD",
                table: "Gadget");
        }
    }
}
