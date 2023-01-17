using AutoMapper;
using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.UserMessage;
using FairPlaySocial.Notifications.Hubs.Post;
using FairPlaySocial.Notifications.Hubs.UserMessage;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static FairPlaySocial.Common.Global.Constants;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles User Messages
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class UserMessageController : ControllerBase
    {
        private ICurrentUserProvider currentUserProvider;
        private readonly IMapper mapper;
        private readonly IHubContext<UserMessageNotificationHub, IUserMessageNotificationHub> hubContext;
        private readonly UserMessageService userMessageService;

        /// <summary>
        /// <see cref="UserMessageController"/> constructor
        /// </summary>
        /// <param name="currentUserProvider"><see cref="ICurrentUserProvider"/> instance</param>
        /// <param name="mapper"> <see cref="IMapper"/> instance</param>
        /// <param name="hubContext">hub context</param>
        /// <param name="userMessageService"><see cref="UserMessage"/> instance</param>
        public UserMessageController(
            ICurrentUserProvider currentUserProvider,
            IMapper mapper,
            IHubContext<UserMessageNotificationHub, IUserMessageNotificationHub> hubContext,
        UserMessageService userMessageService)
        {
            this.currentUserProvider = currentUserProvider;
            this.mapper = mapper;
            this.hubContext = hubContext;
            this.userMessageService = userMessageService;
        }

        /// <summary>
        /// Creates a new message
        /// </summary>
        /// <param name="applicationUserService"></param>
        /// <param name="createUserMessageModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<UserMessageModel> CreateUserMessageAsync(
            [FromServices] ApplicationUserService applicationUserService,
            CreateUserMessageModel createUserMessageModel,
            CancellationToken cancellationToken)
        {
            var receiverEntity = await applicationUserService
                .GetApplicationUserByIdAsync(createUserMessageModel.ToApplicationUserId!.Value,
                trackEntities: false, cancellationToken: cancellationToken);
            if (receiverEntity is null)
                throw new CustomValidationException($"Unable to find user with id: {createUserMessageModel.ToApplicationUserId}");
            var entity = this.mapper.Map<CreateUserMessageModel, UserMessage>(createUserMessageModel);
            entity.FromApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            entity = await this.userMessageService.CreateUserMessageAsync(entity, cancellationToken);
            await this.hubContext.Clients
                .User(receiverEntity.AzureAdB2cobjectId.ToString())
                .ReceiveMessage(new Models.Notifications.UserMessageNotificationModel()
                {
                    Message = createUserMessageModel.Message
                });
            var result = this.mapper.Map<UserMessage, UserMessageModel>(entity);
            return result;
        }
    }
}
