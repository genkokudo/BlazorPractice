using BlazorPractice.Shared.Settings;
using BlazorPractice.Shared.Wrapper;
using System.Threading.Tasks;

namespace BlazorPractice.Shared.Managers
{
    // IManagerではないのでサービスの個別登録が必要
    // IManagerではないのは恐らく、派生先のインタフェースがサーバ側で使用するものだから？
    public interface IPreferenceManager
    {
        /// <summary>
        /// ローカルストレージに設定を保存する
        /// </summary>
        /// <param name="preference"></param>
        /// <returns></returns>
        Task SetPreference(IPreference preference);

        /// <summary>
        /// ローカルストレージから設定を取得する
        /// </summary>
        /// <returns></returns>
        Task<IPreference> GetPreference();

        /// <summary>
        /// 言語を変更する
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}