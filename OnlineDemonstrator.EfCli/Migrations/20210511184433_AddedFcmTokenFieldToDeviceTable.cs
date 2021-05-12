using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineDemonstrator.EfCli.Migrations
{
    public partial class AddedFcmTokenFieldToDeviceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FcmToken",
                table: "Devices",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNotSendNotifications",
                table: "Devices",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FcmToken",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "IsNotSendNotifications",
                table: "Devices");
        }
    }
}
