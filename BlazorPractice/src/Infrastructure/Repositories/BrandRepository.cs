using BlazorPractice.Application.Interfaces.Repositories;
using BlazorPractice.Domain.Entities.Catalog;

namespace BlazorPractice.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IRepositoryAsync<Brand, int> _repository;

        public BrandRepository(IRepositoryAsync<Brand, int> repository)
        {
            _repository = repository;
        }
    }
}