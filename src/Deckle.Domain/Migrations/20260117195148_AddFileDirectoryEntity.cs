using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deckle.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFileDirectoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DirectoryId",
                table: "Files",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FileDirectories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentDirectoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDirectories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileDirectories_FileDirectories_ParentDirectoryId",
                        column: x => x.ParentDirectoryId,
                        principalTable: "FileDirectories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileDirectories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_DirectoryId",
                table: "Files",
                column: "DirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDirectories_ParentDirectoryId",
                table: "FileDirectories",
                column: "ParentDirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDirectories_ProjectId",
                table: "FileDirectories",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDirectories_ProjectId_ParentDirectoryId_Name",
                table: "FileDirectories",
                columns: new[] { "ProjectId", "ParentDirectoryId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_FileDirectories_DirectoryId",
                table: "Files",
                column: "DirectoryId",
                principalTable: "FileDirectories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_FileDirectories_DirectoryId",
                table: "Files");

            migrationBuilder.DropTable(
                name: "FileDirectories");

            migrationBuilder.DropIndex(
                name: "IX_Files_DirectoryId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "DirectoryId",
                table: "Files");
        }
    }
}
