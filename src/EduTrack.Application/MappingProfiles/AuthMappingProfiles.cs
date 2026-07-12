using AutoMapper;
using EduTrack.Application.DTOs.Auth;
using EduTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduTrack.Application.MappingProfiles;

public class AuthMappingProfiles : Profile
{
	public AuthMappingProfiles()
	{
		CreateMap<User, LoginResponse>()
			 .ForMember(c => c.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()));


		CreateMap<RegisterRequest, User>();
		CreateMap<User, RegisterResponse>();
    }
}
