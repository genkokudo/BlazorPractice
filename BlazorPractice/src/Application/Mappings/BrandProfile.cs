using AutoMapper;
using BlazorPractice.Application.Features.Brands.Commands.AddEdit;
using BlazorPractice.Application.Features.Brands.Queries.GetAll;
using BlazorPractice.Application.Features.Brands.Queries.GetById;
using BlazorPractice.Domain.Entities.Catalog;

namespace BlazorPractice.Application.Mappings
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<AddEditBrandCommand, Brand>().ReverseMap();
            CreateMap<GetBrandByIdResponse, Brand>().ReverseMap();
            CreateMap<GetAllBrandsResponse, Brand>().ReverseMap();
        }
    }
}