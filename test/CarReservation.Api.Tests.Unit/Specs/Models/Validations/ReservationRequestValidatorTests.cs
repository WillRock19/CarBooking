using AutoMapper;
using CarReservation.Api.Infraestructure;
using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.Validations;
using CarReservation.Api.Tests.Unit.Builders.DTO;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Models.Validations
{
    public class ReservationRequestValidatorTests
    {
        private readonly DateTime _currentDate;
        private readonly ReservationRequestValidator _reservationRequestValidator;
        private readonly Mock<ICurrentDate> _currentDateMock;
        private readonly Mock<IMapper> _mapperMock;

        public ReservationRequestValidatorTests()
        {
            _currentDate = DateTime.UtcNow;
            _currentDateMock = new Mock<ICurrentDate>();

            _currentDateMock.Setup(x => x.DateUtcNow())
                .Returns(_currentDate);

            _reservationRequestValidator = new ReservationRequestValidator(_currentDateMock.Object);
        }

        [Test]
        public void Validator_WhenReservationRequestIsValid_ShouldNotReturnErrorMessage()
        {
            var reservationRequest = new CreateReservationRequestBuilder().Build();

            var result = _reservationRequestValidator.Validate(reservationRequest);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        public void Validator_WhenReservationDateIsEmpty_ReturnsErrorMessage()
        {
            var propertyName = nameof(CreateReservationRequest.ReservationDate);
            var reservationRequest = new CreateReservationRequestBuilder().WithEmptyReservationDate().Build();

            var result = _reservationRequestValidator.Validate(reservationRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"'Reservation Date' must not be empty.");
        }

        [Test]
        public void Validator_WhenReservationIsNotForFiveMinutesFromNow_ReturnsErrorMessage()
        {
            var propertyName = nameof(CreateReservationRequest.ReservationDate);
            var pastDate = _currentDate.AddMinutes(5).AddMilliseconds(-1);

            var reservationRequest = new CreateReservationRequestBuilder()
                .WithReservationDate(pastDate)
                .Build();

            var result = _reservationRequestValidator.Validate(reservationRequest);
            result.Errors
            .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"'Reservation Date' must be at least five minutes from now.");
        }

        [Test]
        public void Validator_WhenDurationInMinutesIsEmpty_ReturnsErrorMessage()
        {
            var propertyName = nameof(CreateReservationRequest.DurationInMinutes);
            var reservationRequest = new CreateReservationRequestBuilder().WithEmptyDurationInMinutes().Build();

            var result = _reservationRequestValidator.Validate(reservationRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"'Duration In Minutes' must be greater than '0'.");
        }
    }
}
