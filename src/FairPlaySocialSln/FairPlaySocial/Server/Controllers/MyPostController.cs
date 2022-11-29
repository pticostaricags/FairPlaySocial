using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.CustomExceptions;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Notifications.Hubs;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyPostController : ControllerBase
    {
        private IMapper mapper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly PostService postService;
        private readonly IHubContext<NotificationHub, INotificationHub> hubContext;
        public MyPostController(IMapper mapper,
            ICurrentUserProvider currentUserProvider, PostService postService,
            IHubContext<NotificationHub, INotificationHub> hubContext)
        {
            this.mapper = mapper;
            this.currentUserProvider = currentUserProvider;
            this.postService = postService;
            this.hubContext = hubContext;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateMyPostAsync(
            [FromServices] ApplicationUserService applicationUserService,
            [FromServices] ForbiddenUrlService forbiddenUrlService,
            [FromServices] HttpClient httpClient,
            CreatePostModel createPostModel,
            CancellationToken cancellationToken)
        {
            if (!String.IsNullOrWhiteSpace(createPostModel.Url))
            {
                //Check https://stackoverflow.com/questions/2569851/how-to-expand-urls-in-c
                var response = await httpClient.GetAsync(createPostModel.Url);
                string redirectUrl = String.Empty;
                if (
                    (response.StatusCode == System.Net.HttpStatusCode.Redirect ||
                    response.StatusCode == System.Net.HttpStatusCode.Moved) &&
                    !String.IsNullOrWhiteSpace(response.Headers.Location?.AbsoluteUri))
                {
                    redirectUrl = response.Headers.Location!.AbsoluteUri;
                }
                var isForbiddeUrl = await forbiddenUrlService
                    .GetAllForbiddenUrl(trackEntities: false, cancellationToken: cancellationToken)
                    .Where(p => EF.Functions.Like(createPostModel.Url, "%" + p.Url + "%")
                    || EF.Functions.Like(redirectUrl, "%" + p.Url + "%"))
                    .AnyAsync(cancellationToken: cancellationToken);
                if (isForbiddeUrl)
                {
                    throw new CustomValidationException($"Forbidden url: {createPostModel.Url}");
                }
            }
            var entity = this.mapper.Map<CreatePostModel, Post>(createPostModel);
            entity.OwnerApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            entity =
            await this.postService.CreatePostAsync(entity, cancellationToken);
            var post = this.mapper.Map<Post, PostModel>(entity);
            //TODO: Consider using groups to send only to users in the "Home Feed" page
            var userEntity = await applicationUserService.GetApplicationUserByIdAsync(this.currentUserProvider.GetApplicationUserId(), trackEntities: false, cancellationToken: cancellationToken);
            post.OwnerApplicationUserFullName = userEntity.FullName;
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.NotificationModel()
            {
                From = userEntity.FullName,
                GroupName = null,
                Message = entity.Text,
                Post = post
            });
            return Ok();
        }
    }
}
