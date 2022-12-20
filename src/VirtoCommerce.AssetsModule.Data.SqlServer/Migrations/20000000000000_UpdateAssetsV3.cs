using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.AssetsModule.Data.SqlServer.Migrations
{
    public partial class UpdateAssetsV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Skips 20210720132408_InitialAssets migration if table already exists
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AssetEntry'))
                    BEGIN
	                    INSERT INTO [__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20211006062748_Initial', '3.1.8')                 
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This method defined empty
        }
    }
}
