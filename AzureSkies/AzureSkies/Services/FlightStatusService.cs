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
using System.Text.RegularExpressions;

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
            string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");
            SmsClient smsClient = new SmsClient(connectionString);

            // Create Regex pattern that message must follow, else return error.
            Regex rg = new Regex(@"[a-zA-Z]{2}[a-zA-Z]?[0-9]{3}[0-9]?");
            if (message.ToLower() == "landed")
            {
                await IfMsgIsLanded(phoneNumber);
                return;
            }
            else if (rg.IsMatch(message) == false)
            {
                SmsSendResult sendDuplicateMsg = smsClient.Send(
                    from: "+18443976066",
                    to: phoneNumber,
                    message: "Please enter a valid Flight ICAO Number.\n" +
                    "Accepted Formats: ABC123, ABC1234, AB123, AB1234\n"
                    );
                return;
            }

            IList<FlightInfo> flights = await _context.FlightsInfo.ToListAsync();
            foreach(FlightInfo flight in flights)
            {
                if (message.ToUpper() == flight.FlightIcao.ToUpper())
                {
                    // Don't do anything if the phone number already exists in the flight.
                    if (flight.PhoneNumbers.Contains(phoneNumber))
                    {
                        SmsSendResult sendDuplicateMsg = smsClient.Send(
                            from: "+18443976066",
                            to: phoneNumber,
                            message: ($"Whoa! You have already subscribed to this flight.\n" +
                            $"Text LANDED to unsubscribe.")
                            );
                        return;
                    }
                }
            }

            string accessKey = Environment.GetEnvironmentVariable("AVIATION_STACK_API_KEY");
            string path = $"http://api.aviationstack.com/v1/flights?access_key={accessKey}&flight_icao={message}&limit=1";

            HttpResponseMessage response = await client.GetAsync(path);
            // Establishing a connection to the SMS Service for sending texts to users.
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
                message: ($"Welcome to Azure Skies.\n" +
                    $"Flight {flightInfo.FlightIcao} with {flightInfo.AirlineName} on {flightInfo.FlightDate}:\n" +
                    $"Depart From: {flightInfo.DepartureAirport}.\n" +
                    $"Arrive At: {flightInfo.ArrivalAirport}.\n" +
                    $"Flight Status: {flightInfo.FlightStatus.ToUpper()}.\n" +
                    $"Subscribed to automatic flight updates.\n" +
                    $"Text LANDED to immediately unsubscribe from all flight events.")
                );
        }

        public async Task IfMsgIsLanded(string phoneNumber)
        {
            string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");
            SmsClient smsClient = new SmsClient(connectionString);

            bool found = false;
            IList<FlightInfo> flights = await _context.FlightsInfo.ToListAsync();
            foreach(FlightInfo flight in flights)
            {
                if (flight.PhoneNumbers == phoneNumber)
                {
                    SmsSendResult sendStop = smsClient.Send(
                    from: "+18443976066",
                    to: phoneNumber,
                    message: ($"Unsubscribed from flight updates for flight: {flight.FlightIcao}.\n" +
                    $"Thank you for using Azure Skies.")
                    );
                    await Delete(flight.Id);
                    found = true;
                }
            }
            if (found == false)
            {
                SmsSendResult sendNotFound = smsClient.Send(
                from: "+18443976066",
                to: phoneNumber,
                message: "Unable to find flight associated with this number.\n" +
                "You have yet to subscribe to any flights."
                );
                return;
            }
            return;
        }

        public async Task Delete(int id)
        {
            FlightInfo flight = await _context.FlightsInfo.FindAsync(id);
            _context.Entry(flight).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public async Task GetFlights()
        {
            string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");
            SmsClient smsClient = new SmsClient(connectionString);

            string accessKey = Environment.GetEnvironmentVariable("AVIATION_STACK_API_KEY");
            IList<FlightInfo> flightsToUpdate = await _context.FlightsInfo.ToListAsync();
            if (flightsToUpdate.Count > 0)
            {
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
                            message: ($"Flight {flight.FlightNumber} with {flight.AirlineName} on {flight.FlightDate}:\n" +
                            $"Depart From: {flight.DepartureAirport}.\n" +
                            $"Arrive At: {flight.ArrivalAirport}.\n" +
                            $"Flight Status: {schema.data[0].flight_status.ToUpper()}.\n" +
                            $"Unsubscribed. Thank you for using Azure Skies.")
                            );
                            await Delete(flight.Id);

                            _context.Entry(flight).State = EntityState.Deleted;
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
                        message: ($"Flight {flight.FlightNumber} with {flight.Airline} on {flight.Date}:\n" +
                        $"Depart from: {flight.Departure}\n" +
                        $"Arrive at: {flight.Arrival}\n" +
                        $"current status is: {flight.FlightStatus.ToUpper()}.\n" +
                        $"Text LANDED to unsubscribe.")
                        );
                }
            }
        }
    }
}
