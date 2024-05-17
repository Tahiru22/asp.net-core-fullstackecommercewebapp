namespace fullstackecommercewebapp.Services
{
    public interface IEmailPassResetSender
    {
        void SendEmail(Message message);
        Task SendEmailAsync(Message message);
    }
}
