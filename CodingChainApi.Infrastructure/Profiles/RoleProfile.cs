using AutoMapper;
using CodingChainApi.Infrastructure.Models;

namespace CodingChainApi.Infrastructure.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Right, Domain.Users.Right>().ConvertUsing(role => new Domain.Users.Right(role.Name));
            // CreateMap<Role, RoleModel>().ConvertUsing(role => new RoleModel {Name = });
            // CreateMap<RoleModel,RoleId>().ConvertUsing(role => new RoleId ( role.Name));
        }
    }
}