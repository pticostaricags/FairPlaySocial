using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlaySocial.SharedUI.Pages.Public.Embedded
{
    [AllowAnonymous]
    [Route($"{Common.Global.Constants.MauiBlazorAppPages.PublicPagesRoutes.EmbeddedPost}/{{{nameof(PostId)}:long}}")]
    public partial class Post : IDisposable
    {
        [Parameter]
        public long? PostId { get; set; }
        [Inject]
        private PublicFeedClientService? PublicFeedClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        [Inject]
        private HttpClientService? HttpClientService { get; set; }
        [Inject]
        PersistentComponentState? ApplicationState { get; set; }
        private bool IsBusy { get; set; }
        public string? OgImageUrl { get; set; }
        private PostModel? PostModel { get; set; }
        private string? ErrorMessage { get; set; }
        private PersistingComponentStateSubscription persistingSubscription;
        private const string PERSIST_POST_KEY = "postModel";
        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.IsBusy = true;
                var apiBaseUrl = this.HttpClientService!.CreateAnonymousClient().BaseAddress;
                this.OgImageUrl = $"{apiBaseUrl}api/OpenGraph/Post/{this.PostId}/OgImage";
                //Check https://learn.microsoft.com/en-us/aspnet/core/blazor/components/prerendering-and-integration?view=aspnetcore-7.0&pivots=webassembly#persist-prerendered-state
                persistingSubscription = ApplicationState!
                    .RegisterOnPersisting(this.PersistPostModelAsync);

                if (!ApplicationState.TryTakeFromJson<PostModel>(
                    PERSIST_POST_KEY, out var restored))
                {
                    this.PostModel = await this.PublicFeedClientService!
                    .GetPostByPostIdAsync(this.PostId!.Value, base.CancellationToken);
                    if (this.PostModel is null)
                    {
                        this.ErrorMessage = $"Unable to find post with Id: {PostId}";
                        await this.ToastService!
                            .ShowErrorMessageAsync(this.ErrorMessage, base.CancellationToken);
                    }
                    else
                        this.ErrorMessage = null;
                }
                else
                {
                    this.PostModel = restored!;
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message, base.CancellationToken);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private Task PersistPostModelAsync()
        {
            ApplicationState!.PersistAsJson("postModel", this.PostModel);

            return Task.CompletedTask;
        }

        void IDisposable.Dispose()
        {
            persistingSubscription.Dispose();
        }
    }
}
