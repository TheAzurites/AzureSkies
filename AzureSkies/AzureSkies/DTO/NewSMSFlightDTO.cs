using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.DTO
{
    public class NewSMSFlightDTO
    {
        // Flight {FlightNumber}
        public string id { get; set; }
        // with {Airline}
        public string topic{ get; set; }
        // on {Date},
        public string subject { get; set; }

        public string eventType { get; set; }

        public string dataVersion { get; set; }

        public string metadataVersion { get; set; }

        public string eventTime { get; set; }
        // current status is: {FlightStatus}
        public NewSMSDataDTO newSmsDTO { get; set; }
    }
}
