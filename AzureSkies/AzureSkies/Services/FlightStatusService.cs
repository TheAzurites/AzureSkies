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

        public async Task AddFlight(string message, string phoneNumber)
        {

            string accessKey = Environment.GetEnvironmentVariable("AVIATION_STACK_API_KEY");
            string path = $"http://api.aviationstack.com/v1/flights?access_key={accessKey}&flight_icao={message}&limit=1";

            // Establishing a connection to the SMS Service for sending texts to users.
            string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");
            SmsClient smsClient = new SmsClient(connectionString);

            HttpResponseMessage response = await client.GetAsync(path);
            Root schema = new Root();
            if (response.IsSuccessStatusCode)
            {
                schema = await response.Content.ReadAsAsync<Root>();
            }
            else
            {
            SmsSendResult sendBadRequest = smsClient.Send(
                from: "+18443976066",
                to: phoneNumber,
                message: "The flight number that you attempted to enter was not found in the database."         
                );
            }

            if (schema.data.Count <= 0)
            {
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

            // Saving flightInfo to DbContext for later use while accessing
            _context.Entry(flightInfo).State = EntityState.Added;
            await _context.SaveChangesAsync();

            SmsSendResult sendResult = smsClient.Send(
                from: "+18443976066",
                to: phoneNumber,
                message: ($"Welcome to Azure Skies. Flight {flightInfo.FlightIcao} with {flightInfo.AirlineName} on {flightInfo.FlightDate}" +
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
            string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");

            SmsClient smsClient = new SmsClient(connectionString);

            string accessKey = Environment.GetEnvironmentVariable("AVIATION_STACK_API_KEY");
            IList<FlightInfo> flightsToUpdate = await _context.FlightsInfo.ToListAsync();
            
            foreach(FlightInfo flight in flightsToUpdate)
            {
                string icao = flight.FlightIcao;
                string path = $"http://api.aviationstack.com/v1/flights?access_key={accessKey}&flight_icao={icao}&limit=1";

                HttpResponseMessage response = await client.GetAsync(path);
                Root schema = new Root();
                if (response.IsSuccessStatusCode)
                {
                    schema = await response.Content.ReadAsAsync<Root>();
                    if (schema.data[0].flight_status == "cancelled" || schema.data[0].flight_status == "landed")
                    {
                        SmsSendResult sendResult = smsClient.Send(
                        from: "+18443976066",
                        to: flight.PhoneNumbers,
                        message: ($"Flight {flight.FlightDate} with {flight.AirlineName} on {flight.FlightNumber}" +
                        $" current status is: {flight.FlightStatus}. You are now unsubscribed from texts.")
                        );
                        await Delete(flight.Id);
                    }
                _context.Entry(flight).State = EntityState.Modified;
                }
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
