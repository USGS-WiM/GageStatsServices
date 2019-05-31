using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GageStatsDB.Migrations
{
    public partial class reconfigureObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Statistics_Citations_StationID",
                schema: "gagestats",
                table: "Statistics");

            migrationBuilder.DropTable(
                name: "StatisticErrors",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "StatisticUnitTypes",
                schema: "gagestats");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "gagestats",
                table: "Stations");

            migrationBuilder.RenameColumn(
                name: "StatisticGroupID",
                schema: "gagestats",
                table: "Statistics",
                newName: "StatisticGroupTypeID");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", "'postgis', '', ''");

            migrationBuilder.AlterColumn<double>(
                name: "YearsofRecord",
                schema: "gagestats",
                table: "Statistics",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CitationID",
                schema: "gagestats",
                table: "Statistics",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "PredictionIntervalID",
                schema: "gagestats",
                table: "Statistics",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRegulated",
                schema: "gagestats",
                table: "Stations",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.CreateTable(
                name: "Characteristics",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    StationID = table.Column<int>(nullable: false),
                    VariableTypeID = table.Column<int>(nullable: false),
                    UnitTypeID = table.Column<int>(nullable: false),
                    CitationID = table.Column<int>(nullable: true),
                    Value = table.Column<double>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characteristics", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Characteristics_Citations_CitationID",
                        column: x => x.CitationID,
                        principalSchema: "gagestats",
                        principalTable: "Citations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characteristics_Stations_StationID",
                        column: x => x.StationID,
                        principalSchema: "gagestats",
                        principalTable: "Stations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PredictionInterval",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Variance = table.Column<double>(nullable: true),
                    LowerConfidenceInterval = table.Column<double>(nullable: true),
                    UpperConfidenceInterval = table.Column<double>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionInterval", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StatisticError",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    StatisticID = table.Column<int>(nullable: false),
                    ErrorTypeID = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticError", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StatisticError_Statistics_StatisticID",
                        column: x => x.StatisticID,
                        principalSchema: "gagestats",
                        principalTable: "Statistics",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Username = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Role = table.Column<string>(nullable: false),
                    PrimaryPhone = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: false),
                    Salt = table.Column<string>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_CitationID",
                schema: "gagestats",
                table: "Statistics",
                column: "CitationID");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_PredictionIntervalID",
                schema: "gagestats",
                table: "Statistics",
                column: "PredictionIntervalID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_CitationID",
                schema: "gagestats",
                table: "Characteristics",
                column: "CitationID");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_StationID",
                schema: "gagestats",
                table: "Characteristics",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticError_StatisticID",
                schema: "gagestats",
                table: "StatisticError",
                column: "StatisticID");

            migrationBuilder.AddForeignKey(
                name: "FK_Statistics_Citations_CitationID",
                schema: "gagestats",
                table: "Statistics",
                column: "CitationID",
                principalSchema: "gagestats",
                principalTable: "Citations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Statistics_PredictionInterval_PredictionIntervalID",
                schema: "gagestats",
                table: "Statistics",
                column: "PredictionIntervalID",
                principalSchema: "gagestats",
                principalTable: "PredictionInterval",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Statistics_Citations_CitationID",
                schema: "gagestats",
                table: "Statistics");

            migrationBuilder.DropForeignKey(
                name: "FK_Statistics_PredictionInterval_PredictionIntervalID",
                schema: "gagestats",
                table: "Statistics");

            migrationBuilder.DropTable(
                name: "Characteristics",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "PredictionInterval",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "StatisticError",
                schema: "gagestats");

            migrationBuilder.DropTable(
                name: "User",
                schema: "gagestats");

            migrationBuilder.DropIndex(
                name: "IX_Statistics_CitationID",
                schema: "gagestats",
                table: "Statistics");

            migrationBuilder.DropIndex(
                name: "IX_Statistics_PredictionIntervalID",
                schema: "gagestats",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "PredictionIntervalID",
                schema: "gagestats",
                table: "Statistics");

            migrationBuilder.RenameColumn(
                name: "StatisticGroupTypeID",
                schema: "gagestats",
                table: "Statistics",
                newName: "StatisticGroupID");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", "'postgis', '', ''")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AlterColumn<int>(
                name: "YearsofRecord",
                schema: "gagestats",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CitationID",
                schema: "gagestats",
                table: "Statistics",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRegulated",
                schema: "gagestats",
                table: "Stations",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "gagestats",
                table: "Stations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StatisticErrors",
                schema: "gagestats",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ErrorTypeID = table.Column<int>(nullable: false),
                    StatisticID = table.Column<int>(nullable: true),
                    UnitTypeID = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false)
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
                name: "IX_StatisticErrors_StatisticID",
                schema: "gagestats",
                table: "StatisticErrors",
                column: "StatisticID");

            migrationBuilder.AddForeignKey(
                name: "FK_Statistics_Citations_StationID",
                schema: "gagestats",
                table: "Statistics",
                column: "StationID",
                principalSchema: "gagestats",
                principalTable: "Citations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
