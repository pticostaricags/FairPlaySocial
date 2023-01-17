using AutoMapper;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.UserMessage;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles User Messages
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserMessageController : ControllerBase
    {
        private ICurrentUserProvider currentUserProvider;
        private readonly IMapper mapper;
        private readonly UserMessageService userMessageService;

        /// <summary>
        /// <see cref="UserMessageController"/> constructor
        /// </summary>
        /// <param name="currentUserProvider"><see cref="ICurrentUserProvider"/> instance</param>
        /// <param name="mapper"> <see cref="IMapper"/> instance</param>
        /// <param name="userMessageService"><see cref="UserMessage"/> instance</param>
        public UserMessageController(
            ICurrentUserProvider currentUserProvider,
            IMapper mapper,
            UserMessageService userMessageService)
        {
            this.currentUserProvider = currentUserProvider;
            this.mapper = mapper;
            this.userMessageService = userMessageService;
        }

        /// <summary>
        /// Creates a new message
        /// </summary>
        /// <param name="createUserMessageModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<UserMessageModel> CreateUserMessageAsync(CreateUserMessageModel createUserMessageModel,
            CancellationToken cancellationToken)
        {
            var entity = this.mapper.Map<CreateUserMessageModel, UserMessage>(createUserMessageModel);
            entity.FromApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            entity = await this.userMessageService.CreateUserMessageAsync(entity, cancellationToken);
            var result = this.mapper.Map<UserMessage, UserMessageModel>(entity);
            return result;
        }
    }
}
