using BlazorPractice.Application.Interfaces.Repositories;
using BlazorPractice.Domain.Entities.Misc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPractice.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IRepositoryAsync<Document, int> _repository;

        public DocumentRepository(IRepositoryAsync<Document, int> repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsDocumentTypeUsed(int documentTypeId)
        {
            return await _repository.Entities.AnyAsync(b => b.DocumentTypeId == documentTypeId);
        }

        public async Task<bool> IsDocumentExtendedAttributeUsed(int documentExtendedAttributeId)
        {
            return await _repository.Entities.AnyAsync(b => b.ExtendedAttributes.Any(x => x.Id == documentExtendedAttributeId));
        }
    }
}