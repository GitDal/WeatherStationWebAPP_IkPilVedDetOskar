using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStationWebAPP.Models
{
    public class DtoWeatherObservation
    {
        public DtoWeatherObservation()
        {
        }

        public DtoWeatherObservation(WeatherObservation weatherObservation, Place place)
        {
            Date = weatherObservation.Date;
            Temperature = weatherObservation.Temperature;
            Humidity = weatherObservation.Humidity;
            Pressure = weatherObservation.Pressure;

            Name = place.Name;
            Latitude = place.Latitude;
            Longitude = place.Longitude;
        }

        public DateTime Date { get; set; }
        public float Temperature { get; set; }
        public int Humidity { get; set; }
        public float Pressure { get; set; }

        // Place data
        public string Name { get; set; }
        public double Latitude { get; set; }    // gps-coordinates latitude
        public double Longitude { get; set; }   // gps-coordinates longitude
    }
}
