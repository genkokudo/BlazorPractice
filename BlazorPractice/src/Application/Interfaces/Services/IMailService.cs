using BlazorPractice.Application.Requests.Mail;
using System.Threading.Tasks;

namespace BlazorPractice.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}