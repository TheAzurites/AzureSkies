using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.Models
{
    public class Root
    {
        public List<Datum> data { get; set; }
        public Pagination pagination { get; set; }
        public Error error { get; set; }
    }
    public class Datum
    {
        public string flight_date { get; set; }
        public string flight_status { get; set; }
        public Departure departure { get; set; }
        public Arrival arrival { get; set; }
        public Airline airline { get; set; }
        public Flight flight { get; set; }
    }

    public class Error
    {
        public string message { get; set; }
    }
    public class Pagination
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public int count { get; set; }
        public int total { get; set; }
    }

    public class Departure
    {
        public string airport { get; set; }
        public string terminal { get; set; }
        public string gate { get; set; }
    }

    public class Arrival
    {
        public string airport { get; set; }
        public string timezone { get; set; }
        public string terminal { get; set; }
        public string gate { get; set; }
        public string baggage { get; set; }
    }

    public class Airline
    {
        public string name { get; set; }
    }

    public class Flight
    {
        public string iata { get; set; }
        public string icao { get; set; }
        public string number { get; set; }
    }
}
