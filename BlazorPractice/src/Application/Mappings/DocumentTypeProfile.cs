using AutoMapper;
using BlazorPractice.Application.Features.DocumentTypes.Commands.AddEdit;
using BlazorPractice.Application.Features.DocumentTypes.Queries.GetAll;
using BlazorPractice.Application.Features.DocumentTypes.Queries.GetById;
using BlazorPractice.Domain.Entities.Misc;

namespace BlazorPractice.Application.Mappings
{
    public class DocumentTypeProfile : Profile
    {
        public DocumentTypeProfile()
        {
            CreateMap<AddEditDocumentTypeCommand, DocumentType>().ReverseMap();
            CreateMap<GetDocumentTypeByIdResponse, DocumentType>().ReverseMap();
            CreateMap<GetAllDocumentTypesResponse, DocumentType>().ReverseMap();
        }
    }
}