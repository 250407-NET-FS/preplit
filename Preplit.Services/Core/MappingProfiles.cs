using AutoMapper;
using Preplit.Domain;

namespace Preplit.Services.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, User>();
            CreateMap<Category, Category>();
            CreateMap<Card, Card>();
        }
    }
}