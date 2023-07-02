using AutoMapper;
using CarReservation.Api.Interfaces.Repositories;
using CarReservation.Api.Models.Domain;
using CarReservation.Api.Models.DTO.Response;
using CarReservation.Api.Models.Mapper;
using CarReservation.Api.Services;
using CarReservation.Api.Tests.Unit.Builders.DTO;
using CarReservation.Api.Tests.Unit.Builders.Models;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Services
{
    public class CarServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<ICarRepository> _carRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<Reservation>> _reservationValidatorMock;

        public CarServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _carRepositoryMock = new Mock<ICarRepository>();
            _reservationValidatorMock = new Mock<IValidator<Reservation>>();
            _reservationRepositoryMock = new Mock<IReservationRepository>();
        }

        internal class UpdateCar : CarServiceTests 
        {
            private CarService _service;

            [SetUp]
            public void SetUp()
            {
                _service = new CarService(_carRepositoryMock.Object, _mapperMock.Object, _reservationRepositoryMock.Object, _reservationValidatorMock.Object);
            }

            [Test]
            [TestCase("")]
            [TestCase(null)]
            public void WhenCardIdParameterIsNullOrEmpty_ThrowsArgumentNullError(string carId) 
            {
                Func<CarResponse> func = () => _service.UpdateCar(carId, new CreateCarRequestBuilder().Build());
                func.Should().Throw<ArgumentNullException>().WithParameterName("carId");
            }

            [Test]
            public void WhenCarRequestParameterIsNullOrEmpty_ThrowsArgumentNullError()
            {
                Func<CarResponse> func = () => _service.UpdateCar("C1", null!);
                func.Should().Throw<ArgumentNullException>().WithParameterName("updatedCarRequest");
            }

            [Test]
            public void WhenCarExistsInDatabase_ShouldCallUpdateWithMappedCarRequestContainingOriginalCarId()
            {
                // Arrange
                var carId = "C1";
                var createCarRequest = new CreateCarRequestBuilder()
                    .WithMake("Make to update")
                    .WithModel("Model to update")
                    .Build();

                var existingCar = new CarBuilder().Build();

                var carMapped = new CarBuilder()
                    .WithMake(createCarRequest.Make!)
                    .WithModel(createCarRequest.Model!)
                    .Build();

                _carRepositoryMock.Setup(x => x.GetById(carId))
                    .Returns(existingCar);

                _mapperMock.Setup(x => x.Map<Car>(createCarRequest))
                    .Returns(carMapped);

                // Act
                _service.UpdateCar(carId, createCarRequest);

                // Assert
                _carRepositoryMock.Verify(x => 
                    x.Update(It.Is<Car>(car => car.Id == carId && car.Make == carMapped.Make && car.Model == carMapped.Model)), Times.Once());
            }

            [Test]
            public void WhenCarIsCorrectlyUpdated_MapUpdatedRegisterToCarResponse() 
            {
                // Arrange
                var carId = "C1";
                var createCarRequest = new CreateCarRequestBuilder()
                    .WithMake("Make")
                    .WithModel("Model")
                    .Build();

                var existingCar = new CarBuilder().Build();

                var carMapped = new CarBuilder()
                    .WithMake(createCarRequest.Make!)
                    .WithModel(createCarRequest.Model!)
                    .Build();

                var carMappedWithId = carMapped with { Id = carId };

                _carRepositoryMock.Setup(x => x.GetById(carId))
                    .Returns(existingCar);

                _mapperMock.Setup(x => x.Map<Car>(createCarRequest))
                    .Returns(carMapped);

                _carRepositoryMock.Setup(x => x.Update(It.IsAny<Car>()))
                    .Returns(carMappedWithId);

                // Act
                _service.UpdateCar(carId, createCarRequest);

                // Assert
                _mapperMock.Verify(x => x.Map<CarResponse>(carMappedWithId), Times.Once());
            }
        }

        internal class ReserveCarAsync : CarServiceTests
        {
            private CarService _service;

            [SetUp]
            public void SetUp()
            {                
                _service = new CarService(_carRepositoryMock.Object, _mapperMock.Object, _reservationRepositoryMock.Object, _reservationValidatorMock.Object);
            }

            [Test]
            public async Task WhenReservationIsInvalid_ReturnsIdNullWithErrorMessage() 
            {
                // Arrange
                const string firstErrorMessage = "First error that occured";
                const string secondErrorMessage = "Second error that occured";
                var expectedErrorMessage = $"The reservation cannot be made. The following errors occured: \n* {firstErrorMessage}\n* {secondErrorMessage}";
                
                var fakeReservationRequest = new CreateReservationRequestBuilder().Build();
                var fakeInvalidReservation = new ReservationBuilder().Build();

                var validationFailures = new List<ValidationFailure> 
                {
                    new ValidationFailure("propertyName", firstErrorMessage),
                    new ValidationFailure("propertyName2", secondErrorMessage),
                };
                var validationResultWithErrors = new ValidationResult(validationFailures);

                _mapperMock.Setup(x => x.Map<Reservation>(fakeReservationRequest))
                    .Returns(fakeInvalidReservation);

                _reservationValidatorMock.Setup(x => x.ValidateAsync(fakeInvalidReservation, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(validationResultWithErrors);

                // Act
                var result = await _service.ReserveCarAsync(fakeReservationRequest);

                // Assert
                result.ReservationId.Should().BeNull();
                result.Message.Should().Be(expectedErrorMessage);
            }

            [Test]
            public async Task WhenThereAreNoCarAvailableToReservationDateAndDuration_ReturnsIdNullWithListOfErrors()
            {
                // Arrange
                const string carId = "C1";
                const string expectedErrorMessage = "There's no car available for the desired date and time.";

                var fakeReservationRequest = new CreateReservationRequestBuilder().Build();
                var fakeInvalidReservation = new ReservationBuilder().Build();
                var reservationRequestValidationResult = new ValidationResult();
                var existingCarFromDatabase = new CarBuilder().WithId(carId).Build();

                _mapperMock.Setup(x => x.Map<Reservation>(fakeReservationRequest))
                    .Returns(fakeInvalidReservation);

                _reservationValidatorMock.Setup(x => x.ValidateAsync(fakeInvalidReservation, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(reservationRequestValidationResult);

                _reservationRepositoryMock.Setup(x => x.FindCarsReservedDuringDate(It.IsAny<DateTime>()))
                    .Returns(new List<string>() { carId });

                _carRepositoryMock.Setup(x => x.GetAll())
                    .Returns(new List<Car>() { existingCarFromDatabase });

                // Act
                var result = await _service.ReserveCarAsync(fakeReservationRequest);

                // Assert
                result.ReservationId.Should().BeNull();
                result.Message.Should().Be(expectedErrorMessage);
            }

            [Test]
            public async Task WhenThereAreCarAvailables_CreatesReservationAndReturnIdWithSuccessMessage()
            {
                // Arrange
                const string UnavailableCarId = "C1";
                const string AvailableCarId = "C2";
                
                var reservationId = Guid.NewGuid();
                var reservationInitialDate = DateTime.UtcNow;
                var expectedSuccessMessage = $"Reservation successfully created for {reservationInitialDate}. Your reservation ID is: {reservationId}.";

                var fakeReservationRequest = new CreateReservationRequestBuilder().Build();
                var fakeReservation = new ReservationBuilder().WithCarId(AvailableCarId).Build();
                var reservationRequestValidationResult = new ValidationResult();

                var unavailableCarFromDatabase = new CarBuilder().WithId(UnavailableCarId).Build();
                var availableCarFromDatabase = new CarBuilder().WithId(AvailableCarId).Build();

                _mapperMock.Setup(x => x.Map<Reservation>(fakeReservationRequest))
                    .Returns(fakeReservation);

                _reservationValidatorMock.Setup(x => x.ValidateAsync(fakeReservation, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(reservationRequestValidationResult);

                _reservationRepositoryMock.Setup(x => x.FindCarsReservedDuringDate(It.IsAny<DateTime>()))
                    .Returns(new List<string>() { UnavailableCarId });

                _carRepositoryMock.Setup(x => x.GetAll())
                    .Returns(new List<Car>() { unavailableCarFromDatabase, availableCarFromDatabase });

                _reservationRepositoryMock.Setup(x => x.Add(It.Is<Reservation>(reservation => reservation.CarId == AvailableCarId)))
                    .Returns(reservationId);

                // Act
                var result = await _service.ReserveCarAsync(fakeReservationRequest);

                // Assert
                result.ReservationId.Should().Be(reservationId);
                result.Message.Should().Be(expectedSuccessMessage);
            }
        }

        internal class AllCarReservationsUntil : CarServiceTests 
        {
            private CarService _service;
            private IMapper _realMapper;
            private IEnumerable<Reservation> _reservationsInDatabase;

            [SetUp]
            public void SetUp()
            {
                _reservationsInDatabase = new List<Reservation>()
                {
                    new ReservationBuilder().WithCarId("C1").WithInitialDate(DateTime.UtcNow.AddHours(1)).Build(),
                    new ReservationBuilder().WithCarId("C2").WithInitialDate(DateTime.UtcNow.AddHours(2)).Build(),
                    new ReservationBuilder().WithCarId("C3").WithInitialDate(DateTime.UtcNow.AddHours(3)).Build(),
                    new ReservationBuilder().WithCarId("C4").WithInitialDate(DateTime.UtcNow.AddHours(8)).Build(),
                    new ReservationBuilder().WithCarId("C5").WithInitialDate(DateTime.UtcNow.AddHours(10)).Build(),
                    new ReservationBuilder().WithCarId("C6").WithInitialDate(DateTime.UtcNow.AddHours(15)).Build(),
                };
                _reservationRepositoryMock.Setup(x => x.GetAll()).Returns(_reservationsInDatabase);

                _realMapper = new MapperConfiguration(cfg => cfg.AddProfile<ReservationProfile>()).CreateMapper();
                _service = new CarService(_carRepositoryMock.Object, _realMapper, _reservationRepositoryMock.Object, _reservationValidatorMock.Object);
            }

            [Test]
            public void WhenLimitDateIsNull_ReturnsAllRegistersFromDatabase()
            {
                var limitDate = (DateTime?)null;

                var result = _service.AllCarReservationsUntil(limitDate);

                result.Should().HaveCount(_reservationsInDatabase.Count())
                    .And
                    .Contain(x => _reservationsInDatabase.Any(
                        reservation => reservation.Id == x.Id &&
                        reservation.CarId == x.CarId &&
                        reservation.InitialDate == x.InitialDate &&
                        reservation.DurationInMinutes.TotalMinutes == x.DurationInMinutes)
                    );
            }

            [Test]
            public void WhenLimitDateHasValue_ReturnsAllRegistersWhoseInitialDateAreLessOrEqualToLimitDate()
            {
                var limitDate = DateTime.UtcNow.AddHours(9);
                var expectedResult = _reservationsInDatabase.Where(x => x.CarId != "C5" && x.CarId != "C6");

                var result = _service.AllCarReservationsUntil(limitDate);

                result.Should().HaveCount(4)
                    .And
                    .Contain(x => expectedResult.Any(
                        reservation => reservation.Id == x.Id &&
                        reservation.CarId == x.CarId &&
                        reservation.InitialDate == x.InitialDate &&
                        reservation.DurationInMinutes.TotalMinutes == x.DurationInMinutes)
                    );
            }
        }
    }
}
