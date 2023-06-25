using CarReservation.Api.Models.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Models.Domain
{
    public class CarTests
    {
        [Test]
        [TestCase(0, "C0")]
        [TestCase(1, "C1")]
        [TestCase(2, "C2")]
        [TestCase(100, "C100")]
        [TestCase(17265176, "C17265176")]
        [TestCase(99836353, "C99836353")]
        public void CreateId_WhenReceivingAnyNumber_ReturnsNumberAsExpectedCarId(long number, string expectedCarId) 
        {
            Car.CreateId(number).Should().Be(expectedCarId);
        }
    }
}
