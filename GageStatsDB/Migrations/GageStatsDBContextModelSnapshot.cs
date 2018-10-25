﻿// <auto-generated />
using System;
using GageStatsDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GageStatsDB.Migrations
{
    [DbContext(typeof(GageStatsDBContext))]
    partial class GageStatsDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("gagestats")
                .HasAnnotation("Npgsql:PostgresExtension:postgis", "'postgis', '', ''")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("GageStatsDB.Resources.Agency", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<string>("Description");

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("Agencies");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Citation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author")
                        .IsRequired();

                    b.Property<string>("CitationURL")
                        .IsRequired();

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("Citations");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Station", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AgencyID");

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<bool>("IsRegulated");

                    b.Property<DateTime>("LastModified");

                    b.Property<Point>("Location")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("StationTypeID");

                    b.Property<string>("Type")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("AgencyID");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("StationTypeID");

                    b.ToTable("Stations");
                });

            modelBuilder.Entity("GageStatsDB.Resources.StationType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<string>("Description");

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("StationTypes");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Statistic", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CitationID");

                    b.Property<string>("Comments");

                    b.Property<DateTime>("LastModified");

                    b.Property<int>("RegressionTypeID");

                    b.Property<int>("StationID");

                    b.Property<int>("StatisticGroupID");

                    b.Property<int>("UnitTypeID");

                    b.Property<double>("Value");

                    b.Property<int>("YearsofRecord");

                    b.HasKey("ID");

                    b.HasIndex("StationID");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("GageStatsDB.Resources.StatisticErrors", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ErrorTypeID");

                    b.Property<int?>("StatisticID");

                    b.Property<int>("UnitTypeID");

                    b.Property<double>("Value");

                    b.HasKey("ID");

                    b.HasIndex("StatisticID");

                    b.ToTable("StatisticErrors");
                });

            modelBuilder.Entity("GageStatsDB.Resources.StatisticUnitTypes", b =>
                {
                    b.Property<int>("StatisticID");

                    b.Property<string>("UnitTypeID");

                    b.Property<DateTime>("LastModified");

                    b.HasKey("StatisticID", "UnitTypeID");

                    b.ToTable("StatisticUnitTypes");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Station", b =>
                {
                    b.HasOne("GageStatsDB.Resources.Agency", "Agency")
                        .WithMany("Stations")
                        .HasForeignKey("AgencyID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GageStatsDB.Resources.StationType", "StationType")
                        .WithMany("Stations")
                        .HasForeignKey("StationTypeID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GageStatsDB.Resources.Statistic", b =>
                {
                    b.HasOne("GageStatsDB.Resources.Citation", "Citation")
                        .WithMany("Statistics")
                        .HasForeignKey("StationID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GageStatsDB.Resources.Station", "Station")
                        .WithMany("Statistics")
                        .HasForeignKey("StationID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GageStatsDB.Resources.StatisticErrors", b =>
                {
                    b.HasOne("GageStatsDB.Resources.Statistic", "Statistic")
                        .WithMany("StatisticErrors")
                        .HasForeignKey("StatisticID");
                });

            modelBuilder.Entity("GageStatsDB.Resources.StatisticUnitTypes", b =>
                {
                    b.HasOne("GageStatsDB.Resources.Statistic", "Statistic")
                        .WithMany("StatisticUnitTypes")
                        .HasForeignKey("StatisticID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
