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

            modelBuilder.Entity<FlightInfo>().HasData(
                new FlightInfo
                {
                    Id = 0001,
                    flightDate = "2021-08-10",
                    flightStatus = "active",
                    airlineName = "Delta Airlines",
                    departureAirport = "Seattle-Tacoma International Airport",
                    arrivalAirport = "John F. Kennedy International Airport",
                    flightIata = "DAL.0001",
                    flightNumber = "0001"
                });
        }
    }
}
