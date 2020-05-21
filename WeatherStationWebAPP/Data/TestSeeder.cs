using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherStationWebAPP.Models;

namespace WeatherStationWebAPP.Data
{
    public class TestSeeder
    {
        public static string _date22May = "2020-05-22";
        public static string _date23May = "2020-05-23";
        public static string _date24May = "2020-05-24";

        public static void seedMe(ApplicationDbContext context)
        {
            var place = context.Places.Add(new Place()
            {
                Latitude = 56.170295,
                Longitude = 10.199121,
                Name = "Det Kgl. Bibliotek - Aarhus"
            }).Entity;

            context.SaveChanges();

            context.Observations.Add(new WeatherObservation()
            {
                Date = DateTime.Parse(_date22May),
                Humidity = 24,
                Pressure = 12.23,
                Temperature = 17.21,
                PlaceId = place.Id
            });

            context.Observations.Add(new WeatherObservation()
            {
                Date = DateTime.Parse(_date22May),
                Humidity = 25,
                Pressure = 14.45,
                Temperature = 18.13,
                PlaceId = place.Id
            });

            context.Observations.Add(new WeatherObservation()
            {
                Date = DateTime.Parse(_date23May),
                Humidity = 20,
                Pressure = 16.91,
                Temperature = 18.73,
                PlaceId = place.Id
            });
            context.Observations.Add(new WeatherObservation()
            {
                Date = DateTime.Parse(_date24May),
                Humidity = 21,
                Pressure = 17.78,
                Temperature = 19.34,
                PlaceId = place.Id
            });
            context.SaveChanges();
        }
    }
}
