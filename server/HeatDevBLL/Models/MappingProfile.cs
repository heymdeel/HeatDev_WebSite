using AutoMapper;
using HeatDevBLL.Models.DTO;
using HeatDevBLL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeatDevBLL.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserSignUpDTO, User>();
            CreateMap<UserSignUpDTO, UserProfile>();

            CreateMap<OrderCreateDTO, Order>();
            CreateMap<ReviewCreateDTO, Review>();

            CreateMap<UserProfileDTO, UserProfile>();
        }
    }
}
