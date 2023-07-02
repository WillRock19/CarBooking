using CarReservation.Api.Interfaces.Infraestructure;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CarReservation.Api.Controllers
{
    [ApiController]
    [Consumes("application/json")]
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

        /// <summary>
        /// Get all saved cars.
        /// </summary>
        /// <returns>A list with all cars in database.</returns>
        /// <response code="200">Returns 200 with a list of all cars.</response>
        /// <response code="400">Returns 400 with an error message.</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<CarResponse>> GetAll() 
        {
            try 
            {
                return Ok(carService.GetAllCars());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get an existing car by ID.
        /// </summary>
        /// <param name="car_id">The ID of an existing car.</param>
        /// <returns>The car information.</returns>
        /// <response code="200">Returns 201 with the car data.</response>
        /// <response code="400">Returns 400 with an error message.</response>
        /// <response code="404">Returns 404 with an empty content.</response>
        [HttpGet]
        [Route("{car_id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<CarResponse> GetById(string car_id)
        {
            if(string.IsNullOrEmpty(car_id))
                return BadRequest($"Query parameter {nameof(car_id)} cannot be null or empty.");

            var car = carService.GetCar(car_id);
            return car == null ? NotFound() : Ok(car);
        }

        /// <summary>
        /// Creates a new car.
        /// </summary>
        /// <param name="carRequest">The information of the car to be created.</param>
        /// <returns>The url to get the new resource and the car ID</returns>
        /// <response code="201">Returns 201 with the url of the new resource and the car ID.</response>
        /// <response code="400">Returns 400 with an error message.</response>
        /// <response code="422">Returns 422 with a list of validation rules the carRequest should comply.</response>
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
            return Created($"api/v1/car/{carId}", new { carId });
        }

        /// <summary>
        /// Updates the an existing car.
        /// </summary>
        /// <param name="car_id">The ID of the car to be updated.</param>
        /// <param name="carRequest">The new information to update an existing register.</param>
        /// <returns></returns>
        /// <response code="202">Returns 202 with empty content.</response>
        /// <response code="400">Returns 400 with an error message.</response>
        /// <response code="422">Returns 422 with a list of validation rules the carRequest should comply.</response>
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

        /// <summary>
        /// Creates a reservation with a duration for a specific date.
        /// </summary>
        /// <param name="car_id">The ID of the car to be deleted.</param>
        /// <returns></returns>
        /// <response code="204">Returns 204 with empty content.</response>
        /// <response code="400">Returns 400 with an error message.</response>
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

        /// <summary>
        /// Get all upcoming reservations until a limit date. If date is not informed, returns all upcoming reservations.
        /// </summary>
        /// <param name="limitDate">The maximum reservation date to return. It should be in UTC format.</param>
        /// <returns>List of reservations.</returns>
        /// <response code="200">Returns 200 with the list of reservations.</response>
        /// <response code="400">Returns 400 with an error message.</response>
        [HttpGet("reservations")]
        public ActionResult<IEnumerable<ReservationResponse>> GetUpcomingReservations([FromQuery] DateTime? limitDate) 
        {
            try
            {
                var allReservationsResponse = carService.GetAllUpcomingReservationsUntil(limitDate);
                return Ok(allReservationsResponse);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Creates a reservation with a duration for a specific date.
        /// </summary>
        /// <param name="reservationRequest">The request containing a ReservationDate and a DurationInMinutes. The date should be in UTC format.</param>
        /// <returns>The CreateReservationResult containing a ReservationId, CarId and a Message.</returns>
        /// <response code="200">Returns 200 with the reservation data.</response>
        /// <response code="400">Returns 400 with an error message.</response>
        /// <response code="422">Returns 422 with a list of validation rules the ReservationRequest should comply.</response>
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
