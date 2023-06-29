using AutoMapper;
using CarReservation.Api.Models.Domain;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;

namespace CarReservation.Api.Models.Mapper
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            CreateMap<CarRequest, Car>()
                .ForMember(dest => dest.Id, src => src.Ignore())
                .ForMember(dest => dest.Make, src => src.MapFrom(request => request.Make ?? string.Empty))
                .ForMember(dest => dest.Model, src => src.MapFrom(request => request.Model ?? string.Empty))
                .ConstructUsing(src => new Car(string.Empty, src.Make!, src.Model!));

            CreateMap<Car, CarResponse>();
        }
    }
}
