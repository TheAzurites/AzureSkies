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
        public Task<FlightDTO> GetFlight(NewSMSFlightDTO newSMSFlightDTO);

        public Task<FlightInfo> AddFlight(string flight_icao);

        public Task Delete(int id);
    }
}
