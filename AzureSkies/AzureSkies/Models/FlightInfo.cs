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
        [Required]
        public string FlightStatus { get; set; }
        [Required]
        public string FlightDate { get; set; }
        [Required]
        public string FlightNumber { get; set; }
        [Required]
        public string Airline { get; set; }
        public List<string> GuestPhoneNumber { get; set; }
    }
}
