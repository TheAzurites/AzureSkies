﻿using AzureSkies.DTO;
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
        // public Task<FlightDTO> GetFlight(NewSMSFlightDTO newSMSFlightDTO);

        public void AddFlight(string message, string phoneNumber);

        public Task<IList<FlightDTO>> GetFlights();

        public Task Delete(int id);
    }
}
