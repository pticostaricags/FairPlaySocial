using AutoMapper;
using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Group;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyGroupController : ControllerBase
    {
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IMapper mapper;
        private readonly GroupService groupService;

        public MyGroupController(ICurrentUserProvider currentUserProvider, IMapper mapper, GroupService groupService)
        {
            this.currentUserProvider = currentUserProvider;
            this.mapper = mapper;
            this.groupService = groupService;
        }

        [HttpPost("action")]
        public async Task<GroupModel?> CreateMyGroupAsync(CreateGroupModel createGroupModel,
            CancellationToken cancellationToken)
        {
            if (await this.groupService.GetAllGroup(trackEntities: false,
                cancellationToken: cancellationToken)
                .Where(p => p.Name == createGroupModel.Name).AnyAsync(cancellationToken: cancellationToken))
            {
                throw new CustomValidationException($"Unable to create group {createGroupModel.Name}. That name is already being used");
            }
            Group groupEntity = this.mapper.Map<CreateGroupModel, Group>(createGroupModel);
            groupEntity = await this.groupService.CreateGroupAsync(groupEntity, cancellationToken);
            var result = this.mapper.Map<Group, GroupModel>(groupEntity);
            return result;
        }
    }
}
