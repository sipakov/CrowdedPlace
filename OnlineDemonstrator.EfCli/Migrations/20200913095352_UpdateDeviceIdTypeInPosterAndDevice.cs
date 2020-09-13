using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineDemonstrator.EfCli.Migrations
{
    public partial class UpdateDeviceIdTypeInPosterAndDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posters_Devices_DeviceId",
                table: "Posters");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "Posters",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Devices",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceId",
                table: "Posters",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Devices",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_Posters_Devices_DeviceId",
                table: "Posters",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
