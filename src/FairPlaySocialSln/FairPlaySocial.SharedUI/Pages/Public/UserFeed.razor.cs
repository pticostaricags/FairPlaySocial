using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.Post;
using FairPlaySocial.SharedUI.Pages.User.Feeds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.SharedUI.Pages.Public
{
    [AllowAnonymous]
    [Route($"{Common.Global.Constants.MauiBlazorAppPages.PublicPagesRoutes.UserFeed}/{{{nameof(ApplicationUserId)}:long}}")]
    public partial class UserFeed
    {
        [Parameter]
        public long? ApplicationUserId { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        [Inject]
        private PublicFeedClientService? PublicFeedClientService { get; set; }
        private bool IsBusy { get; set; }
        private PageRequestModel PageRequestModel { get; set; } = new PageRequestModel()
        {
            PageNumber = 1
        };

        private PagedItems<PostModel>? UserPublicFeed { get; set; }

        private List<PostModel>? PostModels { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsBusy = true;
                this.UserPublicFeed = null;
                StateHasChanged();
                this.UserPublicFeed = await this.PublicFeedClientService!
                    .GetUserFeedAsync(this.ApplicationUserId!.Value, this.PageRequestModel, base.CancellationToken);
                this.PostModels = UserPublicFeed!.Items!.ToList();
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnPreviousPageButtonCllickedAsync()
        {
            this.PageRequestModel.PageNumber--;
            await LoadDataAsync();
        }
        private async Task OnNextPageButtonClickedAsync()
        {
            this.PageRequestModel.PageNumber++;
            await LoadDataAsync();
        }
    }
}
