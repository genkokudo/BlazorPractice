using System.Collections.Generic;

namespace BlazorPractice.Shared.Wrapper
{
    /// <summary>
    /// 実行結果
    /// </summary>
    public interface IResult
    {
        List<string> Messages { get; set; }

        bool Succeeded { get; set; }
    }

    /// <summary>
    /// データを伴う結果
    /// </summary>
    /// <typeparam name="T">データ形式を指定する</typeparam>
    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }
}