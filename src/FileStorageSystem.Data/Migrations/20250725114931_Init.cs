using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileStorageSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "FileMetaData",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OriginalSize = table.Column<long>(type: "bigint", nullable: false),
                    OriginalChecksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UploadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileMetaDataStatus = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMetaData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChunkMetaData",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileMetaDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChunkIndex = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    StorageProviderType = table.Column<int>(type: "integer", nullable: false),
                    Checksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChunkMetaData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChunkMetaData_FileMetaData_FileMetaDataId",
                        column: x => x.FileMetaDataId,
                        principalSchema: "public",
                        principalTable: "FileMetaData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChunkMetaData_FileMetaDataId",
                schema: "public",
                table: "ChunkMetaData",
                column: "FileMetaDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChunkMetaData",
                schema: "public");

            migrationBuilder.DropTable(
                name: "FileMetaData",
                schema: "public");
        }
    }
}
