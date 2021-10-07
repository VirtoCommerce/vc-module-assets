using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace VirtoCommerce.AssetsModule.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetEntry",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    RelativeUrl = table.Column<string>(maxLength: 2083, nullable: false),
                    TenantId = table.Column<string>(maxLength: 128, nullable: true),
                    TenantType = table.Column<string>(maxLength: 256, nullable: true),
                    Name = table.Column<string>(maxLength: 1024, nullable: false),
                    MimeType = table.Column<string>(maxLength: 128, nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 10, nullable: true),
                    Size = table.Column<long>(nullable: false),
                    Group = table.Column<string>(maxLength: 64, nullable: true)
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
