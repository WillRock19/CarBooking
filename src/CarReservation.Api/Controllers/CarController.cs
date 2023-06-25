using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CarReservation.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly IValidator<CarRequest> validator;
        private readonly ICarService carService;

        public CarController(ICarService carService, IValidator<CarRequest> validator)
        {
            this.carService = carService;
            this.validator = validator;
        }

        [HttpGet]
        public IEnumerable<CarResponse> GetAll() => carService.GetAllCars();

        [HttpGet("{car_id}")]
        public IActionResult GetById([FromQuery] string car_id)
        {
            if(string.IsNullOrEmpty(car_id))
                return BadRequest($"Query parameter {nameof(car_id)} cannot be null or empty.");

            return Ok();
        }

        [HttpPost]
        public IActionResult AddCar([FromBody] CarRequest carRequest)
        {
            if (carRequest == null)
                return UnprocessableEntity("Body content cannot be null.");

            var validationResult = validator.Validate(carRequest);

            if(!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var carId = carService.InsertCar(carRequest);
            return Accepted(new { carId });
        }

        [HttpPut("{car_id}")]
        public IActionResult UpdateCar([FromQuery] string car_id, [FromBody] CarRequest car)
        {
            return Accepted();
        }

        [HttpDelete("{car_id}")]
        public IActionResult RemoveCar([FromQuery] string car_id)
        {
            return NoContent();
        }
    }
}
