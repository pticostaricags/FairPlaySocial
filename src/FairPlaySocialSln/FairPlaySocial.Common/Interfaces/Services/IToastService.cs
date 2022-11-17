namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface IToastService
    {
        Task ShowErrorMessageAsync(string message, CancellationToken cancellationToken);
        Task ShowSuccessMessageAsync(string message, CancellationToken cancellationToken);
    }
}
