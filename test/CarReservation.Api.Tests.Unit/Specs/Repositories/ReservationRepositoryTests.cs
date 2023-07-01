using CarReservation.Api.Models.Domain;
using CarReservation.Api.Repositories;
using CarReservation.Api.Tests.Unit.Builders.Models;
using FluentAssertions;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Repositories
{
    internal class ReservationRepositoryTests
    {
        internal class Add : ReservationRepositoryTests 
        {
            [Test]
            public void WhenReceivingNullReservation_ThrowsArgumentException()
            {
                var repository = new ReservationRepository();

                Func<Guid> action = () => repository.Add(null!);

                action.Should().Throw<ArgumentException>().WithParameterName("entity");
            }

            [Test]
            public void WhenAddingSingleReservation_AddsReservationWithCorrectId()
            {
                var repository = new ReservationRepository();
                var originalReservation = new ReservationBuilder().Build();

                var reservationId = repository.Add(originalReservation);

                repository.GetById(reservationId).Should().BeEquivalentTo(originalReservation with { Id = reservationId });
            }

            [Test]
            public void WhenAddingMultipleReservations_AddsReservationsToDatabaseWithCorrectIds()
            {
                var repository = new ReservationRepository();
                var reservation1 = new ReservationBuilder().WithCarId("C13").Build();
                var reservation2 = new ReservationBuilder().WithCarId("C123").Build();

                var reservationId1 = repository.Add(reservation1);
                var reservationId2 = repository.Add(reservation2);

                repository.GetById(reservationId1).Should().BeEquivalentTo(reservation1 with { Id = reservationId1 });
                repository.GetById(reservationId2).Should().BeEquivalentTo(reservation2 with { Id = reservationId2 });
            }
        }

        internal class GetAll : ReservationRepositoryTests 
        {
            [Test]
            public void WhenDatabaseIsEmpty_ReturnsEmptyList()
            {
                var repository = new ReservationRepository();

                repository.GetAll().Should().BeEmpty();
            }

            [Test]
            public void WhenDatabaseHasReservations_ReturnsAllReservations()
            {
                var repository = new ReservationRepository();

                var firstReservation = new ReservationBuilder().Build();
                var firstReservationId = repository.Add(firstReservation);

                var secondReservation = new ReservationBuilder().Build();
                var secondReservationId = repository.Add(secondReservation);

                var thirdReservation = new ReservationBuilder().Build();
                var thirdReservationId = repository.Add(thirdReservation);

                repository.GetAll().Should().BeEquivalentTo(new List<Reservation>
                {
                    firstReservation with { Id = firstReservationId },
                    secondReservation with { Id = secondReservationId },
                    thirdReservation with { Id = thirdReservationId },
                });
            }
        }

        internal class FindCarsReservedInInterval : ReservationRepositoryTests 
        {
            private readonly DateTime intervalBeginning;
            private readonly DateTime intervalEnding;

            public FindCarsReservedInInterval()
            {
                intervalBeginning = DateTime.Now.AddHours(2);
                intervalEnding = intervalBeginning.AddHours(2);
            }

            [Test]
            public void WhenDatabaseIsEmpty_ReturnsEmptyList()
            {
                var repository = new ReservationRepository();

                var carIds = repository.FindCarsReservedInInterval(intervalBeginning, intervalEnding);
                
                carIds.Should().BeEmpty();
            }

            [Test]
            public void WhenThereAreNoReservationsInTheInformedInterval_ReturnsEmptyList() 
            {
                var repository = new ReservationRepository();
                var reservation = ReservationOutsideInterval();

                repository.Add(reservation);

                var carIds = repository.FindCarsReservedInInterval(intervalBeginning, intervalEnding);
                carIds.Should().BeEmpty();
            }

            [Test]
            public void WhenThereIsSingleReservationInTheInformedInterval_ReturnsCarIdOfReservation()
            {
                var repository = new ReservationRepository();
                var reservationInInterval = ReservationInsideInterval();
                var reservationOutsideInterval = ReservationOutsideInterval();

                repository.Add(reservationInInterval);
                repository.Add(reservationOutsideInterval);

                var carIds = repository.FindCarsReservedInInterval(intervalBeginning, intervalEnding);
                
                carIds.Should().HaveCount(1).And.Contain(x => x == reservationInInterval.CarId);
            }

            [Test]
            public void WhenThereIsMultipleReservationsInTheInformedInterval_ReturnsCarIdOfSpecificReservations()
            {
                var repository = new ReservationRepository();
                var reservationInInterval1 = ReservationInsideInterval();
                var reservationInInterval2 = ReservationInsideInterval();
                var reservationOutsideInterval1 = ReservationOutsideInterval();
                var reservationOutsideInterval2 = ReservationOutsideInterval();

                repository.Add(reservationInInterval1);
                repository.Add(reservationOutsideInterval1);

                repository.Add(reservationInInterval2);
                repository.Add(reservationOutsideInterval2);

                var carIds = repository.FindCarsReservedInInterval(intervalBeginning, intervalEnding);

                carIds.Should().HaveCount(2)
                    .And
                    .Contain(x => x == reservationInInterval1.CarId)
                    .And
                    .Contain(x => x == reservationInInterval2.CarId);
            }

            [Test]
            public void WhenAllReservationsAreInTheInformedInterval_ReturnsCarIdOfAllReservations()
            {
                var repository = new ReservationRepository();
                var reservationInInterval1 = ReservationInsideInterval();
                var reservationInInterval2 = ReservationInsideInterval();
                var reservationInInterval3 = ReservationInsideInterval();

                repository.Add(reservationInInterval1);
                repository.Add(reservationInInterval2);
                repository.Add(reservationInInterval3);

                var carIds = repository.FindCarsReservedInInterval(intervalBeginning, intervalEnding);

                carIds.Should().HaveCount(3)
                    .And
                    .Contain(x => x == reservationInInterval1.CarId)
                    .And
                    .Contain(x => x == reservationInInterval2.CarId)
                    .And
                    .Contain(x => x == reservationInInterval3.CarId);
            }

            private Reservation ReservationInsideInterval() 
            {
                var initialDateInsideDesiredInterval = intervalBeginning.AddMinutes(5);

                return new ReservationBuilder()
                    .WithInitialDate(initialDateInsideDesiredInterval)
                    .WithDurationInMinutes(20)
                    .Build();
            }

            private Reservation ReservationOutsideInterval()
            {
                var initialDateOutsideDesiredInterval = intervalEnding.AddHours(1);

                return new ReservationBuilder()
                    .WithInitialDate(initialDateOutsideDesiredInterval)
                    .WithDurationInMinutes(120)
                    .Build();
            }
        }
    }
}
