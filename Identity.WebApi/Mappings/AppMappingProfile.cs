using AutoMapper;
using Identity.WebApi.Models;

/// <summary>
/// Defines the mapping profiles for AutoMapper.
/// </summary>
public class AppMappingProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AppMappingProfile"/> class.
	/// </summary>
	public AppMappingProfile()
	{
		// Mapping configuration for RegisterDto to AppUser
		CreateMap<RegisterDto, AppUser>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
			.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

		// Mapping configuration for AppUser to LoginResponseDto
		CreateMap<AppUser, LoginResponseDto>()
			.ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
			.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
			.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id.ToString()));
	}
}
