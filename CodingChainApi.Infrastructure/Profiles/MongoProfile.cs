using AutoMapper;
using MongoDB.Bson;

namespace CodingChainApi.Infrastructure.Profiles
{
    public class MongoProfile : Profile
    {
        public MongoProfile()
        {
            CreateMap<string, ObjectId>().ConvertUsing((s, id) => new ObjectId(s));
            CreateMap<ObjectId, string>().ConvertUsing((id, s) => id.ToString());
        }
    }
}