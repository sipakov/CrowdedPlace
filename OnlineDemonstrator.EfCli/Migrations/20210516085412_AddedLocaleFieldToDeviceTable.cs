using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineDemonstrator.EfCli.Migrations
{
    public partial class AddedLocaleFieldToDeviceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Locale",
                table: "Devices",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locale",
                table: "Devices");
        }
    }
}
