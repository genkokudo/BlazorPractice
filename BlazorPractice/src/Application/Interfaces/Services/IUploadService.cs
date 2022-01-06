using BlazorPractice.Application.Requests;

namespace BlazorPractice.Application.Interfaces.Services
{
    /// <summary>
    /// ファイルのアップロード処理
    /// </summary>
    public interface IUploadService
    {
        /// <summary>
        /// リクエストのファイルを保存し、DBに格納するパスを返す
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DBに記録する相対パス</returns>
        string UploadAsync(UploadRequest request);
    }
}