using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CT.Repository.Migrations.GD.SqlServer
{
    /// <inheritdoc />
    public partial class GD_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "GD");

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "GD",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gadget",
                schema: "GD",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gadget", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GadgetCategory",
                schema: "GD",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GadgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ordinal = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GadgetCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GadgetCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "GD",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GadgetCategory_Gadget_GadgetId",
                        column: x => x.GadgetId,
                        principalSchema: "GD",
                        principalTable: "Gadget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                schema: "GD",
                table: "Category",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Include", new[] { "CreatedAt", "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Category_UpdatedAt",
                schema: "GD",
                table: "Category",
                column: "UpdatedAt")
                .Annotation("SqlServer:Include", new[] { "Name", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Gadget_Name",
                schema: "GD",
                table: "Gadget",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Include", new[] { "CreatedAt", "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Gadget_UpdatedAt",
                schema: "GD",
                table: "Gadget",
                column: "UpdatedAt")
                .Annotation("SqlServer:Include", new[] { "Name", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GadgetCategory_CategoryId",
                schema: "GD",
                table: "GadgetCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GadgetCategory_GadgetId_CategoryId",
                schema: "GD",
                table: "GadgetCategory",
                columns: new[] { "GadgetId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GadgetCategory_GadgetId_Ordinal",
                schema: "GD",
                table: "GadgetCategory",
                columns: new[] { "GadgetId", "Ordinal" })
                .Annotation("SqlServer:Include", new[] { "CreatedAt", "UpdatedAt", "CategoryId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GadgetCategory",
                schema: "GD");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "GD");

            migrationBuilder.DropTable(
                name: "Gadget",
                schema: "GD");
        }
    }
}
