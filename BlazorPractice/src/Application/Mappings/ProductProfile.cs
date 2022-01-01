using AutoMapper;
using BlazorPractice.Application.Features.Products.Commands.AddEdit;
using BlazorPractice.Domain.Entities.Catalog;

namespace BlazorPractice.Application.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<AddEditProductCommand, Product>().ReverseMap();
        }
    }
}