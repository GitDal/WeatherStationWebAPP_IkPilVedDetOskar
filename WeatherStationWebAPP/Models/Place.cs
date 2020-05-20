using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStationWebAPP.Models
{
    public class Place
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }    // gps-coordinates latitude
        public double Longitude { get; set; }   // gps-coordinates longitude
    }
}
