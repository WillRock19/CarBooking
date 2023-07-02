using CarReservation.Api.Controllers;
using CarReservation.Api.Interfaces.Infraestructure;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Tests.Unit.Builders.DTO;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CarReservation.Api.Tests.Unit.Specs.Controllers
{
    public class CarControllerTests
    {
        private readonly Mock<IValidator<CreateCarRequest>> _carRequestValidatorMock;
        private readonly Mock<IValidator<CreateReservationRequest>> _reservationRequestValidatorMock;
        private readonly Mock<ICarService> _carServiceMock;
        private readonly CarController _carController;

        public CarControllerTests()
        {
            _carRequestValidatorMock = new Mock<IValidator<CreateCarRequest>>();
            _reservationRequestValidatorMock = new Mock<IValidator<CreateReservationRequest>>();
            _carServiceMock = new Mock<ICarService>();

            _carController = new CarController(_carServiceMock.Object, _carRequestValidatorMock.Object, _reservationRequestValidatorMock.Object);
        }

        internal class GetById : CarControllerTests
        {
            [Test]
            [TestCase(null)]
            [TestCase("")]
            public void WhenCarIdIsNullOrEmpty_ReturnsBadRequestWithMessage(string carId)
            {
                var result = _carController.GetById(carId);
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be("Query parameter car_id cannot be null or empty.");
            }
        }

        internal class AddCar : CarControllerTests 
        {
            [Test]
            public async Task WhenCarRequestIsNull_ReturnsBadRequestWithMessage()
            {
                var result = await _carController.CreateCar(null!);
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be("Request content cannot be null.");
            }

            [Test]
            public async Task WhenCarRequestIsInvalid_ReturnsUnprocessableEntityWithValidationErrorsInMessage()
            {
                var carRequest = new CreateCarRequestBuilder().Build();
                var error1 = "Error that happened with SomeProperty";
                var error2 = "Error that happened with SomeProperty2";
                var validationFail = new List<ValidationFailure>()
                {
                    new ValidationFailure("SomeProperty", error1),
                    new ValidationFailure("SomeProperty2", error2)
                };
                var validationResultWithError = new ValidationResult(validationFail);
                var expectedListOfErrors = new List<string>() { error1, error2 };

                _carRequestValidatorMock.Setup(x => x.ValidateAsync(carRequest, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(validationResultWithError);

                var result = await _carController.CreateCar(carRequest);
                
                var resultAsBadRequest = (UnprocessableEntityObjectResult)result;
                resultAsBadRequest.StatusCode.Should().Be(422);
                resultAsBadRequest.Value.As<IEnumerable<string>>().ToList().Should().BeEquivalentTo(expectedListOfErrors);
            }
        }

        internal class UpdateCar : CarControllerTests
        {
            [Test]
            [TestCase(null)]
            [TestCase("")]
            public async Task WhenCarIdIsNullOrEmpty_ReturnsBadRequestWithMessage(string carId)
            {
                var result = await _carController.UpdateCar(carId, new CreateCarRequestBuilder().Build());
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be("Query parameter car_id cannot be null or empty.");
            }

            [Test]
            public async Task WhenCarRequestIsNull_ReturnsBadRequestWithMessage()
            {
                var result = await _carController.UpdateCar("C10", null!);
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be("Request content cannot be null.");
            }

            [Test]
            public async Task WhenCarRequestIsInvalid_ReturnsUnprocessableEntityWithValidationErrorsInMessage()
            {
                var carRequest = new CreateCarRequestBuilder().Build();
                var error1 = "Error that happened with SomeProperty";
                var error2 = "Error that happened with SomeProperty2";
                var validationFail = new List<ValidationFailure>()
                {
                    new ValidationFailure("SomeProperty", error1),
                    new ValidationFailure("SomeProperty2", error2)
                };
                var validationResultWithError = new ValidationResult(validationFail);
                var expectedListOfErrors = new List<string>() { error1, error2 };

                _carRequestValidatorMock.Setup(x => x.ValidateAsync(carRequest, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(validationResultWithError);

                var result = await _carController.UpdateCar("C10", carRequest);

                var resultAsBadRequest = (UnprocessableEntityObjectResult)result;
                resultAsBadRequest.StatusCode.Should().Be(422);
                resultAsBadRequest.Value.As<IEnumerable<string>>().ToList().Should().BeEquivalentTo(expectedListOfErrors);
            }
        }

        internal class RemoveById : CarControllerTests
        {
            [Test]
            [TestCase(null)]
            [TestCase("")]
            public void WhenCarIdIsNullOrEmpty_ReturnsBadRequestWithMessage(string carId)
            {
                var result = _carController.RemoveById(carId);
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be("Query parameter car_id cannot be null or empty.");
            }
        }

        internal class GetUpcomingReservations : CarControllerTests 
        {
            [Test]
            public void WhenCarServiceThrowsAnError_ReturnsBadRequestWithMessage()
            {
                const string exceptionMessage = "Some exception message";
                var dateLimit = DateTime.UtcNow.AddHours(5);

                _carServiceMock.Setup(x => x.AllCarReservationsUntil(dateLimit))
                    .Throws(new Exception(exceptionMessage));

                var result = _carController.GetUpcomingReservations(dateLimit);
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be(exceptionMessage);
            }
        }

        internal class CreateReservation : CarControllerTests 
        {
            [Test]
            public async Task WhenCreateReservationRequestIsNull_ReturnsBadRequestWithMessage()
            {
                var result = await _carController.CreateReservation(null);
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be("Request content cannot be null.");
            }

            [Test]
            public async Task WhenCreateReservationRequestIsInvalid_ReturnsUnprocessableEntityWithAllErrors()
            {
                var createReservationRequestInvalid = new CreateReservationRequestBuilder()
                    .WithEmptyReservationDate()
                    .WithEmptyDurationInMinutes()
                    .Build();

                var error1 = "Error that happened with SomeProperty";
                var error2 = "Error that happened with SomeProperty2";
                var validationFail = new List<ValidationFailure>()
                {
                    new ValidationFailure("SomeProperty", error1),
                    new ValidationFailure("SomeProperty2", error2)
                };
                var validationResultWithError = new ValidationResult(validationFail);
                var expectedListOfErrors = new List<string>() { error1, error2 };

                _reservationRequestValidatorMock.Setup(x => x.ValidateAsync(createReservationRequestInvalid, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(validationResultWithError);

                var result = await _carController.CreateReservation(createReservationRequestInvalid);
                var resultAsBadRequest = (UnprocessableEntityObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(422);
                resultAsBadRequest.Value.As<IEnumerable<string>>().ToList().Should().BeEquivalentTo(expectedListOfErrors);
            }

            [Test]
            public async Task WhenAnErrorOccurWhileReservingACar_ReturnsBadRequestWithMessage() 
            {
                const string exceptionMessage = "Some exception message";
                var carReservationRequest = new CreateReservationRequestBuilder().Build();

                _reservationRequestValidatorMock.Setup(x => x.ValidateAsync(carReservationRequest, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new ValidationResult());

                _carServiceMock.Setup(x => x.ReserveCarAsync(carReservationRequest))
                    .Throws(new Exception(exceptionMessage));

                var result = await _carController.CreateReservation(carReservationRequest);
                var resultAsBadRequest = (BadRequestObjectResult)result;

                resultAsBadRequest.StatusCode.Should().Be(400);
                resultAsBadRequest.Value.Should().Be(exceptionMessage);
            }
        }
    }
}
