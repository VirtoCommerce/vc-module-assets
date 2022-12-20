using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace VirtoCommerce.AssetsModule.Data.SqlServer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AssetEntry'))
                    BEGIN
                        CREATE TABLE [AssetEntry](
	                        [Id] [nvarchar](128) NOT NULL,
	                        [CreatedDate] [datetime2](7) NOT NULL,
	                        [ModifiedDate] [datetime2](7) NULL,
	                        [CreatedBy] [nvarchar](64) NULL,
	                        [ModifiedBy] [nvarchar](64) NULL,
	                        [RelativeUrl] [nvarchar](2083) NOT NULL,
	                        [TenantId] [nvarchar](128) NULL,
	                        [TenantType] [nvarchar](256) NULL,
	                        [Name] [nvarchar](1024) NOT NULL,
	                        [MimeType] [nvarchar](128) NULL,
	                        [LanguageCode] [nvarchar](10) NULL,
	                        [Size] [bigint] NOT NULL,
	                        [Group] [nvarchar](64) NULL,
                         CONSTRAINT [PK_AssetEntry] PRIMARY KEY CLUSTERED 
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ) ;
                        
                        CREATE NONCLUSTERED INDEX [IX_AssetEntry_RelativeUrl_Name] ON [AssetEntry]
                        (
	                        [RelativeUrl] ASC,
	                        [Name] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetEntry");
        }
    }
}
