using AutoMapper;
using CarReservation.Api.Models.Domain;
using CarReservation.Api.Models.DTO.Request;

namespace CarReservation.Api.Models.Mapper
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<ReservationRequest, Reservation>()
                .ForMember(dest => dest.Id, src => src.Ignore())
                .ForMember(dest => dest.CarId, src => src.Ignore())
                .ForMember(dest => dest.DurationInMinutes, src => src.MapFrom(request => TimeSpan.FromMinutes(request.DurationInMinutes)))
                .ForMember(dest => dest.InitialDate, src => src.MapFrom(request => request.ReservationDate))
                .ConstructUsing(src => new Reservation(Guid.Empty, string.Empty, src.ReservationDate, TimeSpan.FromMinutes(src.DurationInMinutes)));
        }
    }
}
