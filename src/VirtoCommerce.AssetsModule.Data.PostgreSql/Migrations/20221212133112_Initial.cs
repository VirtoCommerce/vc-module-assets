using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.AssetsModule.Data.PostgreSql.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetEntry",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    RelativeUrl = table.Column<string>(type: "character varying(2083)", maxLength: 2083, nullable: false),
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TenantType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Group = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetEntry", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetEntry_RelativeUrl_Name",
                table: "AssetEntry",
                columns: new[] { "RelativeUrl", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetEntry");
        }
    }
}
