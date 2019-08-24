using AutoMapper;
using KurirServer.Entities;
using KurirServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, RegistrationModel>().ReverseMap();
            CreateMap<User, LoginUserModel>().ReverseMap();
        }
       

    }
}
