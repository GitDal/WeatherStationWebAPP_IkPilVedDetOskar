using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeatherStationWebAPP.Models;

namespace WeatherStationWebAPP.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<WeatherObservation> Observations { get; set; }
        public DbSet<Place> Places { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* Write Fluent API configurations here */
            base.OnModelCreating(modelBuilder);

            //Place
            modelBuilder.Entity<Place>().HasKey(p => p.Id);
            modelBuilder.Entity<Place>().HasIndex(i => new { i.Latitude, i.Longitude }).IsUnique();

            // WeatherObservation
            modelBuilder.Entity<WeatherObservation>().HasKey(p => p.Id);
            modelBuilder.Entity<WeatherObservation>()
                .HasOne<Place>()
                .WithMany()
                .HasForeignKey(f => f.PlaceId);

        }
    }
}
