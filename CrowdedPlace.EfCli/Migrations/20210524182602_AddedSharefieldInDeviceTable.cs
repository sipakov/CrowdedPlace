using Microsoft.EntityFrameworkCore.Migrations;

namespace CrowdedPlace.EfCli.Migrations
{
    public partial class AddedSharefieldInDeviceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SharedCount",
                table: "Devices",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SharedCount",
                table: "Devices");
        }
    }
}
