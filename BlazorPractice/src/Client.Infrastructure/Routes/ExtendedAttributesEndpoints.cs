namespace BlazorPractice.Client.Infrastructure.Routes
{
    /// <summary>
    /// 本体はBlazorPractice.Server.Controllers.Utilities.ExtendedAttributes.Base
    /// </summary>
    public static class ExtendedAttributesEndpoints
    {
        public static string GetAll(string entityName) => $"api/{entityName}ExtendedAttributes";
        public static string GetAllByEntityId<TEntityId>(string entityName, TEntityId entityId) => $"{GetAll(entityName)}/by-entity/{entityId}";

        /// <summary>
        /// 全件Excelに出力
        /// </summary>
        /// <typeparam name="TEntityId"></typeparam>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="includeEntity"></param>
        /// <param name="onlyCurrentGroup"></param>
        /// <param name="currentGroup"></param>
        /// <returns></returns>
        public static string Export<TEntityId>(string entityName, TEntityId entityId, bool includeEntity, bool onlyCurrentGroup = false, string currentGroup = "") => $"api/{entityName}ExtendedAttributes/export?{nameof(entityId)}={entityId}&{nameof(includeEntity)}={includeEntity}&{nameof(onlyCurrentGroup)}={onlyCurrentGroup}&{nameof(currentGroup)}={currentGroup}";

        /// <summary>
        /// 条件で絞ってExcelに出力
        /// </summary>
        /// <typeparam name="TEntityId"></typeparam>
        /// <param name="entityName"></param>
        /// <param name="searchString">検索条件</param>
        /// <param name="entityId"></param>
        /// <param name="includeEntity"></param>
        /// <param name="onlyCurrentGroup"></param>
        /// <param name="currentGroup"></param>
        /// <returns></returns>
        public static string ExportFiltered<TEntityId>(string entityName, string searchString, TEntityId entityId, bool includeEntity, bool onlyCurrentGroup = false, string currentGroup = "") => $"api/{entityName}ExtendedAttributes/export?{nameof(searchString)}={searchString}&{nameof(entityId)}={entityId}&{nameof(includeEntity)}={includeEntity}&{nameof(onlyCurrentGroup)}={onlyCurrentGroup}&{nameof(currentGroup)}={currentGroup}";
        public static string Delete(string entityName) => $"api/{entityName}ExtendedAttributes";
        public static string Save(string entityName) => $"api/{entityName}ExtendedAttributes";
        public static string GetCount(string entityName) => $"api/{entityName}ExtendedAttributes/count";
    }
}