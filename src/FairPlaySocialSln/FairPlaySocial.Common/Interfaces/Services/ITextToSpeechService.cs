namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface ITextToSpeechService
    {
        string? Locale { get; set; }
        Task CancelRunningAudioAsync();
        Task SpeakToDefaultSpeakersAsync(string text, CancellationToken cancellationToken);
    }
}
