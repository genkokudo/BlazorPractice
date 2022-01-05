using BlazorPractice.Application.Extensions;
using BlazorPractice.Application.Interfaces.Services;
using BlazorPractice.Application.Requests;
using System.IO;

namespace BlazorPractice.Infrastructure.Services
{
    public class UploadService : IUploadService
    {
        /// <summary>
        /// リクエストのファイルを保存し、そのDBに格納するパスを返す
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DBに記録する相対パス</returns>
        public string UploadAsync(UploadRequest request)
        {
            if (request.Data == null) return string.Empty;

            // リクエストからデータを取り出す
            var streamData = new MemoryStream(request.Data);
            if (streamData.Length > 0)
            {
                // アップロード先のフォルダパス
                // UploadTypeのEnumにはDescriptionにフォルダパスを格納しているので、取得する
                var folder = request.UploadType.ToDescriptionString();
                var folderName = Path.Combine("Files", folder);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName); // フルパスにする
                
                // ディレクトリが無ければ作成
                bool exists = Directory.Exists(pathToSave);
                if (!exists)
                    Directory.CreateDirectory(pathToSave);

                // 保存先ファイルフルパス
                var fileName = request.FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);

                // DBに記録するパス
                var dbPath = Path.Combine(folderName, fileName);
                if (File.Exists(dbPath))
                {
                    // 有効なファイル名を取得する（既に同じ名前のファイルがあれば、"(1)"のような名前を付加する）
                    dbPath = NextAvailableFilename(dbPath);
                    fullPath = NextAvailableFilename(fullPath);
                }

                // 保存
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    streamData.CopyTo(stream);
                }
                return dbPath;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// ファイル名が重複したときに付加する文字列パターン
        /// </summary>
        private static string numberPattern = " ({0})";

        /// <summary>
        /// 次の有効なファイル名を取得する
        /// 同じ名前のファイルがあれば、"ファイル名 (1)"のような名前を付加する
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns></returns>
        public static string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
                return path;

            // パスが拡張子を持つ場合、拡張子の直前に(1)のような番号パターンを挿入し、次のファイル名を返します。
            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            // そうでなければ，単にパターンをパスに追加して，次のファイル名を返す。
            return GetNextFilename(path + numberPattern);
        }

        /// <summary>
        /// "ファイル名"を"ファイル名 (1)"のようにする
        /// 空いている番号は二分探索で探す
        /// </summary>
        /// <param name="pattern">"ファイル名 ({0})"のようなパターン</param>
        /// <returns></returns>
        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);

            if (!File.Exists(tmp))
                return tmp;

            int min = 1, max = 2; // minは番号が埋まっていると分かっている所、maxは未検証部分

            // ファイルがあれば2倍にして空いているファイル名を探す
            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            // 2の乗数で空いている番号が見つかれば、二分探索で空いている番号を探す
            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }
    }
}