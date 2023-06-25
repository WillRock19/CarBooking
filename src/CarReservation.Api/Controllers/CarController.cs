using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CarReservation.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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
        [Route("")]
        public IEnumerable<CarResponse> GetAll() => carService.GetAllCars();

        [HttpGet]
        [Route("{car_id}")]
        public IActionResult GetById(string car_id)
        {
            if(string.IsNullOrEmpty(car_id))
                return BadRequest($"Query parameter {nameof(car_id)} cannot be null or empty.");

            var car = carService.GetCar(car_id);
            return car == null ? NotFound() : Ok(car);
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

        [HttpPut]
        [Route("{car_id}")]
        public async Task<IActionResult> UpdateCar(string car_id, [FromBody] CarRequest carRequest)
        {
            if (string.IsNullOrEmpty(car_id))
                return BadRequest($"Query parameter {nameof(car_id)} cannot be null or empty.");

            if (carRequest == null)
                return UnprocessableEntity("Request content cannot be null.");

            var validationResult = await validator.ValidateAsync(carRequest);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));

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
    }
}
