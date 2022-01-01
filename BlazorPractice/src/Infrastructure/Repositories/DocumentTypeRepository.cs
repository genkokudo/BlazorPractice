using BlazorPractice.Application.Interfaces.Repositories;
using BlazorPractice.Domain.Entities.Misc;

namespace BlazorPractice.Infrastructure.Repositories
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly IRepositoryAsync<DocumentType, int> _repository;

        public DocumentTypeRepository(IRepositoryAsync<DocumentType, int> repository)
        {
            _repository = repository;
        }
    }
}