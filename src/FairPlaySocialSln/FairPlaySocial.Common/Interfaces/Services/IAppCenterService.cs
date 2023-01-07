using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface IAppCenterService
    {
        void LogEvent(EventType eventType);
        void LogException(Exception ex);
    }

    public enum EventType
    {
        Login,
        Logout,
        StartApp,
        LoadIndexPage,
        LoadHomeFeedPage
    }
}
