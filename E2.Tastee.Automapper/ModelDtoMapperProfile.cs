using AutoMapper;
using E2.Tastee.Common;
using E2.Tastee.Contracts.Services.Dtos;
using E2.Tastee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using E2.Tastee.Common.Dtos;

namespace E2.Tastee.Automapper
{
    public class ModelDtoMapperProfile : Profile
    {
        public ModelDtoMapperProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.RoleTypeIdList, opt => opt.MapFrom(src => src.Roles == null || src.Roles.Count == 0
                    ? new List<TypeOfUserRole>()
                    : src.Roles.ToList()
                        .Select(s => s.TypeOfUserRole)
                        .ToList()));

            CreateMap<User, SimpleDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

            CreateMap<UserDto, SimpleDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));

            CreateMap<UserRole, UserRoleDto>();
            CreateMap<PaginatedList<User>, PaginatedList<UserDto>>();
        }
    }
}
