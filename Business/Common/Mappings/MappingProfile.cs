using AutoMapper;
using Business.Common.DTOs.Customer;
using Business.Common.DTOs.Post;
using DataAccess.Data;

namespace Business.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de Customer a CustomerDto
            CreateMap<Customer, CustomerDto>().ReverseMap();

            // Mapeo para creación
            CreateMap<CustomerCreateDto, Customer>().ReverseMap();

            // Mapeo para actualización
            CreateMap<CustomerUpdateDto, Customer>().ReverseMap();

            // Agrega aquí otros mapeos que necesites

            CreateMap<Post, PostDto>().ReverseMap();

            // Mapeo para creación
            CreateMap<PostCreateDto, Post>().ReverseMap();

            // Mapeo para actualización
            CreateMap<PostUpdateDto, Post>().ReverseMap();
        }
    }
}
