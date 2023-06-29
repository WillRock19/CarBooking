using AutoMapper;
using CarReservation.Api.Models.Domain;
using CarReservation.Api.Models.Mapper;
using CarReservation.Api.Tests.Unit.Builders;
using FluentAssertions;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Models.Mapper
{
    internal class ReservationProfileTests
    {
        private readonly MapperConfiguration configuration;
        private readonly IMapper mapper;

        public ReservationProfileTests()
        {
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ReservationProfile>();
            });

            mapper = configuration.CreateMapper();
        }

        [Test]
        public void Profile_ShouldBeCorrectlyConfigured()
        {
            configuration.AssertConfigurationIsValid();
        }

        [Test]
        public void Profile_WhenMappingReservationRequest_ShouldSetIdToZeroAndCarIdToEmptyString()
        {
            var reservationRequest = new ReservationRequestBuilder().Build();

            var result = mapper.Map<Reservation>(reservationRequest);

            result.Id.Should().BeEmpty();
            result.CarId.Should().BeEmpty();
        }

        [Test]
        public void Profile_WhenMappingReservationRequest_ShouldMapInitialDateAndDurationInMinutes()
        {
            var expectedDuration = 30;
            var initialDate = DateTime.Now;
            var reservationRequest = new ReservationRequestBuilder()
                .WithReservationDate(initialDate)
                .WithDurationInMinutes(expectedDuration)
                .Build();

            var result = mapper.Map<Reservation>(reservationRequest);

            result.InitialDate.Should().Be(initialDate);
            result.DurationInMinutes.Should().Be(TimeSpan.FromMinutes(expectedDuration));
        }

        [Test]
        public void Profile_WhenMappingReservationRequest_ShouldSetEndingDateAsInitialDatePlusDuration()
        {
            var expectedDuration = 30;
            var initialDate = DateTime.Now;
            var expectedEndingDate = initialDate.AddMinutes(expectedDuration);
            var reservationRequest = new ReservationRequestBuilder()
                .WithReservationDate(initialDate)
                .WithDurationInMinutes(expectedDuration)
                .Build();

            var result = mapper.Map<Reservation>(reservationRequest);

            result.EndDate.Should().Be(expectedEndingDate);
        }
    }
}
