using AutoMapper;
using CarReservation.Api.Models.Domain;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;

namespace CarReservation.Api.Models.Mapper
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<CreateReservationRequest, Reservation>()
                .ForMember(dest => dest.Id, src => src.Ignore())
                .ForMember(dest => dest.CarId, src => src.Ignore())
                .ForMember(dest => dest.DurationInMinutes, src => src.MapFrom(request => TimeSpan.FromMinutes(request.DurationInMinutes)))
                .ForMember(dest => dest.InitialDate, src => src.MapFrom(request => request.ReservationDate))
                .ConstructUsing(src => new Reservation(Guid.Empty, string.Empty, src.ReservationDate, TimeSpan.FromMinutes(src.DurationInMinutes)));

            CreateMap<Reservation, ReservationResponse>()
                .ForMember(dest => dest.DurationInMinutes, src => src.MapFrom(reservation => reservation.DurationInMinutes.TotalMinutes));
        }
    }
}
