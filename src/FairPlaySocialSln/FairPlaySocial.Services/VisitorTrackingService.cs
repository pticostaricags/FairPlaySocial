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
        private FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext { get; }
        private IpStackService IpStackService { get; }
        private IpDataService IpDataService { get; }

        public VisitorTrackingService(IHttpContextAccessor httpContextAccessor,
            FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext,
            IpStackService ipStackService, IpDataService ipDataService)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.fairPlaySocialDatabaseContext = fairPlaySocialDatabaseContext;
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
                    var ipGeoLocationInfo = await IpDataService.GetIpGeoLocationInfoAsync(ipAddress: parsedIpAddress);
                    //var ipGeoLocationInfo = await IpStackService.GetIpGeoLocationInfoAsync(ipAddress: parsedIpAddress);
                    country = ipGeoLocationInfo.country_name;
                }
                catch (Exception ex) 
                {
                    string message = $"IpDataService.GetIpGeoLocationInfoAsync failed for Ip: {parsedIpAddress}. Error: {ex.ToString()}";
                    throw new Exception(message, ex);
                }
                var host = httpContext.Request.Host.Value;
                var userAgent = httpContext.Request.Headers["User-Agent"].First();
                ApplicationUser? userEntity = null;
                if (!String.IsNullOrWhiteSpace(visitorTrackingModel.UserAzureAdB2cObjectId))
                    userEntity = await this.fairPlaySocialDatabaseContext
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
                await this.fairPlaySocialDatabaseContext.VisitorTracking
                    .AddAsync(visitedPage, cancellationToken:cancellationToken);
                await this.fairPlaySocialDatabaseContext
                    .SaveChangesAsync(cancellationToken:cancellationToken);
                return visitedPage;
            }
            catch (Exception ex)
            {
                try
                {
                    await this.fairPlaySocialDatabaseContext.ErrorLog.AddAsync(new ErrorLog()
                    {
                        FullException = ex.ToString(),
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    }, cancellationToken: cancellationToken);
                    await this.fairPlaySocialDatabaseContext
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
            var entity = await this.fairPlaySocialDatabaseContext.VisitorTracking
                .SingleOrDefaultAsync(p => p.VisitorTrackingId == visitorTrackingId, cancellationToken);
            if (entity != null)
            {
                entity.LastTrackedDateTime = DateTimeOffset.UtcNow;
                await fairPlaySocialDatabaseContext.SaveChangesAsync(cancellationToken);
            }
            return entity;
        }
    }
}
