using CarReservation.Api.Models.Domain;
using CarReservation.Api.Repositories;
using CarReservation.Api.Tests.Unit.Builders.Models;
using FluentAssertions;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Repositories
{
    public class CarRepositoryTests
    {
        internal class GetAll : CarRepositoryTests
        {
            [Test]
            public void WhenRepositoryHasCars_ReturnsAllCars()
            {
                var repository = new CarRepository();

                repository.Add(new CarBuilder().Build());
                repository.Add(new CarBuilder().Build());
                repository.Add(new CarBuilder().Build());

                var allCars = repository.GetAll();

                allCars.Count().Should().Be(3);
                allCars.All(x => !string.IsNullOrEmpty(x.Id)).Should().BeTrue();
            }

            [Test]
            public void WhenRepositoryIsEmpty_ReturnsEmptyList()
            {
                var repository = new CarRepository();

                var allCars = repository.GetAll();
                allCars.Should().BeEmpty();
            }
        }

        internal class GetById : CarRepositoryTests
        {
            [Test]
            public void WhenRepositoryHasCarWithId_ReturnsCar()
            {
                var repository = new CarRepository();
                var carToInsert = new CarBuilder().WithMake("Specific make").WithModel("Specific model").Build();
                var carId = repository.Add(carToInsert);

                var carRetrieved = repository.GetById(carId);

                carRetrieved.Should().Be(carToInsert with { Id = carId });
            }

            [Test]
            public void WhenRepositoryDoesNotHaveCarWithId_ReturnsNull()
            {
                var repository = new CarRepository();
                repository.Add(new CarBuilder().WithMake("Specific make").WithModel("Specific model").Build());

                var carRetrieved = repository.GetById("SomeInexistentId");
                carRetrieved.Should().BeNull();
            }
        }

        internal class AddCar : CarRepositoryTests
        {
            [Test]
            public void WhenReceivingNullCar_ThrowsArgumentException()
            {
                var repository = new CarRepository();

                Func<string> action = () => repository.Add(null!);

                action.Should().Throw<ArgumentException>().WithParameterName("entity");
            }

            [Test]
            public void WhenAddingSingleCar_AddsCarToDatabaseWithCorrectId()
            {
                var repository = new CarRepository();
                var originalCar = new CarBuilder().Build();

                var carId = repository.Add(originalCar);

                repository.GetById(carId).Should().BeEquivalentTo(originalCar with { Id = carId });
            }

            [Test]
            public void WhenAddingMultipleCars_AddsCarsToDatabaseWithCorrectIds()
            {
                var repository = new CarRepository();
                var car1 = new CarBuilder().Build();
                var car2 = new CarBuilder().WithMake("Some Maker Name 2").Build();
                var expectedIds = new List<string>() { "C1", "C2" };

                repository.Add(car1);
                repository.Add(car2);

                repository.GetAll().All(x => expectedIds.Any(id => id == x.Id)).Should().BeTrue();
            }
        }

        internal class UpdateCar : CarRepositoryTests 
        {
            [Test]
            public void WhenReceivingNullCar_ThrowsArgumentException()
            {
                var repository = new CarRepository();

                Func<Car> action = () => repository.Update(null!);

                action.Should().Throw<ArgumentException>().WithParameterName("entity");
            }

            [Test]
            public void WhenRepositoryDoesNotHaveCarWithId_ThrowsKeyNotFoundError()
            {
                var repository = new CarRepository();
                var carToUpdate = new CarBuilder()
                    .WithId("someCarId")
                    .WithMake("Second Make")
                    .WithModel("Second Model")
                    .Build();

                Action action = () => repository.Update(carToUpdate);
                action.Should().Throw<KeyNotFoundException>().WithMessage($"Operation cannot be completed. There's no card with id '{carToUpdate.Id}'.");
            }

            [Test]
            public void WhenReceivingValidCar_UpdatesCarValueInDatabase()
            {
                var repository = new CarRepository();
                var originalCar = new CarBuilder()
                    .WithMake("First Make")
                    .WithModel("First Model")
                    .Build();

                var originalCarId = repository.Add(originalCar);

                var originalCarChanged = new CarBuilder()
                    .WithId(originalCarId)
                    .WithMake("Second Make")
                    .WithModel("Second Model")
                    .Build();

                var updatedCar = repository.Update(originalCarChanged);
                updatedCar.Should().BeEquivalentTo(originalCarChanged);
                repository.GetAll().Should().NotContain(x => x.Make == originalCar.Make && x.Model == originalCar.Model);
            }
        }

        internal class Delete : CarRepositoryTests 
        {
            [Test]
            public void WhenRepositoryHasCarWithId_RemovesTheCar()
            {
                var repository = new CarRepository();
                var carToInsert = new CarBuilder().WithMake("Specific make").WithModel("Specific model").Build();
                var carId = repository.Add(carToInsert);

                repository.Delete(carId);

                repository.GetAll().Should().BeEmpty();
            }

            [Test]
            public void WhenRepositoryDoesNotHaveCarWithId_ThrowsKeyNotFoundError()
            {
                var repository = new CarRepository();
                var idToDelete = "SomeInexistentId";

                Action action = () => repository.Delete(idToDelete);
                action.Should().Throw<KeyNotFoundException>().WithMessage($"Operation cannot be completed. There's no card with id '{idToDelete}'.");
            }
        }
    }
}
