using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.Models
{
    public class FlightInfo
    {
        [Required]
        public int Id { get; set; }
        public string flightDate { get; set; }
        public string flightStatus { get; set; }
        public string departureAirport { get; set; }
        public string arrivalAirport { get; set; }
        public string airlineName { get; set; }
        public string flightIata { get; set; }
        public string flightNumber { get; set; }

    }
}
