﻿using BlazorPractice.Application.Features.DocumentTypes.Commands.AddEdit;
using BlazorPractice.Application.Features.DocumentTypes.Queries.GetAll;
using BlazorPractice.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPractice.Client.Infrastructure.Managers.Misc.DocumentType
{
    public interface IDocumentTypeManager : IManager
    {
        Task<IResult<List<GetAllDocumentTypesResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditDocumentTypeCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}