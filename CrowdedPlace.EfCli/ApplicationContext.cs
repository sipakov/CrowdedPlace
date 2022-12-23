using System;
using CrowdedPlace.Libraries.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CrowdedPlace.EfCli
{
    public sealed class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
            //  Database.EnsureCreated();
        }

        public ApplicationContext(int timeOut)
        {
            Database.SetCommandTimeout(timeOut);
        }

        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
          //  Database.EnsureCreated();
        }

        private static readonly ILoggerFactory Factory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var appConnectionString = Configurator.GetConfiguration().GetConnectionString("PostgreSqlConnection");
            if (optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(appConnectionString);
            }
            else
            {
                optionsBuilder.UseLoggerFactory(Factory).UseNpgsql(appConnectionString);
            }
        }
        
        public DbSet<Device> Devices { get; set; }
        public DbSet<Poster> Posters { get; set; }
        public DbSet<Demonstration> Demonstrations { get; set; }
        public DbSet<ObjectionableContent> ObjectionableContents { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureDevicesModelCreation(modelBuilder);
            ConfigurePostersModelCreation(modelBuilder);
            ConfigureDemonstrationsModelCreation(modelBuilder);
            ConfigureObjectionableContentModelCreation(modelBuilder);
        }

        private static void ConfigureDevicesModelCreation(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            modelBuilder.Entity<Device>().HasKey(x => new { x.Id});
            modelBuilder.Entity<Device>().Property(x => x.FcmToken).HasMaxLength(500);
            modelBuilder.Entity<Device>().Property(x => x.Locale).HasMaxLength(50);
        }
        
        private static void ConfigurePostersModelCreation(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            modelBuilder.Entity<Poster>().HasKey(x => new { x.DeviceId, x.CreatedDate, x.DemonstrationId});
            modelBuilder.Entity<Poster>().Property(x => x.Name).HasMaxLength(30);
            modelBuilder.Entity<Poster>().Property(x => x.Title).HasMaxLength(100);
            modelBuilder.Entity<Poster>().Property(x => x.Message).HasMaxLength(1000);
            modelBuilder.Entity<Poster>().HasIndex(x => x.DemonstrationId);
        }
        
        private static void ConfigureDemonstrationsModelCreation(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            modelBuilder.Entity<Demonstration>().HasKey(x => new { x.Id});
            modelBuilder.Entity<Demonstration>().Property(x => x.CityName).HasMaxLength(100);
            modelBuilder.Entity<Demonstration>().Property(x => x.CountryName).HasMaxLength(100);
            modelBuilder.Entity<Demonstration>().Property(x => x.AreaName).HasMaxLength(100);
        }

        private static void ConfigureObjectionableContentModelCreation(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            modelBuilder.Entity<ObjectionableContent>().Property(x=>x.Comment).HasMaxLength(500);
        }
    }
}
