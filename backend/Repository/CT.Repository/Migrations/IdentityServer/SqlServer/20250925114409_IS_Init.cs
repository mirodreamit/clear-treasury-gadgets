using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CT.Repository.Migrations.IdentityServer.SqlServer
{
    /// <inheritdoc />
    public partial class IS_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "IdentityServer");

            migrationBuilder.CreateTable(
                name: "User",
                schema: "IdentityServer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsSuperAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnonymousUser",
                schema: "IdentityServer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymousUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnonymousUser_User_Id",
                        column: x => x.Id,
                        principalSchema: "IdentityServer",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCredential",
                schema: "IdentityServer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredential", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCredential_User_Id",
                        column: x => x.Id,
                        principalSchema: "IdentityServer",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDetail",
                schema: "IdentityServer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDetail_User_Id",
                        column: x => x.Id,
                        principalSchema: "IdentityServer",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousUser_SessionId",
                schema: "IdentityServer",
                table: "AnonymousUser",
                column: "SessionId",
                unique: true)
                .Annotation("SqlServer:Include", new[] { "CreatedAt", "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_User_Identifier",
                schema: "IdentityServer",
                table: "User",
                column: "Identifier",
                unique: true)
                .Annotation("SqlServer:Include", new[] { "IsSuperAdmin", "CreatedAt", "UpdatedAt", "IsBlocked" });

            migrationBuilder.CreateIndex(
                name: "IX_UserDetail_Email",
                schema: "IdentityServer",
                table: "UserDetail",
                column: "Email",
                unique: true)
                .Annotation("SqlServer:Include", new[] { "DisplayName", "CreatedAt", "UpdatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymousUser",
                schema: "IdentityServer");

            migrationBuilder.DropTable(
                name: "UserCredential",
                schema: "IdentityServer");

            migrationBuilder.DropTable(
                name: "UserDetail",
                schema: "IdentityServer");

            migrationBuilder.DropTable(
                name: "User",
                schema: "IdentityServer");
        }
    }
}
