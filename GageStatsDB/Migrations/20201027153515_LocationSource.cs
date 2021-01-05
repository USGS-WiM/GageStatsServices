using Microsoft.EntityFrameworkCore.Migrations;

namespace GageStatsDB.Migrations
{
    public partial class LocationSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RegionID",
                schema: "gagestats",
                table: "Stations",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "LocationSource",
                schema: "gagestats",
                table: "Stations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationSource",
                schema: "gagestats",
                table: "Stations");

            migrationBuilder.AlterColumn<int>(
                name: "RegionID",
                schema: "gagestats",
                table: "Stations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
