using AutoMapper;
using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.Domain;
using CarReservation.Api.Services;
using CarReservation.Api.Tests.Unit.Builders;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Services
{
    public class CarServiceTests
    {
        private readonly Mock<IReservationRepository> reservationRepositoryMock;
        private readonly Mock<ICarRepository> carRepositoryMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IValidator<Reservation>> reservationValidatorMock;

        public CarServiceTests()
        {
            mapperMock = new Mock<IMapper>();
            carRepositoryMock = new Mock<ICarRepository>();
            reservationValidatorMock = new Mock<IValidator<Reservation>>();
            reservationRepositoryMock = new Mock<IReservationRepository>();
        }
        internal class ReserveCarAsync : CarServiceTests
        {
            private CarService service;

            [SetUp]
            public void SetUp()
            {                
                service = new CarService(carRepositoryMock.Object, mapperMock.Object, reservationRepositoryMock.Object, reservationValidatorMock.Object);
            }

            [Test]
            public async Task WhenReservationIsInvalid_ReturnsIdNullWithErrorMessage() 
            {
                // Arrange
                const string firstErrorMessage = "First error that occured";
                const string secondErrorMessage = "Second error that occured";
                var expectedErrorMessage = $"The reservation cannot be made. The following errors occured: \r\n* {firstErrorMessage}\r\n* {secondErrorMessage}\r\n";
                
                var fakeReservationRequest = new ReservationRequestBuilder().Build();
                var fakeInvalidReservation = new ReservationBuilder().Build();

                var validationFailures = new List<ValidationFailure> 
                {
                    new ValidationFailure("propertyName", firstErrorMessage),
                    new ValidationFailure("propertyName2", secondErrorMessage),
                };
                var validationResultWithErrors = new ValidationResult(validationFailures);

                mapperMock.Setup(x => x.Map<Reservation>(fakeReservationRequest))
                    .Returns(fakeInvalidReservation);

                reservationValidatorMock.Setup(x => x.ValidateAsync(fakeInvalidReservation, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(validationResultWithErrors);

                // Act
                var result = await service.ReserveCarAsync(fakeReservationRequest);

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

                var fakeReservationRequest = new ReservationRequestBuilder().Build();
                var fakeInvalidReservation = new ReservationBuilder().Build();
                var reservationRequestValidationResult = new ValidationResult();
                var existingCarFromDatabase = new CarBuilder().WithId(carId).Build();

                mapperMock.Setup(x => x.Map<Reservation>(fakeReservationRequest))
                    .Returns(fakeInvalidReservation);

                reservationValidatorMock.Setup(x => x.ValidateAsync(fakeInvalidReservation, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(reservationRequestValidationResult);

                reservationRepositoryMock.Setup(x => x.FindCarsReservedInInterval(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(new List<string>() { carId });

                carRepositoryMock.Setup(x => x.GetAll())
                    .Returns(new List<Car>() { existingCarFromDatabase });

                // Act
                var result = await service.ReserveCarAsync(fakeReservationRequest);

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
                var expectedSuccessMessage = $"Reservation successfully created for date {reservationInitialDate}. Your reservation ID is: {reservationId}.";

                var fakeReservationRequest = new ReservationRequestBuilder().Build();
                var fakeReservation = new ReservationBuilder().WithCarId(AvailableCarId).Build();
                var reservationRequestValidationResult = new ValidationResult();

                var unavailableCarFromDatabase = new CarBuilder().WithId(UnavailableCarId).Build();
                var availableCarFromDatabase = new CarBuilder().WithId(AvailableCarId).Build();

                mapperMock.Setup(x => x.Map<Reservation>(fakeReservationRequest))
                    .Returns(fakeReservation);

                reservationValidatorMock.Setup(x => x.ValidateAsync(fakeReservation, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(reservationRequestValidationResult);

                reservationRepositoryMock.Setup(x => x.FindCarsReservedInInterval(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(new List<string>() { UnavailableCarId });

                carRepositoryMock.Setup(x => x.GetAll())
                    .Returns(new List<Car>() { unavailableCarFromDatabase, availableCarFromDatabase });

                reservationRepositoryMock.Setup(x => x.Add(It.Is<Reservation>(reservation => reservation.CarId == AvailableCarId)))
                    .Returns(reservationId);

                // Act
                var result = await service.ReserveCarAsync(fakeReservationRequest);

                // Assert
                result.ReservationId.Should().Be(reservationId);
                result.Message.Should().Be(expectedSuccessMessage);
            }
        }
    }
}
