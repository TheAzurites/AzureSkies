using AzureSkies.DTO;
using AzureSkies.Interfaces;
using AzureSkies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AzureSkies.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Communication.Sms;

namespace AzureSkies.Services
{
    public class FlightStatusService : IFlightStatus
    {
        static HttpClient client = new HttpClient();
        private AzureSkiesDbContext _context;

        public FlightStatusService(AzureSkiesDbContext context)
        {
            _context = context;
        }

        public async void AddFlight(string message, string phoneNumber)
        {
            string accessKey = Environment.GetEnvironmentVariable("AVIATION_STACK_API_KEY");
            string path = $"http://api.aviationstack.com/v1/flights?access_key={accessKey}&flight_icao={message}&limit=1";

            HttpResponseMessage response = await client.GetAsync(path);
            Root schema = new Root();
            if (response.IsSuccessStatusCode)
            {
                schema = await response.Content.ReadAsAsync<Root>();
            }

            FlightInfo flightInfo = new()
            {
                Id = Convert.ToInt32(schema.data[0].flight.number),
                AirlineName = schema.data[0].airline.name,
                FlightDate = schema.data[0].flight_date,
                DepartureAirport = schema.data[0].departure.airport,
                ArrivalAirport = schema.data[0].arrival.airport,
                FlightStatus = schema.data[0].flight_status,
                FlightIcao = schema.data[0].flight.icao,
                FlightNumber = schema.data[0].flight.number,
                PhoneNumbers = message
            };

            string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");

            SmsClient smsClient = new SmsClient(connectionString);

            SmsSendResult sendResult = smsClient.Send(
                from: "+18443976066",
                to: phoneNumber,
                message: ($"Flight {flightInfo.FlightDate} with {flightInfo.AirlineName} on {flightInfo.FlightNumber}" +
                $" current status is: {flightInfo.FlightStatus}. You are now subscribed to automatic flight updates.")
                );
        }

        //public Task<FlightDTO> GetFlight(NewSMSFlightDTO newSMSFlightDTO)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task Delete(int id)
        {
            FlightInfo flight = await _context.FlightsInfo.FindAsync(id);
            _context.Entry(flight).State = EntityState.Deleted;
            // save the changes.
            await _context.SaveChangesAsync();
        }

        public async Task<IList<FlightDTO>> GetFlights()
        {
            // LINQ Query - each flight that has a flight status of "landed" is put into
            // a new iterable list and deleted. A message is also sent out marking deletion.

            IList<FlightInfo> flightInfoLINQ = await _context.FlightsInfo.Where(
                flight => flight.FlightStatus.Contains("landed")).ToListAsync();
            foreach(FlightInfo flight in flightInfoLINQ)
            {
                // Delete the current flight from the DB.
                    string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");

                    SmsClient smsClient = new SmsClient(connectionString);

                    SmsSendResult sendResult = smsClient.Send(
                        from: "+18443976066",
                        to: flight.PhoneNumbers,
                        message: ($"Flight {flight.FlightDate} with {flight.AirlineName} on {flight.FlightNumber}" +
                        $" current status is: {flight.FlightStatus}. You are now unsubscribed from texts.")
                        );
                await Delete(flight.Id);
            }

            IList<FlightDTO> list = await _context.FlightsInfo
                .Select(flight => new FlightDTO
                {
                    FlightNumber = flight.FlightNumber,
                    Airline = flight.AirlineName,
                    Date = flight.FlightDate,
                    FlightStatus = flight.FlightStatus,
                    PhoneNumbers = flight.PhoneNumbers
                }).ToListAsync();

            foreach(FlightDTO flight in list)
            {
                    string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");

                    SmsClient smsClient = new SmsClient(connectionString);

                    SmsSendResult sendResult = smsClient.Send(
                        from: "+18443976066",
                        to: flight.PhoneNumbers,
                        message: ($"Flight {flight.Date} with {flight.Airline} on {flight.FlightNumber}" +
                        $" current status is: {flight.FlightStatus}.")
                        );
            }
            return list;
        }
    }
}
