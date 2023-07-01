using AutoMapper;
using CarReservation.Api.Models.Domain;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;
using CarReservation.Api.Models.Mapper;
using CarReservation.Api.Tests.Unit.Builders.DTO;
using CarReservation.Api.Tests.Unit.Builders.Models;
using FluentAssertions;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Models.Mapper
{
    public class CarProfileTests
    {
        private readonly MapperConfiguration configuration;
        private readonly IMapper mapper;

        public CarProfileTests()
        {
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarProfile>();
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
        public void Profile_WhenCarRequestPropertiesHaveValue_MapToCarCorrectly()
        {
            var carRequest = new CreateCarRequestBuilder().Build();

            var result = mapper.Map<Car>(carRequest);

            result.Id.Should().BeEmpty();
            result.Make.Should().Be(carRequest.Make);
            result.Model.Should().Be(carRequest.Model);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        public void Profile_WhenCarRequestPropertiesAreEmptyOrNull_MapToCarCorrectly(string make, string model)
        {
            var carRequest = new CreateCarRequest() { Make = make, Model = model };

            var result = mapper.Map<Car>(carRequest);

            result.Id.Should().BeEmpty();
            result.Make.Should().BeEmpty();
            result.Model.Should().BeEmpty();
        }

        [Test]
        public void Profile_ShouldMapCarToCarResponseCorrectly()
        {
            var car = new CarBuilder()
                .WithId("C345")
                .WithMake("Some make")
                .WithModel("Some model")
                .Build();

            var result = mapper.Map<CarResponse>(car);

            result.Id.Should().Be(car.Id);
            result.Make.Should().Be(car.Make);
            result.Model.Should().Be(car.Model);
        }
    }
}
