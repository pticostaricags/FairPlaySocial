using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.ExternalReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages.User.ExternalReports
{
    [Authorize(Roles = $"{Constants.Roles.User}")]
    [Route(Constants.MauiBlazorAppPages.UserRolePagesRoutes.ExternalReportsViewer)]
    public partial class ExternalReportsViewer
    {
        [Inject]
        private RestrictedExternalReportClientService? RestrictedExternalReportClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private bool IsBusy { get; set; }
        private ExternalReportModel[]? AllExternalReports { get; set; }
        private string? SelectedReportName { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    this.IsBusy = true;
                    this.AllExternalReports = await this.RestrictedExternalReportClientService!
                        .GetAllExternalReportsInfoAsync(base.CancellationToken);
                }
                catch (Exception ex)
                {
                    await this.ToastService!
                        .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
                }
                finally
                {
                    this.IsBusy = false;
                }
                StateHasChanged();
            }
        }
    }
}
