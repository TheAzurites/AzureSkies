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
using Newtonsoft.Json.Linq;
using Azure;
using Azure.Communication;
using Azure.Communication.Sms;
using Azure.Messaging.EventGrid;
using System.Net.Http;
using Azure.Messaging.EventGrid.SystemEvents;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

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
        public EventGridEvent Incoming([FromBody]object request)
        {
            //Deserializing the request 
            var eventGridEvent = JsonConvert.DeserializeObject<EventGridEvent[]>(request.ToString())
                .FirstOrDefault();
            //var data = eventGridEvent.Data as JObject;

            _service.AddFlight(eventGridEvent.Data.Message, eventGridEvent.Data.From);
            eventGridEvent.validationResponse = eventGridEvent.Data.validationCode;
            return eventGridEvent;
            }

        //https://localhost:44359/api/flights/flighticao/dal0380

        // POST: api/Flights
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpGet("flighticao/{flight_icao}")]
        //public async Task<ActionResult<FlightInfo>> GetFlightInfo(string flight_icao)
        //{
        //    FlightInfo flightInfo = await _service.AddFlight(flight_icao);

        //    return flightInfo;
        //}

        [HttpPut("outgoing")]
        public async Task<ActionResult<IEnumerable<FlightDTO>>> GetFlights()
        {
            var list = await _service.GetFlights();
            return Ok(list);
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlightInfo(int id)
        {
            await _service.Delete(id);
            return NoContent();
        }

        //private bool FlightInfoExists(int id)
        //{
        //    return _context.FlightInfo.Any(e => e.Id == id);
        //}
    }
}

public class EventGridEvent
{
    public string Id { get; set; }
    public string EventType { get; set; }
    public string Subject { get; set; }
    public DateTime EventTime { get; set; }
    public Data Data { get; set; }
    public string validationResponse { get; set; }
}

public class Data
{
    public string From { get; set; }
    public string Message { get; set; }
    public string validationCode { get; set; }
}


