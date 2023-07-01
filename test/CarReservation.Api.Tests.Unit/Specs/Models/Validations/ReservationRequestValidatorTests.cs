using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.Validations;
using CarReservation.Api.Tests.Unit.Builders;
using CarReservation.Api.Tests.Unit.Builders.DTO;
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
