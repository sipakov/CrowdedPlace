using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineDemonstrator.EfCli.Migrations
{
    public partial class AddedForeignKeyToPoster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Posters_Devices_DeviceId",
                table: "Posters",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posters_Devices_DeviceId",
                table: "Posters");
        }
    }
}
