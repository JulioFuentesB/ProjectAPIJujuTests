using AutoMapper;
using Business.Common.DTOs.Customer;
using DataAccess.Data;

namespace Business.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de Customer a CustomerDto
            CreateMap<Customer, CustomerDto>();

            // Mapeo inverso si es necesario
            CreateMap<CustomerDto, Customer>();

            // Mapeo para creación
            CreateMap<CustomerCreateDto, Customer>();

            // Mapeo para actualización
            CreateMap<CustomerUpdateDto, Customer>();

            // Agrega aquí otros mapeos que necesites
        }
    }
}
