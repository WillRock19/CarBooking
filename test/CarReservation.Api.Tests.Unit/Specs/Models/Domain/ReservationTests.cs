using CarReservation.Api.Models.Domain;
using CarReservation.Api.Tests.Unit.Builders.Models;
using FluentAssertions;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Models.Domain
{
    internal class ReservationTests
    {
        internal class Constructor : ReservationTests 
        {
            [Test]
            public void WhenReceivingInitialDateAndDuration_ShouldSetEndDate() 
            {
                var initialDate = DateTime.UtcNow;
                var durationInMinutes = TimeSpan.FromMinutes(60);
                var expectedEndDate = initialDate + durationInMinutes;

                Reservation reservation = new(Guid.NewGuid(), string.Empty, initialDate, durationInMinutes);

                reservation.EndDate.Should().Be(expectedEndDate);
            }
        }

        internal class StartsInTwentyFourHoursOrLess : ReservationTests 
        {
            [Test]
            public void WhenInitialDateIsLessOrEqualToTwentyFourHoursFromNow_ReturnsTrue()
            {
                var reservation1 = new ReservationBuilder().WithInitialDate(DateTime.UtcNow).Build();
                var reservation2 = new ReservationBuilder().WithInitialDate(DateTime.UtcNow.AddHours(23)).Build();
                var reservation3 = new ReservationBuilder().WithInitialDate(DateTime.UtcNow.AddHours(24)).Build();

                reservation1.StartsInTwentyFourHoursOrLess().Should().BeTrue();
                reservation2.StartsInTwentyFourHoursOrLess().Should().BeTrue();
                reservation3.StartsInTwentyFourHoursOrLess().Should().BeTrue();
            }

            [Test]
            public void WhenInitialDateIsHigherThanTwentyFourHoursFromNow_ReturnsFalse()
            {
                var reservation1 = new ReservationBuilder().WithInitialDate(DateTime.UtcNow.AddHours(24).AddMinutes(1)).Build();
                var reservation2 = new ReservationBuilder().WithInitialDate(DateTime.UtcNow.AddHours(25)).Build();
                var reservation3 = new ReservationBuilder().WithInitialDate(DateTime.UtcNow.AddHours(40)).Build();

                reservation1.StartsInTwentyFourHoursOrLess().Should().BeFalse();
                reservation2.StartsInTwentyFourHoursOrLess().Should().BeFalse();
                reservation3.StartsInTwentyFourHoursOrLess().Should().BeFalse();
            }
        }

        internal class HasDurationOfTwoHoursTops : ReservationTests      
        {
            [Test]
            [TestCase(1)]
            [TestCase(5)]
            [TestCase(55)]
            [TestCase(60)]
            [TestCase(80)]
            [TestCase(100)]
            [TestCase(119)]
            [TestCase(120)]
            public void WhenDurationInMinutesIsLessOrEqualToTwoHours_ReturnsTrue(int durationInMinutes)
            {
                var reservation = new ReservationBuilder().WithDurationInMinutes(durationInMinutes).Build();

                reservation.HasDurationOfTwoHoursTops().Should().BeTrue();
            }

            [Test]
            [TestCase(121)]
            [TestCase(125)]
            [TestCase(200)]
            [TestCase(345)]
            [TestCase(800)]
            [TestCase(1000)]
            public void WhenDurationIsHigherThanTwoHours_ReturnsFalse(int durationInMinutes)
            {
                var reservation = new ReservationBuilder().WithDurationInMinutes(durationInMinutes).Build();

                reservation.HasDurationOfTwoHoursTops().Should().BeFalse();
            }
        }
    }
}
