using FairPlaySocial.Common.Providers;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.VisitorTracking;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.IpData.Services;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Services
{
    public class VisitorTrackingService
    {
        private IHttpContextAccessor HttpContextAccessor { get; }
        private FairPlaySocialDatabaseContext FairPlaySocialDatabaseContext { get; }
        private IpStackService IpStackService { get; }
        private IpDataService IpDataService { get; }

        public VisitorTrackingService(IHttpContextAccessor httpContextAccessor,
            FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext,
            IpStackService ipStackService, IpDataService ipDataService)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.FairPlaySocialDatabaseContext = fairPlaySocialDatabaseContext;
            this.IpStackService = ipStackService;
            this.IpDataService = ipDataService;
        }

        public async Task<VisitorTracking?> TrackVisitAsync(
            VisitorTrackingModel visitorTrackingModel,
            CancellationToken cancellationToken)
        {
            try
            {
                var httpContext = HttpContextAccessor.HttpContext;
                var remoteIpAddress = httpContext.Connection.RemoteIpAddress.ToString();
                if (remoteIpAddress == "::1")
                {
                    var ipAddresses = await IpAddressProvider.GetCurrentHostIPv4AddressesAsync();
                    remoteIpAddress = ipAddresses.First();
                }
                var parsedIpAddress = System.Net.IPAddress.Parse(remoteIpAddress);
                string country = string.Empty;
                try
                {
                    var ipGeoLocationInfo = await IpDataService.GetIpGeoLocationInfoAsync(ipAddress: parsedIpAddress, cancellationToken);
                    //var ipGeoLocationInfo = await IpStackService.GetIpGeoLocationInfoAsync(ipAddress: parsedIpAddress);
                    country = ipGeoLocationInfo.country_name;
                }
                catch (Exception ex) 
                {
                    string message = $"IpDataService.GetIpGeoLocationInfoAsync failed for Ip: {parsedIpAddress}. Error: {ex}";
                    throw new Exception(message, ex);
                }
                var host = httpContext.Request.Host.Value;
                string userAgent = string.Empty;
                if (httpContext.Request.Headers.ContainsKey("User-Agent"))
                {
                    userAgent = httpContext.Request!.Headers["User-Agent"]!.First()!;
                }
                else
                {
                    userAgent = "Unknown";
                }
                
                ApplicationUser? userEntity = null;
                if (!String.IsNullOrWhiteSpace(visitorTrackingModel.UserAzureAdB2cObjectId))
                    userEntity = await this.FairPlaySocialDatabaseContext
                        .ApplicationUser
                        .SingleOrDefaultAsync(p => 
                        p.AzureAdB2cobjectId.ToString() == 
                        visitorTrackingModel.UserAzureAdB2cObjectId,
                        cancellationToken:cancellationToken);
                var visitedPage = new VisitorTracking()
                {
                    ApplicationUserId = userEntity?.ApplicationUserId,
                    Country = country,
                    Host = host,
                    RemoteIpAddress = remoteIpAddress,
                    UserAgent = userAgent,
                    VisitDateTime = DateTimeOffset.UtcNow,
                    VisitedUrl = visitorTrackingModel.VisitedUrl,
                    SessionId = visitorTrackingModel.SessionId
                };
                await this.FairPlaySocialDatabaseContext.VisitorTracking
                    .AddAsync(visitedPage, cancellationToken:cancellationToken);
                await this.FairPlaySocialDatabaseContext
                    .SaveChangesAsync(cancellationToken:cancellationToken);
                return visitedPage;
            }
            catch (Exception ex)
            {
                try
                {
                    await this.FairPlaySocialDatabaseContext.ErrorLog.AddAsync(new ErrorLog()
                    {
                        FullException = ex.ToString(),
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    }, cancellationToken: cancellationToken);
                    await this.FairPlaySocialDatabaseContext
                        .SaveChangesAsync(cancellationToken:cancellationToken);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return null;
        }

        public async Task<VisitorTracking?> UpdateVisitTimeElapsedAsync(long visitorTrackingId, CancellationToken cancellationToken)
        {
            var entity = await this.FairPlaySocialDatabaseContext.VisitorTracking
                .SingleOrDefaultAsync(p => p.VisitorTrackingId == visitorTrackingId, cancellationToken);
            if (entity != null)
            {
                entity.LastTrackedDateTime = DateTimeOffset.UtcNow;
                await FairPlaySocialDatabaseContext.SaveChangesAsync(cancellationToken);
            }
            return entity;
        }
    }
}
