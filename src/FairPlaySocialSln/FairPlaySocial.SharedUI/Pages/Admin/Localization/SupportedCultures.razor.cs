using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Culture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages.Admin.Localization
{
    [Authorize(Roles = Constants.Roles.Admin)]
    [Route(Constants.MauiBlazorAppPages.AdminRolePagesRoutes.SupportedCultures)]
    public partial class SupportedCultures
    {
        [Inject]
        private CultureClientService? CultureClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private bool IsBusy { get; set; }
        private CultureModel[]? AvailableCultures { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                this.IsBusy = true;
                this.AvailableCultures = await
                    this.CultureClientService!.GetAvailableCulturesAsync(base.CancellationToken);
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

        private async Task OnEnableCultureButtonClickAsync(string cultureName)
        {
            try
            {
                this.IsBusy = true;
                await this.CultureClientService!.CreateCultureAsync(new CreateCultureModel()
                {
                    Name = cultureName
                }, base.CancellationToken);
                await LoadDataAsync();
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
        }
    }
}
