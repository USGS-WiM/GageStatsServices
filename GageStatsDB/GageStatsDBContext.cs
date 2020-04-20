//------------------------------------------------------------------------------
//----- DB Context ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Responsible for interacting with Database 
//
//discussion:   The primary class that is responsible for interacting with data as objects. 
//              The context class manages the entity objects during run time, which includes 
//              populating objects with data from a database, change tracking, and persisting 
//              data to the database.
//              
//
//   

using Microsoft.EntityFrameworkCore;
using GageStatsDB.Resources;
using Microsoft.EntityFrameworkCore.Metadata;
using NetTopologySuite;
using SharedDB.Resources;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

//specifying the data provider and connection string
namespace GageStatsDB
{
    public class GageStatsDBContext:DbContext
    {
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Characteristic> Characteristics { get; set; }
        public DbSet<Citation> Citations { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<StationType> StationTypes { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<User> User { get; set; }

        //Shared views
        public DbSet<ErrorType> ErrorTypes { get; set; }
        public DbSet<UnitType> UnitTypes { get; set; }
        public DbSet<RegressionType> RegressionTypes { get; set; }
        public DbSet<StatisticGroupType> StatisticGroupTypes { get; set; }
        public DbSet<UnitConversionFactor> UnitConversionFactors { get; set; }
        public DbSet<UnitSystemType> UnitSystemTypes { get; set; }
        public DbSet<VariableType> VariableTypes { get; set; }

        public GageStatsDBContext() : base()
        {
        }
        public GageStatsDBContext(DbContextOptions<GageStatsDBContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            //specify DB schema
            modelBuilder.HasDefaultSchema("gagestats");
            modelBuilder.Entity<ErrorType>().ToTable("ErrorType_view");
            modelBuilder.Entity<RegressionType>().ToTable("RegressionType_view");
            modelBuilder.Entity<StatisticGroupType>().ToTable("StatisticGroupType_view");
            modelBuilder.Entity<UnitConversionFactor>().ToTable("UnitConversionFactor_view");
            modelBuilder.Entity<UnitSystemType>().ToTable("UnitSystemType_view");
            modelBuilder.Entity<UnitType>().ToTable("UnitType_view");
            modelBuilder.Entity<VariableType>().ToTable("VariableType_view");

            //Specify other unique constraints
            //EF Core currently does not support changing the value of alternate keys. We do have #4073 tracking removing this restriction though.
            //BTW it only needs to be an alternate key if you want it to be used as the target key of a relationship.If you just want a unique index, then use the HasIndex() method, rather than AlternateKey().Unique index values can be changed.
            modelBuilder.Entity<Station>().HasIndex(k => k.Code).IsUnique();
            modelBuilder.Entity<Agency>().HasIndex(k => k.Code).IsUnique();
            modelBuilder.Entity<StationType>().HasIndex(k => k.Code).IsUnique();

            modelBuilder.Entity<User>().Property(e => e.Role).HasConversion<string>();

            //add shadowstate for when models change
            foreach (var entitytype in modelBuilder.Model.GetEntityTypes())
            {
                if (new List<string>() { typeof(StatisticError).FullName,typeof(ErrorType).FullName,typeof(RegressionType).FullName,
                                         typeof(StatisticGroupType).FullName,typeof(UnitConversionFactor).FullName,typeof(UnitSystemType).FullName,
                                         typeof(UnitType).FullName,typeof(VariableType).FullName }
                .Contains(entitytype.Name))
                { continue; }

                modelBuilder.Entity(entitytype.Name).Property<DateTime>("LastModified");
            }//next entitytype



            //cascade delete is default, rewrite behavior
            modelBuilder.Entity(typeof(Statistic).ToString(), b =>
            {
                b.HasOne(typeof(Station).ToString(), "Station")
                    .WithMany("Statistics")
                    .HasForeignKey("StationID")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(typeof(Citation).ToString(), "Citation")
                    .WithMany("Statistics")
                    .HasForeignKey("CitationID")
                    .OnDelete(DeleteBehavior.Restrict);
               
            });
            modelBuilder.Entity<Statistic>()
                .HasOne(p => p.PredictionInterval)
                .WithOne(s => s.Statistic)
                .HasForeignKey<Statistic>(p => p.PredictionIntervalID)
                .OnDelete(DeleteBehavior.Cascade);
                

            /*modelBuilder.Entity(typeof(Station).ToString(), b =>
            {
                b.HasOne(typeof(StationType).ToString(), "StationType")
                    .WithMany("Stations")
                    .HasForeignKey("StationTypeID")
                    .OnDelete(DeleteBehavior.Restrict);
                b.HasOne(typeof(Agency).ToString(), "Agency")
                    .WithMany("Stations")
                    .HasForeignKey("AgencyID")
                    .OnDelete(DeleteBehavior.Restrict);
            });*/

            base.OnModelCreating(modelBuilder);

            //Must Comment out after migration
            //modelBuilder.Ignore(typeof(ErrorType));
            //modelBuilder.Ignore(typeof(RegressionType));
            //modelBuilder.Ignore(typeof(StatisticGroupType));
            //modelBuilder.Ignore(typeof(UnitConversionFactor));
            //modelBuilder.Ignore(typeof(UnitSystemType));
            //modelBuilder.Ignore(typeof(UnitType));
            //modelBuilder.Ignore(typeof(VariableType));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#warning Add connectionstring for migrations
            //var connectionstring = "User ID=;Password=;Host=;Port=5432;Database=StatsDB;Pooling=true;";
            //optionsBuilder.UseNpgsql(connectionstring, x => { x.MigrationsHistoryTable("_EFMigrationsHistory", "gagestats"); x.UseNetTopologySuite(); });
        }

       
    }
}
