using AutoMapper;
using CodingChainApi.Infrastructure.Models;
using Domain.Users;
using Right = Domain.Users.Right;

namespace CodingChainApi.Infrastructure.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Models.Right, Right>().ConvertUsing(role => new Right(role.Name));
            // CreateMap<Role, RoleModel>().ConvertUsing(role => new RoleModel {Name = });
            // CreateMap<RoleModel,RoleId>().ConvertUsing(role => new RoleId ( role.Name));
        }
    }
}