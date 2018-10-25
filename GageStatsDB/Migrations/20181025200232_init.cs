using System;
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
                name: "Agencies",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Citations",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Title = table.Column<string>(nullable: false),
                    Author = table.Column<string>(nullable: false),
                    CitationURL = table.Column<string>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StationTypes",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
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
                    Location = table.Column<Point>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Stations_Agencies_AgencyID",
                        column: x => x.AgencyID,
                        principalSchema: "gagestats",
                        principalTable: "Agencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stations_StationTypes_StationTypeID",
                        column: x => x.StationTypeID,
                        principalSchema: "gagestats",
                        principalTable: "StationTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
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
                    CitationID = table.Column<int>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Statistics_Citations_StationID",
                        column: x => x.StationID,
                        principalSchema: "gagestats",
                        principalTable: "Citations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Statistics_Stations_StationID",
                        column: x => x.StationID,
                        principalSchema: "gagestats",
                        principalTable: "Stations",
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
                        name: "FK_StatisticErrors_Statistics_StatisticID",
                        column: x => x.StatisticID,
                        principalSchema: "gagestats",
                        principalTable: "Statistics",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatisticUnitTypes",
                schema: "gagestats",
                columns: table => new
                {
                    StatisticID = table.Column<int>(nullable: false),
                    UnitTypeID = table.Column<string>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticUnitTypes", x => new { x.StatisticID, x.UnitTypeID });
                    table.ForeignKey(
                        name: "FK_StatisticUnitTypes_Statistics_StatisticID",
                        column: x => x.StatisticID,
                        principalSchema: "gagestats",
                        principalTable: "Statistics",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_Code",
                schema: "gagestats",
                table: "Agencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stations_AgencyID",
                schema: "gagestats",
                table: "Stations",
                column: "AgencyID");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_Code",
                schema: "gagestats",
                table: "Stations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stations_StationTypeID",
                schema: "gagestats",
                table: "Stations",
                column: "StationTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StationTypes_Code",
                schema: "gagestats",
                table: "StationTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatisticErrors_StatisticID",
                schema: "gagestats",
                table: "StatisticErrors",
                column: "StatisticID");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_StationID",
                schema: "gagestats",
                table: "Statistics",
                column: "StationID");

            //custom sql
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION ""gagestats"".""trigger_set_lastmodified""()
                    RETURNS TRIGGER AS $$
                    BEGIN
                      NEW.""LastModified"" = NOW();
                      RETURN NEW;
                    END;
                    $$ LANGUAGE plpgsql;
                ");
            migrationBuilder.Sql(@"
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON ""gagestats"".""Agencies""  FOR EACH ROW EXECUTE PROCEDURE ""gagestats"".""trigger_set_lastmodified""();
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON ""gagestats"".""Citations""  FOR EACH ROW EXECUTE PROCEDURE ""gagestats"".""trigger_set_lastmodified""();
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON ""gagestats"".""Stations"" FOR EACH ROW EXECUTE PROCEDURE  ""gagestats"".""trigger_set_lastmodified""();
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON ""gagestats"".""StationTypes""  FOR EACH ROW EXECUTE PROCEDURE ""gagestats"".""trigger_set_lastmodified""();
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON ""gagestats"".""Statistics"" FOR EACH ROW EXECUTE PROCEDURE  ""gagestats"".""trigger_set_lastmodified""();
                CREATE TRIGGER lastupdate BEFORE INSERT OR UPDATE ON ""gagestats"".""StatisticUnitTypes""  FOR EACH ROW EXECUTE PROCEDURE ""gagestats"".""trigger_set_lastmodified""();
                ");
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
                name: "Statistics",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "Citations",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "Stations",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "Agencies",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "StationTypes",
                schema: "gagestats");
        }
    }
}
