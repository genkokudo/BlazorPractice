using System;
using System.ComponentModel;

namespace BlazorPractice.Application.Extensions
{
    /// <summary>
    /// Enumに機能を追加する
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Enumの各要素に[Description("せつめい")]みたいなのが付いている場合、それを取得する
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToDescriptionString(this Enum val)
        {
            var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0
                ? attributes[0].Description
                : val.ToString();
        }
    }
}