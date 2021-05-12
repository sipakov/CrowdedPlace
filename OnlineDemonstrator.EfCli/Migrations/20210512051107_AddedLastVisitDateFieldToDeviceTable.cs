using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineDemonstrator.EfCli.Migrations
{
    public partial class AddedLastVisitDateFieldToDeviceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisitDate",
                table: "Devices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVisitDate",
                table: "Devices");
        }
    }
}
