using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OnlineDemonstrator.EfCli.Migrations
{
    public partial class initiatemigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Demonstrations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DemonstrationDate = table.Column<DateTime>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    CityName = table.Column<string>(maxLength: 100, nullable: true),
                    CountryName = table.Column<string>(maxLength: 100, nullable: true),
                    AreaName = table.Column<string>(maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demonstrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OsId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsLicenseActivated = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsBanned = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObjectionableReasons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reason = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectionableReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posters",
                columns: table => new
                {
                    DeviceId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: true),
                    Message = table.Column<string>(maxLength: 1000, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    DemonstrationId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posters", x => new { x.DeviceId, x.CreatedDate });
                    table.ForeignKey(
                        name: "FK_Posters_Demonstrations_DemonstrationId",
                        column: x => x.DemonstrationId,
                        principalTable: "Demonstrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posters_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectionableContents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<Guid>(nullable: false),
                    ObjectionableDeviceId = table.Column<Guid>(nullable: false),
                    ObjectionablePosterCreatedDate = table.Column<DateTime>(nullable: false),
                    ObjectionableReasonId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectionableContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectionableContents_ObjectionableReasons_ObjectionableRea~",
                        column: x => x.ObjectionableReasonId,
                        principalTable: "ObjectionableReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectionableContents_ObjectionableReasonId",
                table: "ObjectionableContents",
                column: "ObjectionableReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Posters_DemonstrationId",
                table: "Posters",
                column: "DemonstrationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectionableContents");

            migrationBuilder.DropTable(
                name: "Posters");

            migrationBuilder.DropTable(
                name: "ObjectionableReasons");

            migrationBuilder.DropTable(
                name: "Demonstrations");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
