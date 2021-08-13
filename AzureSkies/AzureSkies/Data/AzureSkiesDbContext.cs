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
        public DbSet<FlightInfo> FlightsInfo { get; set; }

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
                    FlightDate = "2021-08-10",
                    FlightStatus = "active",
                    AirlineName = "Delta Airlines",
                    DepartureAirport = "Seattle-Tacoma International Airport",
                    ArrivalAirport = "John F. Kennedy International Airport",
                    FlightIcao = "DAL.0001",
                    FlightNumber = "0001",
                    PhoneNumbers = "+1234567889"
                });
        }
    }
}
