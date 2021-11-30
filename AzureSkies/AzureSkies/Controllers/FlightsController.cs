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

        // Dependency Injection
        public FlightsController(AzureSkiesDbContext context, IFlightStatus service)
        {
            _service = service;
        }

        // GET: https://azureskieslatest.azurewebsites.net/api/flights/incoming
        [HttpPost("incoming")]
        public async Task<ActionResult<EventGridEvent>> Incoming([FromBody]object request)
        {
            //Deserializing the request 
            var eventGridEvent = JsonConvert.DeserializeObject<EventGridEvent[]>(request.ToString())
                .FirstOrDefault();
            //var data = eventGridEvent.Data as JObject;
            if (eventGridEvent.Data.From != null)
            {
                await _service.AddFlight(eventGridEvent.Data.Message, eventGridEvent.Data.From);
            }
                eventGridEvent.validationResponse = eventGridEvent.Data.validationCode;
                return Ok(eventGridEvent);
            }

        [HttpGet("outgoing")]
        public async Task GetFlights()
        {
            await _service.GetFlights();
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlightInfo(int id)
        {
            await _service.Delete(id);
            return NoContent();
        }
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


