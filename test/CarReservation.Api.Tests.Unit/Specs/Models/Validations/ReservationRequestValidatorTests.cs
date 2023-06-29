using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.Validations;
using CarReservation.Api.Tests.Unit.Builders;
using FluentAssertions;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Models.Validations
{
    public class ReservationRequestValidatorTests
    {
        private readonly ReservationRequestValidator _reservationRequestValidator;

        public ReservationRequestValidatorTests()
        {
            _reservationRequestValidator = new ReservationRequestValidator();
        }

        [Test]
        public void Validator_WhenReservationRequestIsValid_ShouldNotReturnErrorMessage()
        {
            var reservationRequest = new ReservationRequestBuilder().Build();

            var result = _reservationRequestValidator.Validate(reservationRequest);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        public void Validator_WhenReservationDateIsEmpty_ReturnsErrorMessage()
        {
            var propertyName = nameof(ReservationRequest.ReservationDate);
            var reservationRequest = new ReservationRequestBuilder().WithEmptyReservationDate().Build();

            var result = _reservationRequestValidator.Validate(reservationRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"'Reservation Date' must not be empty.");
        }

        [Test]
        public void Validator_WhenDurationInMinutesIsEmpty_ReturnsErrorMessage()
        {
            var propertyName = nameof(ReservationRequest.DurationInMinutes);
            var reservationRequest = new ReservationRequestBuilder().WithEmptyDurationInMinutes().Build();

            var result = _reservationRequestValidator.Validate(reservationRequest);

            result.Errors
                .Should()
                .Contain(x => x.PropertyName == propertyName && x.ErrorMessage == $"'Duration In Minutes' must be greater than '0'.");
        }
    }
}
