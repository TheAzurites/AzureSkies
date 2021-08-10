using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.DTO
{
    public class NewSMSDataDTO
    {
        // Flight {FlightNumber}
        public string messageId { get; set; }
        // with {Airline}
        public string from { get; set; }
        // on {Date},
        public string to { get; set; }
        // current status is: {FlightStatus}
        public string message { get; set; }

        public string receivedTimestamp { get; set; }
    }
}
