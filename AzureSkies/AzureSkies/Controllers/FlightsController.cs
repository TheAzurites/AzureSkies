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

namespace AzureSkies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly AzureSkiesDbContext _context;
        private readonly FlightStatusService _service;

        // DI
        public FlightsController(AzureSkiesDbContext context, FlightStatusService service)
        {
            _context = context;
            _service = service;
        }

        // GET: api/Flights/4506/Date/2021-08-09
        [HttpGet("FlightNumber/{FlightNumber}/Date/{FlightDate}")]
        public async Task<ActionResult<string>> GetFlightInfo(string FlightNumber, string FlightDate)
        {
            //var flightDTO = await _service.GetFlight(FlightNumber, FlightDate);
            //return flightDTO;

            return ($"{FlightNumber} {FlightDate}");

        }

        // PUT: api/Flights/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightInfo(int id, FlightInfo flightInfo)
        {
            if (id != flightInfo.Id)
            {
                return BadRequest();
            }
            _context.Entry(flightInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Flights
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FlightInfo>> PostFlightInfo(FlightInfo flightInfo)
        {
            _context.FlightInfo.Add(flightInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlightInfo", new { id = flightInfo.Id }, flightInfo);
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

        private bool FlightInfoExists(int id)
        {
            return _context.FlightInfo.Any(e => e.Id == id);
        }
    }
}
