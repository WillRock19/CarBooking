using AutoMapper;
using CarReservation.Api.Models.Domain;
using CarReservation.Api.Models.DTO.Response;
using CarReservation.Api.Models.Mapper;
using CarReservation.Api.Tests.Unit.Builders.DTO;
using CarReservation.Api.Tests.Unit.Builders.Models;
using FluentAssertions;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Models.Mapper
{
    internal class ReservationProfileTests
    {
        private readonly MapperConfiguration _configuration;
        private readonly IMapper _mapper;

        public ReservationProfileTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ReservationProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void Profile_ShouldBeCorrectlyConfigured()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Test]
        public void Profile_WhenMappingReservationRequestToReservation_ShouldSetIdToZeroAndCarIdToEmptyString()
        {
            var reservationRequest = new CreateReservationRequestBuilder().Build();

            var result = _mapper.Map<Reservation>(reservationRequest);

            result.Id.Should().BeEmpty();
            result.CarId.Should().BeEmpty();
        }

        [Test]
        public void Profile_WhenMappingReservationRequestToReservation_ShouldMapInitialDateAndDurationInMinutes()
        {
            var expectedDuration = 30;
            var initialDate = DateTime.Now;
            var reservationRequest = new CreateReservationRequestBuilder()
                .WithReservationDate(initialDate)
                .WithDurationInMinutes(expectedDuration)
                .Build();

            var result = _mapper.Map<Reservation>(reservationRequest);

            result.InitialDate.Should().Be(initialDate);
            result.DurationInMinutes.Should().Be(TimeSpan.FromMinutes(expectedDuration));
        }

        [Test]
        public void Profile_WhenMappingReservationRequestToReservation_ShouldSetEndingDateAsInitialDatePlusDuration()
        {
            var expectedDuration = 30;
            var initialDate = DateTime.Now;
            var expectedEndingDate = initialDate.AddMinutes(expectedDuration);
            var reservationRequest = new CreateReservationRequestBuilder()
                .WithReservationDate(initialDate)
                .WithDurationInMinutes(expectedDuration)
                .Build();

            var result = _mapper.Map<Reservation>(reservationRequest);

            result.EndDate.Should().Be(expectedEndingDate);
        }

        [Test]
        [TestCase(5)]
        [TestCase(20)]
        [TestCase(56)]
        [TestCase(77)]
        [TestCase(90)]
        [TestCase(113)]
        [TestCase(120)]
        public void Profile_WhenMappingReservationToReservationResponse_ShouldMapPropertiesCorrectlyForMultipleDurations(int duration)
        {
            var initialDate = DateTime.Now;

            var reservation = new ReservationBuilder()
                .WithId(Guid.NewGuid())
                .WithCarId("C123")
                .WithDurationInMinutes(duration)
                .WithInitialDate(initialDate)
                .Build();

            var result = _mapper.Map<ReservationResponse>(reservation);

            result.Id.Should().Be(reservation.Id);
            result.CarId.Should().Be(reservation.CarId);
            result.InitialDate.Should().Be(reservation.InitialDate);
            result.DurationInMinutes.Should().Be(duration);
        }
    }
}
