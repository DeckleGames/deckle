using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deckle.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePathColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Files",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            // Data migration: Populate Path for existing files
            // Root files: Path = FileName
            migrationBuilder.Sql("""
                UPDATE "Files" SET "Path" = "FileName" WHERE "DirectoryId" IS NULL;
                """);

            // Nested files: Build path from directory hierarchy
            migrationBuilder.Sql("""
                WITH RECURSIVE dir_paths AS (
                    SELECT d."Id", d."Name"::text as "Path"
                    FROM "FileDirectories" d WHERE d."ParentDirectoryId" IS NULL
                    UNION ALL
                    SELECT d."Id", dp."Path" || '/' || d."Name"
                    FROM "FileDirectories" d
                    INNER JOIN dir_paths dp ON d."ParentDirectoryId" = dp."Id"
                )
                UPDATE "Files" f SET "Path" = dp."Path" || '/' || f."FileName"
                FROM dir_paths dp WHERE f."DirectoryId" = dp."Id";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Files_ProjectId_Path",
                table: "Files",
                columns: new[] { "ProjectId", "Path" },
                unique: true,
                filter: "\"Status\" = 'Confirmed'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_ProjectId_Path",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Files");
        }
    }
}
