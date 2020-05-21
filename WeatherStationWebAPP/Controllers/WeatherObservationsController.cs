using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WeatherStationWebAPP.Data;
using WeatherStationWebAPP.Hubs;
using WeatherStationWebAPP.Models;

namespace WeatherStationWebAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherObservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<UpdateHub> _hubContext;

        public WeatherObservationsController(ApplicationDbContext context, IHubContext<UpdateHub> hub)
        {
            _context = context;
            _hubContext = hub;
        }


        /************** SEEDER DATABASEN ********************************************/
        [HttpGet("seed")]
        public async Task<ActionResult<IEnumerable<DtoWeatherObservation>>> Seed()
        {
            TestSeeder.seedMe(_context);

            return StatusCode(200);
        }
        /*****************************************************************************/

        // GET: api/WeatherObservations
        // Gets last 3 observations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DtoWeatherObservation>>> GetObservations()
        {
            var weatherObservations = await _context.Observations.OrderByDescending(o => o.Date).Take(3).ToListAsync();

            foreach (var weatherObservation in weatherObservations)
            {
                weatherObservation.Pressure = Math.Round(weatherObservation.Pressure, 1);
                weatherObservation.Temperature = Math.Round(weatherObservation.Temperature, 1);
            }

            return ConvertToDtoWeatherObservations(weatherObservations);

        }

        
        // GET: api/WeatherObservations/{date}
        //[HttpGet("{date}")]
        [HttpGet("{date}")]
        public async Task<ActionResult<IEnumerable<DtoWeatherObservation>>> GetObservations(DateTime date)
        {
            var weatherObservations = await _context.Observations.Where(o => o.Date.Date == date)
                .OrderByDescending(o => o.Date).ToListAsync();

            foreach (var weatherObservation in weatherObservations)
            {
                weatherObservation.Pressure = Math.Round(weatherObservation.Pressure, 1);
                weatherObservation.Temperature = Math.Round(weatherObservation.Temperature, 1);
            }

            return ConvertToDtoWeatherObservations(weatherObservations);
        }
        

        // GET: api/WeatherObservations/{date}
        [HttpGet("{startTime}/{endTime}")]
        public async Task<ActionResult<IEnumerable<DtoWeatherObservation>>> GetObservations(DateTime startTime, DateTime endTime)
        {
            var weatherObservations = await _context.Observations.Where(o => o.Date >= startTime && o.Date <= endTime)
                .OrderByDescending(o => o.Date).ToListAsync();

            foreach (var weatherObservation in weatherObservations)
            {
                weatherObservation.Pressure = Math.Round(weatherObservation.Pressure, 1);
                weatherObservation.Temperature = Math.Round(weatherObservation.Temperature, 1);
            }

            return ConvertToDtoWeatherObservations(weatherObservations);
        }

        
        // GET: api/WeatherObservations/5
        [HttpGet("id/{id}")]
        public async Task<ActionResult<WeatherObservation>> GetWeatherObservation(long id)
        {
            var weatherObservation = await _context.Observations.FindAsync(id);

            if (weatherObservation == null)
            {
                return NotFound();
            }

            weatherObservation.Pressure = Math.Round(weatherObservation.Pressure, 1);
            weatherObservation.Temperature = Math.Round(weatherObservation.Temperature, 1);

            return weatherObservation;
        }

        // POST: api/WeatherObservations
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<WeatherObservation>> PostWeatherObservation(DtoWeatherObservation dtoWeatherObservation)
        {
            var place = _context.Places.FirstOrDefault(p =>
                p.Latitude == dtoWeatherObservation.Latitude && p.Longitude == dtoWeatherObservation.Longitude);

            if (place == null)
            {
                place = _context.Places.Add(new Place()
                {
                    Name = dtoWeatherObservation.Name,
                    Latitude = dtoWeatherObservation.Latitude,
                    Longitude = dtoWeatherObservation.Longitude
                }).Entity;
                await _context.SaveChangesAsync();
            }

            var weatherObservation = new WeatherObservation()
            {
                Date = dtoWeatherObservation.Date,
                Humidity = dtoWeatherObservation.Humidity,
                Temperature = dtoWeatherObservation.Temperature,
                Pressure = dtoWeatherObservation.Pressure,
                PlaceId = place.Id
            };

            _context.Observations.Add(weatherObservation);
            await _context.SaveChangesAsync();
            
            await _hubContext.Clients.All.SendAsync("NewData", dtoWeatherObservation);

            return CreatedAtAction("GetWeatherObservation", new { id = weatherObservation.Id }, weatherObservation);
        }

        // DELETE: api/WeatherObservations/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<WeatherObservation>> DeleteWeatherObservation(long id)
        {
            var weatherObservation = await _context.Observations.FindAsync(id);
            if (weatherObservation == null)
            {
                return NotFound();
            }

            _context.Observations.Remove(weatherObservation);
            await _context.SaveChangesAsync();

            return weatherObservation;
        }

        private bool WeatherObservationExists(long id)
        {
            return _context.Observations.Any(e => e.Id == id);
        }

        private List<DtoWeatherObservation> ConvertToDtoWeatherObservations(List<WeatherObservation> weatherObservations)
        {
            List<DtoWeatherObservation> dtoWeatherObservations = new List<DtoWeatherObservation>();

            foreach (var weatherObservation in weatherObservations)
            {
                var place = _context.Places.Find(weatherObservation.PlaceId);

                dtoWeatherObservations.Add(new DtoWeatherObservation(weatherObservation, place));
            }

            return dtoWeatherObservations;
        }
    }
}
