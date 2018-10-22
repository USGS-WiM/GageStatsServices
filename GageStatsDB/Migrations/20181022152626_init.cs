using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GageStatsDB.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gagestats");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", "'postgis', '', ''");

            migrationBuilder.CreateTable(
                name: "Agency",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agency", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Citation",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Title = table.Column<string>(nullable: false),
                    Author = table.Column<string>(nullable: false),
                    CitationURL = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StationType",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Station",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Code = table.Column<string>(nullable: false),
                    AgencyID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    IsRegulated = table.Column<bool>(nullable: false),
                    StationTypeID = table.Column<int>(nullable: false),
                    Location = table.Column<Point>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Station", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Station_Agency_AgencyID",
                        column: x => x.AgencyID,
                        principalSchema: "gagestats",
                        principalTable: "Agency",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Station_StationType_StationTypeID",
                        column: x => x.StationTypeID,
                        principalSchema: "gagestats",
                        principalTable: "StationType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Statistic",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    StatisticGroupID = table.Column<int>(nullable: false),
                    RegressionTypeID = table.Column<int>(nullable: false),
                    StationID = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false),
                    UnitTypeID = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    YearsofRecord = table.Column<int>(nullable: false),
                    CitationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistic", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Statistic_Citation_StationID",
                        column: x => x.StationID,
                        principalSchema: "gagestats",
                        principalTable: "Citation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Statistic_Station_StationID",
                        column: x => x.StationID,
                        principalSchema: "gagestats",
                        principalTable: "Station",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatisticErrors",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UnitTypeID = table.Column<int>(nullable: false),
                    ErrorTypeID = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false),
                    StatisticID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticErrors", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StatisticErrors_Statistic_StatisticID",
                        column: x => x.StatisticID,
                        principalSchema: "gagestats",
                        principalTable: "Statistic",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatisticUnitTypes",
                schema: "gagestats",
                columns: table => new
                {
                    StatisticID = table.Column<int>(nullable: false),
                    UnitTypeID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticUnitTypes", x => new { x.StatisticID, x.UnitTypeID });
                    table.ForeignKey(
                        name: "FK_StatisticUnitTypes_Statistic_StatisticID",
                        column: x => x.StatisticID,
                        principalSchema: "gagestats",
                        principalTable: "Statistic",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agency_Code",
                schema: "gagestats",
                table: "Agency",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Station_AgencyID",
                schema: "gagestats",
                table: "Station",
                column: "AgencyID");

            migrationBuilder.CreateIndex(
                name: "IX_Station_Code",
                schema: "gagestats",
                table: "Station",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Station_StationTypeID",
                schema: "gagestats",
                table: "Station",
                column: "StationTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StationType_Code",
                schema: "gagestats",
                table: "StationType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Statistic_StationID",
                schema: "gagestats",
                table: "Statistic",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticErrors_StatisticID",
                schema: "gagestats",
                table: "StatisticErrors",
                column: "StatisticID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatisticErrors",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "StatisticUnitTypes",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "Statistic",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "Citation",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "Station",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "Agency",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "StationType",
                schema: "gagestats");
        }
    }
}
