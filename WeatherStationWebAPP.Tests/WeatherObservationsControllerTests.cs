using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using WeatherStationWebAPP.Controllers;
using WeatherStationWebAPP.Data;
using WeatherStationWebAPP.Hubs;
using WeatherStationWebAPP.Models;

namespace WeatherStationWebAPP.Tests
{
    [TestFixture]
    public class WeatherObservationControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private IHubContext<UpdateHub> _mockHub;
        private WeatherObservationsController _uut;
        private readonly DateTime _initialTime = new DateTime(2020, 10, 10, 10, 10, 10);

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase").Options;

            _mockHub = Substitute.For<IHubContext<UpdateHub>>();

            List<WeatherObservation> weatherObservations = GetWeathersObservationsForTest();
            List<Place> places = GetPlacesForTest();

            using (var context = new ApplicationDbContext(_options))
            {
                context.Database.EnsureCreated();

                context.Observations.AddRange(weatherObservations);
                context.Places.AddRange(places);
                context.SaveChanges();
            }
        }

        [TearDown]
        public void TearDown()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                context.Database.EnsureDeleted();
            }
        }

        [Test]
        public async Task GetObservations_DbSeeded_ReturnsDtoWeatherObservationsWithCorrectDates()
        {
            // Arrange
            await using (var context = new ApplicationDbContext(_options))
            {
                _uut = new WeatherObservationsController(context, _mockHub);

                // Act
                var result = await _uut.GetObservations();
                var returnedList = result.Value.ToList();

                // Assert
                var expectedDtoWeatherObservations =
                    GetDtoWeatherObservationsForTest().OrderByDescending(o => o.Date).Take(3).ToList();

                Assert.Multiple((() =>
                {
                    Assert.That(returnedList.Count, Is.EqualTo(3));

                    Assert.That(returnedList[0].Date, Is.EqualTo(expectedDtoWeatherObservations[0].Date));
                    Assert.That(returnedList[1].Date, Is.EqualTo(expectedDtoWeatherObservations[1].Date));
                    Assert.That(returnedList[2].Date, Is.EqualTo(expectedDtoWeatherObservations[2].Date));
                }));

            }
        }

        [Test]
        public async Task GetObservations_WithDateInitialTimePlus1Day_DbSeeded_ReturnsOneDtoWeatherObservationWithCorrectDate()
        {
            // Arrange

            DateTime date = _initialTime.AddDays(1).Date; // Test data should only contain one observation with this date

            await using (var context = new ApplicationDbContext(_options))
            {
                _uut = new WeatherObservationsController(context, _mockHub);

                // Act
                var result = await _uut.GetObservations(date);
                var returnedList = result.Value.ToList();

                // Assert
                Assert.Multiple((() =>
                {
                    Assert.That(returnedList.Count, Is.EqualTo(1));
                    Assert.That(returnedList[0].Date.Date, Is.EqualTo(date));
                }));

            }
        }

        [Test]
        public async Task GetObservations_WithTimeSpanThreeHours_DbSeeded_ReturnsCorrectDtoWeatherObservations()
        {
            // Arrange
            DateTime startTime = _initialTime;
            DateTime endTime = _initialTime.AddHours(3); //Should only contain 2 weatherObservations in this time-range

            await using (var context = new ApplicationDbContext(_options))
            {
                _uut = new WeatherObservationsController(context, _mockHub);

                // Act
                var result = await _uut.GetObservations(startTime, endTime);
                var returnedList = result.Value.ToList();

                // Assert
                Assert.Multiple((() =>
                {
                    Assert.That(returnedList.Count, Is.EqualTo(2));
                    Assert.That(returnedList[0].Date, Is.GreaterThanOrEqualTo(startTime).And.LessThanOrEqualTo(endTime));
                    Assert.That(returnedList[1].Date, Is.GreaterThanOrEqualTo(startTime).And.LessThanOrEqualTo(endTime));
                }));

            }
        }

        // HELPERS

        private List<DtoWeatherObservation> GetDtoWeatherObservationsForTest()
        {
            var weatherObservations = GetWeathersObservationsForTest();
            var places = GetPlacesForTest();

            List<DtoWeatherObservation> dtoWeatherObservations = new List<DtoWeatherObservation>();

            foreach (var weatherObservation in weatherObservations)
            {
                dtoWeatherObservations.Add(new DtoWeatherObservation()
                {
                    Date = weatherObservation.Date,
                    Temperature = weatherObservation.Temperature,
                    Humidity = weatherObservation.Humidity,
                    Pressure = weatherObservation.Pressure,
                    Latitude = places.Find(p => p.Id == weatherObservation.PlaceId).Latitude,
                    Longitude = places.Find(p => p.Id == weatherObservation.PlaceId).Longitude,
                    Name = places.Find(p => p.Id == weatherObservation.PlaceId).Name
                });
            }

            return dtoWeatherObservations;
        }

        private List<WeatherObservation> GetWeathersObservationsForTest()
        {

            return new List<WeatherObservation>()
            {
                new WeatherObservation()
                {
                    Id = 1,
                    Date = _initialTime,
                    Temperature = 20.5,
                    Humidity = 2,
                    Pressure = 1.4,
                    PlaceId = 1
                },
                new WeatherObservation()
                {
                    Id = 2,
                    Date = _initialTime.AddHours(2),
                    Temperature = 21.5,
                    Humidity = 3,
                    Pressure = 1.4,
                    PlaceId = 2
                },
                new WeatherObservation()
                {
                    Id = 3,
                    Date = _initialTime.AddHours(5),
                    Temperature = 20.5,
                    Humidity = 2,
                    Pressure = 1.4,
                    PlaceId = 1
                },
                new WeatherObservation()
                {
                    Id = 4,
                    Date = _initialTime.AddDays(1).AddHours(3),
                    Temperature = 16.3,
                    Humidity = 5,
                    Pressure = 1.1,
                    PlaceId = 2
                },
            };
        }

        private List<Place> GetPlacesForTest()
        {
            return new List<Place>()
            {
                new Place()
                {
                    Id = 1,
                    Latitude = 20,
                    Longitude = 20,
                    Name = "Valhalla"
                },
                new Place()
                {
                    Id = 2,
                    Latitude = 40,
                    Longitude = 40,
                    Name = "Down under"
                }
            };
        }
    }
}