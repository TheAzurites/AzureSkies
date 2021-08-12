using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AzureSkies.Data;
using AzureSkies.Models;
using AzureSkies.Services;
using AzureSkies.DTO;
using AzureSkies.Interfaces;
using Newtonsoft.Json;
using Azure;
using Azure.Communication;
using Azure.Communication.Sms;
using Azure.Messaging.EventGrid;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Azure.Messaging.EventGrid.SystemEvents;
using System.Net;

namespace AzureSkies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly AzureSkiesDbContext _context;
        private readonly IFlightStatus _service;

        // DI
        public FlightsController(AzureSkiesDbContext context, IFlightStatus service)
        {
            _context = context;
            _service = service;
        }

        // GET: https://azureskieslatest.azurewebsites.net/api/flights/incoming
        [HttpPost("incoming")]
        public HttpResponseMessage GetFlightInfo(HttpRequestMessage incoming)
        {
            //var flightDTO = await _service.GetFlight(FlightNumber, FlightDate);
            //return flightDTO;

            //string connectionString = Environment.GetEnvironmentVariable("CommunicationServiceConnection");

            //SmsClient smsClient = new SmsClient(connectionString);

            //SmsSendResult sendResult = smsClient.Send(
            //    from: "+18443976066",
            //    to: "+12158507772",
            //    message: "URL incoming"
            //    );
            var requestContent = incoming.Content.ReadAsStream();

            EventGridEvent[] egEvents = EventGridEvent.ParseMany(BinaryData.FromStream(requestContent));

            foreach (EventGridEvent egEvent in egEvents)
            {
                // If the event is a system event, TryGetSystemEventData will return the deserialized system event
                if (egEvent.TryGetSystemEventData(out object systemEvent))
                {
                    switch (systemEvent)
                    {
                        case SubscriptionValidationEventData subscriptionValidated:
                            return incoming.CreateResponse(HttpStatusCode.OK);
                        case StorageBlobCreatedEventData blobCreated:
                            Console.WriteLine(blobCreated.BlobType);
                            break;
                        // Handle any other system event type
                        default:
                            Console.WriteLine(egEvent.EventType);
                            // we can get the raw Json for the event using Data
                            Console.WriteLine(egEvent.Data.ToString());
                            break;
                    }
                }
                else
                {
                    //switch (egEvent.EventType)
                    //{
                        //case "MyApp.Models.CustomEventType":
                            //TestPayload deserializedEventData = egEvent.Data.ToObjectFromJson<TestPayload>();
                            //Console.WriteLine(deserializedEventData.Name);
                            //break;
                        // Handle any other custom event type
                        //default:
                            Console.Write(egEvent.EventType);
                            Console.WriteLine(egEvent.Data.ToString());
                            break;
                    //}
                }
                return null;
            }
        }

        // PUT: api/Flights/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutFlightInfo(int id, FlightInfo flightInfo)
        //{
        //    if (id == 0)
        //    {
        //        return BadRequest();
        //    }
        //    _context.Entry(flightInfo).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlightInfoExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //https://localhost:44359/api/flights/flighticao/dal0380

        // POST: api/Flights
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpGet("flighticao/{flight_icao}")]
        public async Task<ActionResult<FlightInfo>> GetFlightInfo(string flight_icao)
        {
            FlightInfo flightInfo = await _service.AddFlight(flight_icao);

            return flightInfo;
        }

        [HttpGet("helloworld")]
        public void SayHello()
        {
            Console.WriteLine("Hello World");
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlightInfo(int id)
        {
            var flightInfo = await _context.FlightInfo.FindAsync(id);
            if (flightInfo == null)
            {
                return NotFound();
            }

            _context.FlightInfo.Remove(flightInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //private bool FlightInfoExists(int id)
        //{
        //    return _context.FlightInfo.Any(e => e.Id == id);
        //}
    }
}
