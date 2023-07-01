using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CarReservation.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly IValidator<CreateCarRequest> carRequestvalidator;
        private readonly IValidator<CreateReservationRequest> reservationRequestValidator;
        private readonly ICarService carService;

        public CarController(ICarService carService, IValidator<CreateCarRequest> carRequestvalidator, IValidator<CreateReservationRequest> reservationRequestValidator)
        {
            this.carService = carService;
            this.carRequestvalidator = carRequestvalidator;
            this.reservationRequestValidator = reservationRequestValidator;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IEnumerable<CarResponse> GetAll() => carService.GetAllCars();

        [HttpGet]
        [Route("{car_id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetById(string car_id)
        {
            if(string.IsNullOrEmpty(car_id))
                return BadRequest($"Query parameter {nameof(car_id)} cannot be null or empty.");

            var car = carService.GetCar(car_id);
            return car == null ? NotFound() : Ok(car);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateCar([FromBody] CreateCarRequest carRequest)
        {
            if (carRequest == null)
                return BadRequest("Request content cannot be null.");

            var validationResult = await carRequestvalidator.ValidateAsync(carRequest);

            if(!validationResult.IsValid)
                return UnprocessableEntity(validationResult.Errors.Select(x => x.ErrorMessage));

            var carId = carService.AddCar(carRequest);
            return Created($"api/v1/{carId}", new { carId });
        }

        [HttpPut]
        [Route("{car_id}")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateCar(string car_id, [FromBody] CreateCarRequest carRequest)
        {
            if (string.IsNullOrEmpty(car_id))
                return BadRequest($"Query parameter {nameof(car_id)} cannot be null or empty.");

            if (carRequest == null)
                return BadRequest("Request content cannot be null.");

            var validationResult = await carRequestvalidator.ValidateAsync(carRequest);

            if (!validationResult.IsValid)
                return UnprocessableEntity(validationResult.Errors.Select(x => x.ErrorMessage));

            try
            {
                var updatedCar = carService.UpdateCar(car_id, carRequest);
                return Accepted();
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("{car_id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult RemoveById(string car_id)
        {
            if (string.IsNullOrEmpty(car_id))
                return BadRequest($"Query parameter {nameof(car_id)} cannot be null or empty.");

            try
            {
                carService.DeleteCar(car_id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("reservations")]
        public IActionResult GetUpcomingReservations([FromQuery] DateTime? untilDate) 
        {
            try
            {
                var allReservationsResponse = carService.AllCarReservationsUntil(untilDate);
                return Ok(allReservationsResponse);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("reservations")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest reservationRequest)
        {
            if (reservationRequest == null)
                return BadRequest("Request content cannot be null.");

            var validationResult = await reservationRequestValidator
                .ValidateAsync(reservationRequest);

            if (!validationResult.IsValid)
                return UnprocessableEntity(validationResult.Errors.Select(x => x.ErrorMessage));

            try
            {
                var response = await carService.ReserveCarAsync(reservationRequest);
                return response.ReservationId.HasValue ? Ok(response) : BadRequest(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
