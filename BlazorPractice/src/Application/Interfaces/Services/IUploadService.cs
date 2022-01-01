using BlazorPractice.Application.Requests;

namespace BlazorPractice.Application.Interfaces.Services
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
    }
}