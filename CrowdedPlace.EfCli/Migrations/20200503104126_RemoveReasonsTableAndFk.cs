using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CrowdedPlace.EfCli.Migrations
{
    public partial class RemoveReasonsTableAndFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectionableContents_ObjectionableReasons_ObjectionableRea~",
                table: "ObjectionableContents");

            migrationBuilder.DropTable(
                name: "ObjectionableReasons");

            migrationBuilder.DropIndex(
                name: "IX_ObjectionableContents_ObjectionableReasonId",
                table: "ObjectionableContents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectionableReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectionableReasons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectionableContents_ObjectionableReasonId",
                table: "ObjectionableContents",
                column: "ObjectionableReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectionableContents_ObjectionableReasons_ObjectionableRea~",
                table: "ObjectionableContents",
                column: "ObjectionableReasonId",
                principalTable: "ObjectionableReasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
