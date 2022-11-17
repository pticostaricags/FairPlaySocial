using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.ApplicationUser;
using FairPlaySocial.Models.Filtering;
using FairPlaySocial.Models.FilteringSorting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Reflection;

namespace FairPlaySocial.SharedUI.Pages.Admin.Users
{
    [Authorize(Roles = $"{Constants.Roles.Admin}")]
    [Route(Constants.MauiBlazorAppPages.AdminRolePagesRoutes.UserList)]
    public partial class UserList
    {
        [Inject]
        private ApplicationUserClientService? ApplicationUserClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private bool IsLoading { get; set; }
        private IEnumerable<string>? PropertyNames { get; set; }
        public ApplicationUserModel[]? FilteredApplicationUsers { get; private set; }

        private FilteringSortingModel filteringSortingModel = new()
        {
            Filtering = new()
            {
                ComparisonOperator = ComparisonOperator.Equals
            },
            Sorting=new Models.Sorting.SortingModel[] 
            {
                new Models.Sorting.SortingModel()
                {
                    ColumnName = nameof(ApplicationUserModel.FullName)
                }
            }
        };

        protected override void OnInitialized()
        {
            this.PropertyNames = typeof(ApplicationUserModel).GetProperties().Select(p => p.Name);
        }

        private async Task OnValidSubmitAsync()
        {
            try
            {
                IsLoading = true;
                this.FilteredApplicationUsers =
                    await this.ApplicationUserClientService!
                    .GetFilteredApplicationUserAsync(this.filteringSortingModel, base.CancellationToken);
            }
            catch (Exception ex)
            {
                await ToastService!.ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
