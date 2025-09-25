using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CT.Repository.Migrations.GD.SqlServer
{
    /// <inheritdoc />
    public partial class GD_LastModifiedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedByUserId",
                schema: "GD",
                table: "GadgetCategory",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedByUserId",
                schema: "GD",
                table: "Category",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_GadgetCategory_LastModifiedByUserId",
                schema: "GD",
                table: "GadgetCategory",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_LastModifiedByUserId",
                schema: "GD",
                table: "Category",
                column: "LastModifiedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_User_LastModifiedByUserId",
                schema: "GD",
                table: "Category",
                column: "LastModifiedByUserId",
                principalSchema: "GD",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GadgetCategory_User_LastModifiedByUserId",
                schema: "GD",
                table: "GadgetCategory",
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
                name: "FK_Category_User_LastModifiedByUserId",
                schema: "GD",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_GadgetCategory_User_LastModifiedByUserId",
                schema: "GD",
                table: "GadgetCategory");

            migrationBuilder.DropIndex(
                name: "IX_GadgetCategory_LastModifiedByUserId",
                schema: "GD",
                table: "GadgetCategory");

            migrationBuilder.DropIndex(
                name: "IX_Category_LastModifiedByUserId",
                schema: "GD",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                schema: "GD",
                table: "GadgetCategory");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                schema: "GD",
                table: "Category");
        }
    }
}
