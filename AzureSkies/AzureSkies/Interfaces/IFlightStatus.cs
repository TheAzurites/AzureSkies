using AzureSkies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.Interfaces
{
    public interface IFlightStatus
    {
        // Get Flight data from API
        public Task<FlightInfo> GetFlight(string path);

        public Task<FlightInfo> AddFlight(FlightInfo flight);

        public Task Delete(int id);
    }
}
