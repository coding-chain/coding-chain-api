using System;
using AutoMapper;
using CodingChainApi.Infrastructure.Models;
using Domain.Users;
using User = CodingChainApi.Infrastructure.Models.User;

namespace CodingChainApi.Infrastructure.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Guid, UserId>().ConvertUsing((val, id) => new UserId(val));
            CreateMap<UserId, Guid>().ConvertUsing((id, val) => id.Value);
            
            CreateMap<User, Domain.Users.UserAggregate>().ReverseMap();
        }
    }
}