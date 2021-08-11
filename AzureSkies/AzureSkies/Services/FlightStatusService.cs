using AzureSkies.DTO;
using AzureSkies.Interfaces;
using AzureSkies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AzureSkies.Services
{
    public class FlightStatusService : IFlightStatus
    {
        static HttpClient client = new HttpClient();

        public async Task<FlightInfo> AddFlight(string flight_icao)
        {
            string accessKey = Environment.GetEnvironmentVariable("AVIATION_STACK_API_KEY");
            string path = $"http://api.aviationstack.com/v1/flights?access_key={accessKey}&flight_icao={flight_icao}&limit=1";


            // FlightDTO should be flightInfo when finished. We should create a new DTO from the flightinfo that we 
            // get.
            //Root schema = new();
            
            HttpResponseMessage response = await client.GetAsync(path);
            Root schema = new Root();
            if (response.IsSuccessStatusCode)
            {
                schema = await response.Content.ReadAsAsync<Root>();
            }

            FlightInfo flightInfo = new()
            {
                Id = Convert.ToInt32(schema.data[0].flight.number),
                airlineName = schema.data[0].airline.name,
                flightDate = schema.data[0].flight_date,
                departureAirport = schema.data[0].departure.airport,
                arrivalAirport = schema.data[0].arrival.airport,
                flightStatus = schema.data[0].flight_status,
                flightIata = schema.data[0].flight.iata,
                flightNumber = schema.data[0].flight.number
            };

            return flightInfo;
        }

        public Task<FlightDTO> GetFlight(NewSMSFlightDTO newSMSFlightDTO)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

    }
}
