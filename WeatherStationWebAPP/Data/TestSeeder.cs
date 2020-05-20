using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherStationWebAPP.Models;

namespace WeatherStationWebAPP.Data
{
    public class TestSeeder
    {
        public static void seedMe(ApplicationDbContext context)
        {
            var place = context.Places.Add(new Place()
            {
                Latitude = 56,
                Longitude = 10,
                Name = "Bissensgade 2, Aarhus C"
            }).Entity;

            context.SaveChanges();

            context.Observations.Add(new WeatherObservation()
            {
                Date = DateTime.Now,
                Humidity = 99,
                Pressure = 99,
                Temperature = 99,
                PlaceId = place.Id
            });

            context.Observations.Add(new WeatherObservation()
            {
                Date = DateTime.Now,
                Humidity = 25,
                Pressure = 22,
                Temperature = 18,
                PlaceId = place.Id
            });

            context.Observations.Add(new WeatherObservation()
            {
                Date = DateTime.Now,
                Humidity = 14,
                Pressure = 29,
                Temperature = 23,
                PlaceId = place.Id
            });
            context.Observations.Add(new WeatherObservation()
            {
                Date = DateTime.Now,
                Humidity = 50,
                Pressure = 50,
                Temperature = 50,
                PlaceId = place.Id
            });
            context.SaveChanges();
        }
    }
}
