﻿using System.Threading.Tasks;

namespace BlazorPractice.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<bool> IsBrandUsed(int brandId);
    }
}