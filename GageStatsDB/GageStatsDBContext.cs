//------------------------------------------------------------------------------
//----- DB Context ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

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
//specifying the data provider and connection string
namespace GageStatsDB
{
    public class GageStatsDBContext:DbContext
    {
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Citation> Citations { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<StationType> StationTypes { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<StatisticUnitTypes> StatisticUnitTypes { get; set; }
        public DbSet<StatisticErrors> StatisticErrors { get; set; }

        //DbQuery from here:
        //https://docs.microsoft.com/en-us/ef/core/modeling/query-types
        public DbQuery<RegressionType> RegressionTypes { get; set; }
        public DbQuery<StatisticGroupType> StatisticGroupType { get; set; }
        public DbQuery<UnitConversions> UnitConversions { get; set; }
        public DbQuery<UnitsType> UnitsTypes { get; set; }
        public DbQuery<UnitSystemType> UnitSystemTypes { get; set; }
        public DbQuery<ErrorType> ErrorTypes { get; set; }
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
            modelBuilder.Entity<Agency>().ToTable("Agency", "gagestats");
            modelBuilder.Entity<Citation>().ToTable("Citation", "gagestats");
            modelBuilder.Entity<Station>().ToTable("Station", "gagestats");
            modelBuilder.Entity<StationType>().ToTable("StationType", "gagestats");
            modelBuilder.Entity<Statistic>().ToTable("Statistic", "gagestats");
            modelBuilder.Entity<StatisticErrors>().ToTable("StatisticErrors", "gagestats");
            modelBuilder.Entity<StatisticUnitTypes>().ToTable("StatisticUnitTypes", "gagestats");
            modelBuilder.Query<ErrorType>().ToView("ErrorType_view", "gagestats");
                
            //unique key based on region and manager keys
            modelBuilder.Entity<StatisticUnitTypes>().HasKey(k => new { k.StatisticID, k.UnitTypeID });

            //Specify other unique constraints
            //EF Core currently does not support changing the value of alternate keys. We do have #4073 tracking removing this restriction though.
            //BTW it only needs to be an alternate key if you want it to be used as the target key of a relationship.If you just want a unique index, then use the HasIndex() method, rather than AlternateKey().Unique index values can be changed.
            modelBuilder.Entity<Station>().HasIndex(k => k.Code).IsUnique();
            modelBuilder.Entity<Agency>().HasIndex(k => k.Code).IsUnique();
            modelBuilder.Entity<StationType>().HasIndex(k => k.Code).IsUnique();

            //add shadowstate for when models change
            foreach (var entitytype in modelBuilder.Model.GetEntityTypes())
            {
                //modelBuilder.Entity(entitytype.Name).Property<DateTime>("LastModified");
            }//next entitytype

            //cascade delete is default, rewrite behavior
            modelBuilder.Entity("GageStatsDB.Resources.Statistic", b =>
            {
                b.HasOne("GageStatsDB.Resources.Station", "Station")
                    .WithMany()
                    .HasForeignKey("StationID")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity("GageStatsDB.Resources.Station", b =>
            {
                b.HasOne("GageStatsDB.Resources.StationType", "StationType")
                    .WithMany()
                    .HasForeignKey("StationTypeID")
                    .OnDelete(DeleteBehavior.Restrict);
                b.HasOne("GageStatsDB.Resources.Agency", "Agency")
                    .WithMany()
                    .HasForeignKey("AgencyID")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity("GageStatsDB.Resources.Statistic", b =>
            {
                b.HasOne("GageStatsDB.Resources.Citation", "Citation")
                    .WithMany()
                    .HasForeignKey("StationID")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);             
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#warning Add connectionstring for migrations
            var connectionstring = "User ID=;Password=;Host=;Port=;Database=;Pooling=true;";
            optionsBuilder.UseNpgsql(connectionstring, x => { x.MigrationsHistoryTable("_EFMigrationsHistory", "gagestats"); x.UseNetTopologySuite(); });
        }
    }
}
