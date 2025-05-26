using AutoMapper;
using Preplit.Domain;
using Preplit.Domain.DTOs;

namespace Preplit.Services.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, User>();
            CreateMap<CategoryUpdateDTO, Category>();
            CreateMap<CardUpdateDTO, Card>();
        }
    }
}