using Microsoft.EntityFrameworkCore.Migrations;

namespace CrowdedPlace.EfCli.Migrations
{
    public partial class UpdateConstraintInPosterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Posters",
                table: "Posters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posters",
                table: "Posters",
                columns: new[] { "DeviceId", "CreatedDate", "DemonstrationId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Posters",
                table: "Posters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posters",
                table: "Posters",
                columns: new[] { "DeviceId", "CreatedDate" });
        }
    }
}
