using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogDB.CatalogDBContextMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Elements",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Application = table.Column<string>(type: "text", nullable: true),
                    ApplicationVersion = table.Column<string>(type: "text", nullable: true),
                    BornOn = table.Column<string>(type: "text", nullable: true),
                    ContentStreamed = table.Column<bool>(type: "boolean", nullable: true),
                    DistinguishedName = table.Column<string>(type: "text", nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ObjectType = table.Column<string>(type: "text", nullable: true),
                    ServiceVersion = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elements", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Commands",
                columns: table => new
                {
                    PrimaryKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Application = table.Column<string>(type: "text", nullable: true),
                    ApplicationVersion = table.Column<string>(type: "text", nullable: true),
                    BornOn = table.Column<string>(type: "text", nullable: true),
                    CommandHandle = table.Column<string>(type: "text", nullable: true),
                    CommandSpecifications = table.Column<string>(type: "text", nullable: true),
                    CommandType = table.Column<string>(type: "text", nullable: true),
                    ContentHash = table.Column<string>(type: "text", nullable: true),
                    ContentStreamed = table.Column<bool>(type: "boolean", nullable: true),
                    DistinguishedName = table.Column<string>(type: "text", nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    ID = table.Column<Guid>(type: "uuid", nullable: true),
                    Modified = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ObjectType = table.Column<string>(type: "text", nullable: true),
                    ServiceVersion = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Commands_Elements_ID",
                        column: x => x.ID,
                        principalTable: "Elements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaLibrary",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Album = table.Column<string>(type: "text", nullable: true),
                    Application = table.Column<string>(type: "text", nullable: true),
                    Artist = table.Column<string>(type: "text", nullable: true),
                    BornOn = table.Column<string>(type: "text", nullable: true),
                    ContentStreamed = table.Column<bool>(type: "boolean", nullable: true),
                    DistinguishedName = table.Column<string>(type: "text", nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Year = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaLibrary", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MediaLibrary_Elements_ID",
                        column: x => x.ID,
                        principalTable: "Elements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    PrimaryKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ID = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Notes_Elements_ID",
                        column: x => x.ID,
                        principalTable: "Elements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    PrimaryKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Application = table.Column<string>(type: "text", nullable: true),
                    ApplicationVersion = table.Column<string>(type: "text", nullable: true),
                    BornOn = table.Column<string>(type: "text", nullable: true),
                    ContentHash = table.Column<string>(type: "text", nullable: true),
                    ContentStreamed = table.Column<bool>(type: "boolean", nullable: true),
                    DistinguishedName = table.Column<string>(type: "text", nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    ID = table.Column<Guid>(type: "uuid", nullable: true),
                    Modified = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ObjectType = table.Column<string>(type: "text", nullable: true),
                    ServiceInterfaces = table.Column<string>(type: "text", nullable: true),
                    ServiceType = table.Column<string>(type: "text", nullable: true),
                    ServiceVersion = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Services_Elements_ID",
                        column: x => x.ID,
                        principalTable: "Elements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    PrimaryKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ID = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Tags_Elements_ID",
                        column: x => x.ID,
                        principalTable: "Elements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commands_ID",
                table: "Commands",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_ID_Value",
                table: "Notes",
                columns: new[] { "ID", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_Value",
                table: "Notes",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ID",
                table: "Services",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ID_Value",
                table: "Tags",
                columns: new[] { "ID", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Value",
                table: "Tags",
                column: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commands");

            migrationBuilder.DropTable(
                name: "MediaLibrary");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Elements");
        }
    }
}
