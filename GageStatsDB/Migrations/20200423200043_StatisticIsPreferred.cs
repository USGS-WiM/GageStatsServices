using Microsoft.EntityFrameworkCore.Migrations;

namespace GageStatsDB.Migrations
{
    public partial class StatisticIsPreferred : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPreferred",
                schema: "gagestats",
                table: "Statistics",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreferred",
                schema: "gagestats",
                table: "Statistics");
        }
    }
}
