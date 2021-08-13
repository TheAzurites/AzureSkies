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
                message: "Azure Skies is currently under maintenance. Please try again later."         
                );
            }

            FlightInfo flightInfo = new()
            {
                AirlineName = schema.data[0].airline.name,
                FlightDate = schema.data[0].flight_date,
                DepartureAirport = schema.data[0].departure.airport,
                ArrivalAirport = schema.data[0].arrival.airport,
                FlightStatus = schema.data[0].flight_status,
                FlightIcao = schema.data[0].flight.icao,
                FlightNumber = schema.data[0].flight.number,
                PhoneNumbers = phoneNumber
            };

            // Saving flightInfo to DbContext for later use while accessing
            _context.Entry(flightInfo).State = EntityState.Added;
            await _context.SaveChangesAsync();

            SmsSendResult sendResult = smsClient.Send(
                from: "+18443976066",
                to: phoneNumber,
                message: ($"Welcome to Azure Skies. Flight {flightInfo.FlightIcao} with {flightInfo.AirlineName} on {flightInfo.FlightDate}.\n" +
                    $"Depart from: {flightInfo.DepartureAirport}.\n" +
                    $"Arrive at: {flightInfo.ArrivalAirport}.\n" +
                $"Current status is: {flightInfo.FlightStatus}. You are now subscribed to automatic flight updates.")
                );
        }

        public async Task Delete(int id)
        {
            FlightInfo flight = await _context.FlightsInfo.FindAsync(id);
            _context.Entry(flight).State = EntityState.Deleted;
            // save the changes.
            await _context.SaveChangesAsync();
        }

        public async Task GetFlights()
        {
            string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");
            SmsClient smsClient = new SmsClient(connectionString);

            string accessKey = Environment.GetEnvironmentVariable("AVIATION_STACK_API_KEY");
            IList<FlightInfo> flightsToUpdate = await _context.FlightsInfo.ToListAsync();
            if (flightsToUpdate != null){
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
                            message: ($"Flight {flight.FlightNumber} with {flight.AirlineName} on {flight.FlightDate}.\n" +
                            $"Depart from: {flight.DepartureAirport}.\n" +
                            $"Arrive at: {flight.ArrivalAirport}.\n" +
                            $"Current status is: {schema.data[0].flight_status}. You are now unsubscribed from texts.\n" +
                            $"Thank you for using Azure Skies.")
                            );
                            await Delete(flight.Id);

                            _context.Entry(flight).State = EntityState.Deleted;
                        }
                    }
                }
            }

            IList<FlightDTO> list = await _context.FlightsInfo
                .Select(flight => new FlightDTO
                {
                    FlightNumber = flight.FlightNumber,
                    Airline = flight.AirlineName,
                    Date = flight.FlightDate,
                    FlightStatus = flight.FlightStatus,
                    PhoneNumbers = flight.PhoneNumbers,
                    Arrival = flight.ArrivalAirport,
                    Departure = flight.DepartureAirport
                }).ToListAsync();

            foreach(FlightDTO flight in list)
            {
                SmsSendResult sendResult = smsClient.Send(
                    from: "+18443976066",
                    to: flight.PhoneNumbers,
                    message: ($"Flight {flight.FlightNumber} with {flight.Airline} on {flight.Date}\n" +
                    $"Depart from: {flight.Departure}\n" +
                    $"Arrive at: {flight.Arrival}\n" +
                    $"current status is: {flight.FlightStatus}.")
                    );
            }
        }
    }
}
