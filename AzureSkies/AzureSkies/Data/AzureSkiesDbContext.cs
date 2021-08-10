using AzureSkies.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.Data
{
    public class AzureSkiesDbContext : DbContext
    {
        public DbSet<FlightInfo> FlightInfo { get; set; }

        public AzureSkiesDbContext(DbContextOptions options) : base(options)
        { 

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DataFlight>().HasKey(
                dataFlight => new {dataFlight.DataId, dataFlight.FlightId}
                );

            modelBuilder.Entity<FlightInfo>().HasData(
                new FlightInfo
                {
                    id = 1
                });

            modelBuilder.Entity<Models.Data>().HasData(
                new Models.Data
                {
                    id = 1,
                    flight_date = "2021-08-10",
                    flight_status = "active",
                });
            modelBuilder.Entity<Flight>().HasData(
                new Flight
                {
                    id = 1,
                    iata = "DL0348"
                });
        }
    }
}
