using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Dtos;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

/// <summary>
/// AutoMapper profile to map domain User to ListUserItemResult.
/// </summary>
public class ListUsersProfile : Profile
{
    public ListUsersProfile()
    {
        CreateMap<User, ListUserItemResult>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<Name, NameDto > ()
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.Firstname))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.Lastname));

        CreateMap<Geolocation, GeolocationDto>()
            .ForMember(d => d.Lat, o => o.MapFrom(s => s.Lat))
            .ForMember(d => d.Long, o => o.MapFrom(s => s.Long));

        CreateMap<Address, AddressDto>()
            .ForMember(d => d.City, o => o.MapFrom(s => s.City))
            .ForMember(d => d.Street, o => o.MapFrom(s => s.Street))
            .ForMember(d => d.Number, o => o.MapFrom(s => s.Number))
            .ForMember(d => d.Zipcode, o => o.MapFrom(s => s.Zipcode))
            .ForMember(d => d.Geolocation, o => o.MapFrom(s => s.Geolocation));

    }
}
