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
        private readonly GroupMemberService groupMemberService;

        public MyGroupController(ICurrentUserProvider currentUserProvider, IMapper mapper,
            GroupService groupService, GroupMemberService groupMemberService)
        {
            this.currentUserProvider = currentUserProvider;
            this.mapper = mapper;
            this.groupService = groupService;
            this.groupMemberService = groupMemberService;
        }

        [HttpPost("[action]")]
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
            groupEntity.OwnerApplicationUserId = this.currentUserProvider.GetApplicationUserId();
            groupEntity = await this.groupService.CreateGroupAsync(groupEntity, cancellationToken);
            var result = this.mapper.Map<Group, GroupModel>(groupEntity);
            return result;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> JoinGroupAsync(long groupId, CancellationToken cancellationToken)
        {
            if (!await this.groupService
                .GetAllGroup(trackEntities: false, cancellationToken: cancellationToken)
                .Where(p => p.GroupId == groupId)
                .AnyAsync(cancellationToken: cancellationToken))
            {
                throw new CustomValidationException($"Unable to find a group with id {groupId}");
            }
            if (await this.groupMemberService
                .GetAllGroupMember(trackEntities: false,
                cancellationToken: cancellationToken)
                .Where(p => p.GroupId == groupId && p.MemberApplicationUserId == this.currentUserProvider.GetApplicationUserId())
                .AnyAsync(cancellationToken: cancellationToken))
            {
                throw new CustomValidationException($"User is already a member of Group with id: {groupId}");
            }
            GroupMember groupMemberEntity = new GroupMember()
            {
                GroupId = groupId,
                MemberApplicationUserId = this.currentUserProvider.GetApplicationUserId()
            };
            await this.groupMemberService.CreateGroupMemberAsync(groupMemberEntity, cancellationToken);
            return Ok();
        }
    }
}
