namespace BlazorPractice.Shared.Constants.Application
{
    public static class ApplicationConstants
    {
        public static class SignalR
        {
            public const string HubUrl = "/signalRHub";
            public const string SendUpdateDashboard = "UpdateDashboardAsync";
            public const string ReceiveUpdateDashboard = "UpdateDashboard";
            public const string SendRegenerateTokens = "RegenerateTokensAsync";
            public const string ReceiveRegenerateTokens = "RegenerateTokens";
            public const string ReceiveChatNotification = "ReceiveChatNotification";
            public const string SendChatNotification = "ChatNotificationAsync";
            public const string ReceiveMessage = "ReceiveMessage";
            public const string SendMessage = "SendMessageAsync";

            public const string OnConnect = "OnConnectAsync";
            public const string ConnectUser = "ConnectUser";
            public const string OnDisconnect = "OnDisconnectAsync";
            public const string DisconnectUser = "DisconnectUser";
            public const string OnChangeRolePermissions = "OnChangeRolePermissions";
            public const string LogoutUsersByRole = "LogoutUsersByRole";
        }

        // GetOrAddAsyncというメソッドでキャッシュがあれば取得、無ければ第2引数のデリゲートを実行してキャッシュ登録という仕組み
        /// <summary>
        /// LazyCacheで使用するキャッシュキー
        /// </summary>
        public static class Cache
        {
            public const string GetAllBrandsCacheKey = "all-brands";
            public const string GetAllDocumentTypesCacheKey = "all-document-types";

            /// <summary>
            /// 全検索の時にテーブルごとキャッシュする
            /// レコード削除の時にクリアする
            /// </summary>
            /// <param name="entityFullName"></param>
            /// <returns></returns>
            public static string GetAllEntityExtendedAttributesCacheKey(string entityFullName)
            {
                return $"all-{entityFullName}-extended-attributes";
            }

            /// <summary>
            /// EntityIdによるデータ取得時にキーを発行
            /// </summary>
            /// <typeparam name="TEntityId"></typeparam>
            /// <param name="entityFullName"></param>
            /// <param name="entityId"></param>
            /// <returns></returns>
            public static string GetAllEntityExtendedAttributesByEntityIdCacheKey<TEntityId>(string entityFullName, TEntityId entityId)
            {
                return $"all-{entityFullName}-extended-attributes-{entityId}";
            }
        }

        public static class MimeTypes
        {
            public const string OpenXml = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
    }
}