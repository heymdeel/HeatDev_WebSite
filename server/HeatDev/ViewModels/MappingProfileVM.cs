using AutoMapper;
using HeatDevBLL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatDev.ViewModels
{
    public class MappingProfileVM : Profile
    {
        public MappingProfileVM()
        {
            CreateMap<User, TokenVM>();
        }
    }
}
