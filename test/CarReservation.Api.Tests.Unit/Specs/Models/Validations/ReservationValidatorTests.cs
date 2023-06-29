using CarReservation.Api.Models.Validations;
using CarReservation.Api.Tests.Unit.Builders;
using FluentAssertions;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Models.Validations
{
    internal class ReservationValidatorTests
    {
        private readonly ReservationValidator _reservationValidator;

        public ReservationValidatorTests()
        {
            _reservationValidator = new ReservationValidator();
        }

        [Test]
        public async Task Validator_WhenReservationIsValid_ShouldNotReturnErrorMessage()
        {
            var invalidReservation = new ReservationBuilder()
                .WithInitialDate(DateTime.UtcNow.AddMinutes(5))
                .WithDurationInMinutes(60)
                .Build();

            var result = await _reservationValidator.ValidateAsync(invalidReservation);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        public async Task Validator_WhenInitialDateIsEmpty_ReturnsErrorMessage() 
        {
            var invalidReservation = new ReservationBuilder().WithEmptyInitialDate().Build();

            var result = await _reservationValidator.ValidateAsync(invalidReservation);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "The initial date value must be informed.");
        }

        [Test]
        [TestCase(-5)]
        [TestCase(-1)]
        [TestCase(0)]
        public async Task Validator_WhenDurationIsZeroOrNegative_ReturnsErrorMessage(int durationInMinutes)
        {
            var invalidReservation = new ReservationBuilder().WithDurationInMinutes(durationInMinutes).Build();

            var result = await _reservationValidator.ValidateAsync(invalidReservation);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "'Duration In Minutes' must be greater than '0'.");
        }

        [Test]
        [TestCase(121)]
        [TestCase(140)]
        [TestCase(300)]
        public async Task Validator_WhenDurationIsHigherThanTwoHours_ReturnErrorMessage(int durationInMinutes)
        {
            var invalidReservation = new ReservationBuilder().WithDurationInMinutes(durationInMinutes).Build();

            var result = await _reservationValidator.ValidateAsync(invalidReservation);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "The reservation should have a maximum duration of two hours.");
        }

        [Test]
        public async Task Validator_WhenInitialDateIsSetToMoreThanTwentyFourHoursFromNow_ReturnErrorMessage()
        {
            var initialDate = DateTime.UtcNow.AddHours(24).AddMinutes(1);
            var invalidReservation = new ReservationBuilder().WithInitialDate(initialDate).Build();

            var result = await _reservationValidator.ValidateAsync(invalidReservation);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "The reservation can be taken up to 24 hours ahead.");
        }
    }
}
