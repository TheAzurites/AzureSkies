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

        }
    }
}
