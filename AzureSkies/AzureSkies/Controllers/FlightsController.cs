﻿using System;
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
        [HttpPost("incoming/{incoming}")]
        public  void  GetFlightInfo(NewSMSFlightDTO incoming)
        {
            //var flightDTO = await _service.GetFlight(FlightNumber, FlightDate);
            //return flightDTO;

            

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
        [HttpGet("flightNumber/{flightNumber}/airline/{airline}/date/{date}")]
        public async Task<ActionResult<FlightInfo>> GetFlightInfo(string flightNumber, string airline, string date)
        {
            FlightDTO flightDTO = await _service.AddFlight(flightNumber, airline, date);
            FlightInfo flightInfo = new()
            {
                FlightDate = flightDTO.Date,
                FlightNumber = flightDTO.FlightNumber,
                Airline = flightDTO.Airline
            };
            return flightInfo;
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