using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.Models
{
    public class DataFlight
    {
        public int DataId { get; set; }
        public int FlightId { get; set; }

        public Data Data { get; set; }
        public Flight Flight { get; set; }
    }
}
