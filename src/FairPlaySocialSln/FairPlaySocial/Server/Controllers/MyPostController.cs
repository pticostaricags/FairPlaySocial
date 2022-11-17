using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Notifications.Hubs;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

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
        public async Task<IActionResult> CreateMyPostAsync(CreatePostModel createPostModel,
            CancellationToken cancellationToken)
        {
            var entity = this.mapper.Map<CreatePostModel, Post>(createPostModel);
            entity.OwnerApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            await this.postService.CreatePostAsync(entity, cancellationToken);
            //TODO: Consider using groups to send only to users in the "Home Feed" page
            await hubContext.Clients.All.ReceiveMessage(new Models.Notifications.NotificationModel()
            {
                From = entity.OwnerApplicationUserId.ToString(),
                GroupName = null,
                Message = entity.Text
            });
            return Ok();
        }
    }
}
