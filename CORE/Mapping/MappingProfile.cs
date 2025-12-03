using AutoMapper;
using CORE.DTOs;
using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Mapping
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            //CreateMap<Source, Destination>();
            CreateMap<UserRegisterDto,User>();
            CreateMap<User, AuthBaseResponseDto>();
            CreateMap<UpdateUserDto, User>().ReverseMap();
        }
    }
}
