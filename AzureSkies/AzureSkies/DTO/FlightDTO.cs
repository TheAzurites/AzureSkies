using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.DTO
{
    public class FlightDTO
    {
        // Flight {FlightNumber}
        public string FlightNumber { get; set; }
        // with {Airline}
        public string Airline { get; set; }
        // on {Date},
        public string Date { get; set; }
        // current status is: {FlightStatus}
        public string FlightStatus { get; set; }
    }
}
