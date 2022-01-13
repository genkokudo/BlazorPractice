namespace BlazorPractice.Application.Requests.Mail
{
    /// <summary>
    /// メール送信データ
    /// </summary>
    public class MailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
    }
}