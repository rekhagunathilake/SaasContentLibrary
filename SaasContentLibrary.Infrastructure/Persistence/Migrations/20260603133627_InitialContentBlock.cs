using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaasContentLibrary.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialContentBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "content");

            migrationBuilder.CreateTable(
                name: "content_blocks",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlockType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Locale = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArchivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_blocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "content_versions",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    ContentBody = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    VersionStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AuthoredBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AuthoredOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ApprovedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContentBlockId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_versions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_content_versions_content_blocks_ContentBlockId",
                        column: x => x.ContentBlockId,
                        principalSchema: "content",
                        principalTable: "content_blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_content_blocks_TenantId",
                schema: "content",
                table: "content_blocks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_content_versions_ContentBlockId_VersionNumber",
                schema: "content",
                table: "content_versions",
                columns: new[] { "ContentBlockId", "VersionNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "content_versions",
                schema: "content");

            migrationBuilder.DropTable(
                name: "content_blocks",
                schema: "content");
        }
    }
}
