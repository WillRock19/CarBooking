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

        internal class FindCarsReservedDuringDate : ReservationRepositoryTests 
        {
            private readonly DateTime _currentDateToUse = DateTime.UtcNow;

            [Test]
            public void WhenDatabaseIsEmpty_ReturnsEmptyList()
            {
                var repository = new ReservationRepository();

                var carIds = repository.FindCarsReservedDuringDate(_currentDateToUse);

                carIds.Should().BeEmpty();
            }

            [Test]
            public void WhenReservationStartsPriorAndEndsBeforeInformedDate_ReturnsEmptyList()
            {
                // Arrange
                var reservationDate = _currentDateToUse.AddMinutes(5);
                var reservationDurationInMinutes = 30;
                var dateToSearchFor = reservationDate.AddMinutes(reservationDurationInMinutes + 1);

                var repository = new ReservationRepository();
                var reservation = new ReservationBuilder()
                    .WithInitialDate(reservationDate)
                    .WithDurationInMinutes(reservationDurationInMinutes)
                    .Build();

                repository.Add(reservation);

                // Act
                var carIds = repository.FindCarsReservedDuringDate(dateToSearchFor);

                // Assert
                carIds.Should().BeEmpty();
            }

            [Test]
            public void WhenReservationStartsPriorAndEndsAfterInformedDate_ReturnsTheReservationCarId()
            {
                // Arrange
                var durationInMinutes = 80;
                var dateToSearch = _currentDateToUse.AddMinutes(durationInMinutes - 5);
                var repository = new ReservationRepository();

                var reservation1 = new ReservationBuilder()
                    .WithCarId("C1")
                    .WithInitialDate(_currentDateToUse)
                    .WithDurationInMinutes(durationInMinutes)
                    .Build();

                repository.Add(reservation1);

                // Act
                var result = repository.FindCarsReservedDuringDate(dateToSearch).ToList();

                // Assert
                result.Should().NotBeEmpty().And.Contain(x => x == reservation1.CarId);
            }

            [Test]
            public void WhenReservationStartsPriorAndEndsExactlyAtTheInformedDate_ReturnsTheReservationCarId()
            {
                // Arrange
                var durationInMinutes = 80;
                var dateToSearch = _currentDateToUse.AddMinutes(durationInMinutes);
                var repository = new ReservationRepository();

                var reservation1 = new ReservationBuilder()
                    .WithCarId("C1")
                    .WithInitialDate(_currentDateToUse)
                    .WithDurationInMinutes(durationInMinutes)
                    .Build();

                repository.Add(reservation1);

                // Act
                var result = repository.FindCarsReservedDuringDate(dateToSearch).ToList();

                // Assert
                result.Should().NotBeEmpty().And.Contain(x => x == reservation1.CarId);
            }

            [Test]
            public void WhenMultipleReservationsStartsPriorAndEndsAfterInformedDate_ReturnsCarIdThoseReservations()
            {
                // Arrange
                var repository = new ReservationRepository();
                var dateToSearch = _currentDateToUse.AddMinutes(40);

                var reservationEndsBeforeDate = new ReservationBuilder()
                    .WithInitialDate(_currentDateToUse.AddMinutes(1))
                    .WithDurationInMinutes(30)
                    .Build();

                var reservationEndsAfterDate1 = new ReservationBuilder()
                    .WithInitialDate(_currentDateToUse.AddMinutes(5))
                    .WithDurationInMinutes(40)
                    .Build();

                var reservationEndsAfterDate2 = new ReservationBuilder()
                    .WithInitialDate(_currentDateToUse.AddMinutes(1))
                    .WithDurationInMinutes(55)
                    .Build();

                var reservationEndsAfterDate3 = new ReservationBuilder()
                    .WithInitialDate(_currentDateToUse.AddMinutes(5))
                    .WithDurationInMinutes(60)
                    .Build();

                repository.Add(reservationEndsBeforeDate);
                repository.Add(reservationEndsAfterDate1);

                repository.Add(reservationEndsAfterDate2);
                repository.Add(reservationEndsAfterDate3);

                // Act
                var carIds = repository.FindCarsReservedDuringDate(dateToSearch);

                // Assert
                carIds.Should().HaveCount(3)
                    .And
                    .Contain(x => x == reservationEndsAfterDate1.CarId)
                    .And
                    .Contain(x => x == reservationEndsAfterDate2.CarId)
                    .And
                    .Contain(x => x == reservationEndsAfterDate3.CarId);
            }

            [Test]
            public void WhenReservationStartsAndEndsAfterInformedDate_ReturnsEmptyList()
            {
                // Arrange
                var repository = new ReservationRepository();
                var dateToSearch = _currentDateToUse;

                var reservation1 = new ReservationBuilder()
                    .WithCarId("C1")
                    .WithInitialDate(_currentDateToUse.AddMinutes(5))
                    .WithDurationInMinutes(80)
                    .Build();

                repository.Add(reservation1);

                // Act
                var result = repository.FindCarsReservedDuringDate(dateToSearch).ToList();

                // Assert
                result.Should().BeEmpty();
            }

            [Test]
            public void WhenMultipleReservationsStartPriorAndEndsBeforeAndStartsAfterAndEndAfterInformedDate_ReturnsEmptyList()
            {
                // Arrange
                var repository = new ReservationRepository();
                var dateToSearch = _currentDateToUse.AddMinutes(30);

                var reservationStartsPriorAndEndBefore1 = new ReservationBuilder()
                    .WithInitialDate(_currentDateToUse)
                    .WithDurationInMinutes(20)
                    .Build();

                var reservationStartsPriorAndEndBefore2 = new ReservationBuilder()
                    .WithInitialDate(dateToSearch.AddHours(-2))
                    .WithDurationInMinutes(60)
                    .Build();

                var reservationStartsAfterEndsAfterDate1 = new ReservationBuilder()
                    .WithInitialDate(dateToSearch.AddMinutes(1))
                    .WithDurationInMinutes(40)
                    .Build();

                var reservationStartsAfterEndsAfterDate2 = new ReservationBuilder()
                  .WithInitialDate(dateToSearch.AddHours(1))
                  .WithDurationInMinutes(60)
                  .Build();

                repository.Add(reservationStartsAfterEndsAfterDate1);
                repository.Add(reservationStartsAfterEndsAfterDate2);

                repository.Add(reservationStartsPriorAndEndBefore1);
                repository.Add(reservationStartsPriorAndEndBefore2);

                // Act
                var carIds = repository.FindCarsReservedDuringDate(dateToSearch);

                // Assert
                carIds.Should().BeEmpty();
            }

            [Test]
            public void WhenMultipleReservationsStartPriorAndEndAfterAndStartsAfterAndEndAfterInformedDate_ReturnsOnlyTheOnesThatStartsPrior()
            {
                // Arrange
                var repository = new ReservationRepository();
                var dateToSearch = _currentDateToUse;

                var reservationStartsPriorEndsAfter1 = new ReservationBuilder()
                    .WithInitialDate(dateToSearch.AddMinutes(-10))
                    .WithDurationInMinutes(20)
                    .Build();

                var reservationStartsPriorEndsAfter2 = new ReservationBuilder()
                    .WithInitialDate(dateToSearch.AddMinutes(-30))
                    .WithDurationInMinutes(120)
                    .Build();

                var reservationStartsAfterEndsAfter1 = new ReservationBuilder()
                    .WithInitialDate(dateToSearch.AddMinutes(1))
                    .WithDurationInMinutes(40)
                    .Build();

                var reservationStartsAfterEndsAfter2 = new ReservationBuilder()
                  .WithInitialDate(dateToSearch.AddHours(1))
                  .WithDurationInMinutes(60)
                  .Build();

                repository.Add(reservationStartsAfterEndsAfter1);
                repository.Add(reservationStartsAfterEndsAfter2);

                repository.Add(reservationStartsPriorEndsAfter1);
                repository.Add(reservationStartsPriorEndsAfter1);

                // Act
                var carIds = repository.FindCarsReservedDuringDate(dateToSearch);

                // Assert
                carIds.Should().HaveCount(2)
                    .And
                    .Contain(x => x == reservationStartsPriorEndsAfter1.CarId)
                    .And
                    .Contain(x => x == reservationStartsPriorEndsAfter1.CarId);
            }
        }
    }
}
