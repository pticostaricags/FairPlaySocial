using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Culture;
using FairPlaySocial.Models.Resource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages.Admin.Localization
{
    [Authorize(Roles = Constants.Roles.Admin)]
    [Route(Constants.MauiBlazorAppPages.AdminRolePagesRoutes.ResourceKeysAdmin)]
    public partial class ResourceKeysAdmin
    {
        [Inject]
        private CultureClientService? CultureClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        [Inject]
        private ResourceClientService? ResourceClientService { get; set; }
        private bool IsBusy { get; set; }
        private CultureModel[]? SupportedCultures { get; set; }
        private string? SelectedCulture { get; set; }
        private ResourceModel[]? SelectedCultureResources { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    this.IsBusy = true;
                    this.SupportedCultures = await this.CultureClientService!
                        .GetAllCultureAsync(cancellationToken: base.CancellationToken);
                }
                catch (Exception ex)
                {
                    await this.ToastService!
                        .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
                }
                finally
                {
                    this.IsBusy = false;
                    StateHasChanged();
                }
            }
        }

        private async Task OnSelectedCultureChangeAsync(string? selectedCultureName)
        {
            this.SelectedCulture = selectedCultureName;
            if (!String.IsNullOrWhiteSpace(this.SelectedCulture))
            {
                this.SelectedCultureResources = await this.ResourceClientService!
                    .GetResourcesByCultureNameAsync(this.SelectedCulture!, base.CancellationToken);
            }
        }
    }
}
