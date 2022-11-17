using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;

namespace FairPlaySocial.ClientsConfiguration
{
    public class WhiteLabelingService : IWhiteLabelingService
    {
        private WhiteLabelingData? _whiteLabelingData;
        public WhiteLabelingData? WhiteLabelingData => this._whiteLabelingData;

        public Task LoadWhiteLabelingDataAsync()
        {
            this._whiteLabelingData ??= new WhiteLabelingData()
            {
                ApplicationName = Constants.Assemblies.MainAppAssemblyName
            };
            return Task.CompletedTask;
        }
    }
}
