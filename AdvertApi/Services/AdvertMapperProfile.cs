using AdvertApi.Models;
using AutoMapper;

namespace AdvertApi.Services
{
    public class AdvertMapperProfile : Profile
    {
        public AdvertMapperProfile() 
        {
            CreateMap<AdvertModel, AdvertDto>();
        }

    }
}
