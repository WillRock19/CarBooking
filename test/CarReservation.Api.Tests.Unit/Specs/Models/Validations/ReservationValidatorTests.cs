using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.Validations;
using CarReservation.Api.Tests.Unit.Builders.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Models.Validations
{
    internal class ReservationValidatorTests
    {
        private readonly DateTime _currentDateUtc;
        private readonly Mock<ICurrentDate> _currentDateMock;
        private readonly ReservationValidator _reservationValidator;

        public ReservationValidatorTests()
        {
            _currentDateUtc = DateTime.UtcNow;
            _currentDateMock = new Mock<ICurrentDate>();
            _reservationValidator = new ReservationValidator(_currentDateMock.Object);

            _currentDateMock.Setup(x => x.DateUtcNow()).Returns(_currentDateUtc);
        }

        [Test]
        public async Task Validator_WhenReservationIsValid_ShouldNotReturnErrorMessage()
        {
            var invalidReservation = new ReservationBuilder()
                .WithInitialDate(_currentDateUtc.AddMinutes(5))
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
            result.Errors.Should().Contain(e => e.ErrorMessage == "'Duration In Minutes' must be greater than '00:00:00'.");
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
            var initialDate = _currentDateUtc.AddHours(24).AddMinutes(1);
            var invalidReservation = new ReservationBuilder().WithInitialDate(initialDate).Build();

            var result = await _reservationValidator.ValidateAsync(invalidReservation);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "The reservation can be taken up to 24 hours ahead.");
        }
    }
}
