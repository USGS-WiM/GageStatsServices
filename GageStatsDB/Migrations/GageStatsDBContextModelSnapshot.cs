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
                .HasAnnotation("Npgsql:PostgresExtension:postgis", ",,")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
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

            modelBuilder.Entity("GageStatsDB.Resources.Characteristic", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CitationID");

                    b.Property<string>("Comments");

                    b.Property<DateTime>("LastModified");

                    b.Property<int>("StationID");

                    b.Property<int>("UnitTypeID");

                    b.Property<double>("Value");

                    b.Property<int>("VariableTypeID");

                    b.HasKey("ID");

                    b.HasIndex("CitationID");

                    b.HasIndex("StationID");

                    b.ToTable("Characteristics");
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

            modelBuilder.Entity("GageStatsDB.Resources.PredictionInterval", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("LastModified");

                    b.Property<double?>("LowerConfidenceInterval");

                    b.Property<double?>("UpperConfidenceInterval");

                    b.Property<double?>("Variance");

                    b.HasKey("ID");

                    b.ToTable("PredictionInterval");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Station", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AgencyID");

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<bool?>("IsRegulated");

                    b.Property<DateTime>("LastModified");

                    b.Property<Geometry>("Location")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("StationTypeID");

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

                    b.Property<int?>("CitationID");

                    b.Property<string>("Comments");

                    b.Property<bool>("IsPreferred");

                    b.Property<DateTime>("LastModified");

                    b.Property<int?>("PredictionIntervalID");

                    b.Property<int>("RegressionTypeID");

                    b.Property<int>("StationID");

                    b.Property<int>("StatisticGroupTypeID");

                    b.Property<int>("UnitTypeID");

                    b.Property<double>("Value");

                    b.Property<double?>("YearsofRecord");

                    b.HasKey("ID");

                    b.HasIndex("CitationID");

                    b.HasIndex("PredictionIntervalID")
                        .IsUnique();

                    b.HasIndex("StationID");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("GageStatsDB.Resources.StatisticError", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ErrorTypeID");

                    b.Property<int>("StatisticID");

                    b.Property<double>("Value");

                    b.HasKey("ID");

                    b.HasIndex("StatisticID");

                    b.ToTable("StatisticError");
                });

            modelBuilder.Entity("GageStatsDB.Resources.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("PrimaryPhone");

                    b.Property<string>("Role")
                        .IsRequired();

                    b.Property<string>("Salt")
                        .IsRequired();

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Characteristic", b =>
                {
                    b.HasOne("GageStatsDB.Resources.Citation", "Citation")
                        .WithMany("Characteristics")
                        .HasForeignKey("CitationID");

                    b.HasOne("GageStatsDB.Resources.Station")
                        .WithMany("Characteristics")
                        .HasForeignKey("StationID")
                        .OnDelete(DeleteBehavior.Cascade);
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
                        .HasForeignKey("CitationID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GageStatsDB.Resources.PredictionInterval", "PredictionInterval")
                        .WithOne("Statistic")
                        .HasForeignKey("GageStatsDB.Resources.Statistic", "PredictionIntervalID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GageStatsDB.Resources.Station", "Station")
                        .WithMany("Statistics")
                        .HasForeignKey("StationID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GageStatsDB.Resources.StatisticError", b =>
                {
                    b.HasOne("GageStatsDB.Resources.Statistic", "Statistic")
                        .WithMany("StatisticErrors")
                        .HasForeignKey("StatisticID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
