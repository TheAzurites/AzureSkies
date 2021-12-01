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
        public string FlightDate { get; set; }
        public string FlightStatus { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public string AirlineName { get; set; }
        public string FlightIcao { get; set; }
        public string FlightNumber { get; set; }
        [Required]
        public string PhoneNumbers { get; set; }
    }
}
