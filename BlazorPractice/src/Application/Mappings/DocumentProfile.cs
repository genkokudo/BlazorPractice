using AutoMapper;
using BlazorPractice.Application.Features.Documents.Commands.AddEdit;
using BlazorPractice.Application.Features.Documents.Queries.GetById;
using BlazorPractice.Domain.Entities.Misc;

namespace BlazorPractice.Application.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<AddEditDocumentCommand, Document>().ReverseMap();
            CreateMap<GetDocumentByIdResponse, Document>().ReverseMap();
        }
    }
}