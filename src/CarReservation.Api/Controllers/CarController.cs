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
        public async Task<IActionResult> AddCar([FromBody] CarRequest carRequest)
        {
            if (carRequest == null)
                return UnprocessableEntity("Request content cannot be null.");

            var validationResult = await validator.ValidateAsync(carRequest);

            if(!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));

            var carId = carService.AddCar(carRequest);
            return Accepted(new { carId });
        }

        [HttpPut("{car_id}")]
        public async Task<IActionResult> UpdateCar([FromQuery] string car_id, [FromBody] CarRequest carRequest)
        {
            if (string.IsNullOrEmpty(car_id))
                return BadRequest($"Query parameter {nameof(car_id)} cannot be null or empty.");

            if (carRequest == null)
                return UnprocessableEntity("Request content cannot be null.");

            var validationResult = await validator.ValidateAsync(carRequest);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));

            return Accepted();
        }

        [HttpDelete("{car_id}")]
        public IActionResult RemoveById([FromQuery] string car_id)
        {
            if (string.IsNullOrEmpty(car_id))
                return BadRequest($"Query parameter {nameof(car_id)} cannot be null or empty.");

            return NoContent();
        }
    }
}
