﻿// <auto-generated />
using System;
using GageStatsDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GageStatsDB.Migrations
{
    [DbContext(typeof(GageStatsDBContext))]
    [Migration("20201027153515_LocationSource")]
    partial class LocationSource
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("gagestats")
                .HasAnnotation("Npgsql:PostgresExtension:postgis", ",,")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("GageStatsDB.Resources.Agency", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("Agencies");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Characteristic", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("CitationID")
                        .HasColumnType("integer");

                    b.Property<string>("Comments")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("StationID")
                        .HasColumnType("integer");

                    b.Property<int>("UnitTypeID")
                        .HasColumnType("integer");

                    b.Property<double>("Value")
                        .HasColumnType("double precision");

                    b.Property<int>("VariableTypeID")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("CitationID");

                    b.HasIndex("StationID");

                    b.ToTable("Characteristics");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Citation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CitationURL")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Citations");
                });

            modelBuilder.Entity("GageStatsDB.Resources.PredictionInterval", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double?>("LowerConfidenceInterval")
                        .HasColumnType("double precision");

                    b.Property<double?>("UpperConfidenceInterval")
                        .HasColumnType("double precision");

                    b.Property<double?>("Variance")
                        .HasColumnType("double precision");

                    b.HasKey("ID");

                    b.ToTable("PredictionInterval");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Station", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AgencyID")
                        .HasColumnType("integer");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool?>("IsRegulated")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Geometry>("Location")
                        .IsRequired()
                        .HasColumnType("geometry");

                    b.Property<string>("LocationSource")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("RegionID")
                        .HasColumnType("integer");

                    b.Property<int>("StationTypeID")
                        .HasColumnType("integer");

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
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("StationTypes");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Statistic", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("CitationID")
                        .HasColumnType("integer");

                    b.Property<string>("Comments")
                        .HasColumnType("text");

                    b.Property<bool>("IsPreferred")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("PredictionIntervalID")
                        .HasColumnType("integer");

                    b.Property<int>("RegressionTypeID")
                        .HasColumnType("integer");

                    b.Property<int>("StationID")
                        .HasColumnType("integer");

                    b.Property<int>("StatisticGroupTypeID")
                        .HasColumnType("integer");

                    b.Property<int>("UnitTypeID")
                        .HasColumnType("integer");

                    b.Property<double>("Value")
                        .HasColumnType("double precision");

                    b.Property<double?>("YearsofRecord")
                        .HasColumnType("double precision");

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
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ErrorTypeID")
                        .HasColumnType("integer");

                    b.Property<int>("StatisticID")
                        .HasColumnType("integer");

                    b.Property<double>("Value")
                        .HasColumnType("double precision");

                    b.HasKey("ID");

                    b.HasIndex("StatisticID");

                    b.ToTable("StatisticError");
                });

            modelBuilder.Entity("GageStatsDB.Resources.Characteristic", b =>
                {
                    b.HasOne("GageStatsDB.Resources.Citation", "Citation")
                        .WithMany("Characteristics")
                        .HasForeignKey("CitationID");

                    b.HasOne("GageStatsDB.Resources.Station", "Station")
                        .WithMany("Characteristics")
                        .HasForeignKey("StationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GageStatsDB.Resources.Station", b =>
                {
                    b.HasOne("GageStatsDB.Resources.Agency", "Agency")
                        .WithMany("Stations")
                        .HasForeignKey("AgencyID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GageStatsDB.Resources.StationType", "StationType")
                        .WithMany("Stations")
                        .HasForeignKey("StationTypeID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
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
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("GageStatsDB.Resources.StatisticError", b =>
                {
                    b.HasOne("GageStatsDB.Resources.Statistic", "Statistic")
                        .WithMany("StatisticErrors")
                        .HasForeignKey("StatisticID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
