using AzureSkies.DTO;
using AzureSkies.Interfaces;
using AzureSkies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureSkies.Services
{
    public class FlightStatusService : IFlightStatus
    {
        static HttpClient client = new HttpClient();

        public async Task<FlightDTO> AddFlight(string flightNumber, string airline, string date)
        {
            string access_key = "00d536921176e9ab78f646a12b9c15e8";
            string path = $"http://api.aviationstack.com/v1/flights?{access_key}&" +
                            $"airlineName={airline}&flightNumber={flightNumber}" +
                            $"&flightDate={date}";

            /*  string path = $"http://api.aviationstack.com/v1/flights?{access_key}&" +
                            $"airlineName={flight.Airline}&flightNumber={flight.FlightNumber}" +
                            $"&flightDate={flight.FlightDate}";
            */

            // FlightDTO should be flightInfo when finished. We should create a new DTO from the flightinfo that we 
            // get.
            FlightDTO flightInfo = new();
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                flightInfo = await response.Content.ReadAsAsync<FlightDTO>();
            }

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
