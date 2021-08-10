using AzureSkies.DTO;
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
        public Task<FlightDTO> GetFlight(string flightDate, string flightNumber);

        public Task<FlightDTO> AddFlight(FlightInfo flight);

        public Task Delete(int id);
    }
}
