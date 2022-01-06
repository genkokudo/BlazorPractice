using BlazorPractice.Application.Enums;

namespace BlazorPractice.Application.Requests
{
    /// <summary>
    /// 写真やファイルをアップロードする場合に使用するリクエスト
    /// 
    /// 通常ファイルのアップロードはこのまま使う
    /// 写真アップロードの際はUpdateProfilePictureRequestに継承する
    /// </summary>
    public class UploadRequest
    {
        /// <summary>
        /// 削除時はstring.Empty
        /// </summary>
        public string FileName { get; set; }
        public string Extension { get; set; }
        public UploadType UploadType { get; set; }
        /// <summary>
        /// 削除時はnull
        /// </summary>
        public byte[] Data { get; set; }
    }
}