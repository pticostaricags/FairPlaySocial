namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface IWhiteLabelingService
    {
        WhiteLabelingData? WhiteLabelingData { get; }
        Task LoadWhiteLabelingDataAsync();
    }

    public class WhiteLabelingData
    {
        public string? ApplicationName { get; set; }
    }
}
