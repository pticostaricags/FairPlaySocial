using FairPlaySocial.Common.Interfaces.Services;

namespace FairPlaySocial.MAUIBlazor.MultiPlatformServices
{
    public class MauiAppCenterService : Common.Interfaces.Services.IAppCenterService
    {
        public void LogEvent(EventType eventType)
        {
            try
            {
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventType.ToString());
            }
            catch (Exception ex) 
            {
                this.LogException(ex);
            }
        }

        public void LogException(Exception ex)
        {
            Microsoft.AppCenter.Crashes.Crashes.TrackError(ex);
        }
    }
}
