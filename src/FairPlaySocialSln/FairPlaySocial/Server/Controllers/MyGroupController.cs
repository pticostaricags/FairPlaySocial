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
    /// <summary>
    /// My group management.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class MyGroupController : ControllerBase
    {
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IMapper mapper;
        private readonly GroupService groupService;
        private readonly GroupMemberService groupMemberService;

        /// <summary>
        /// <see cref="MyGroupController"/> constructor.
        /// </summary>
        /// <param name="currentUserProvider"><see cref="ICurrentUserProvider"/> instance.</param>
        /// <param name="mapper"><see cref="IMapper"/> instance.</param>
        /// <param name="groupService"><see cref="GroupService"/> instance.</param>
        /// <param name="groupMemberService"><see cref="GroupMemberService"/> instance.</param>
        public MyGroupController(ICurrentUserProvider currentUserProvider, IMapper mapper,
            GroupService groupService, GroupMemberService groupMemberService)
        {
            this.currentUserProvider = currentUserProvider;
            this.mapper = mapper;
            this.groupService = groupService;
            this.groupMemberService = groupMemberService;
        }

        /// <summary>
        /// Creates new group.
        /// </summary>
        /// <param name="createGroupModel"><see cref="CreateGroupModel"/> instance, representing new group info.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="GroupModel"/> instance representing newly created group.</returns>
        /// <exception cref="CustomValidationException"></exception>
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
            groupEntity.GroupMember.Add(new GroupMember()
            {
                MemberApplicationUserId = groupEntity.OwnerApplicationUserId
            });
            groupEntity = await this.groupService.CreateGroupAsync(groupEntity, cancellationToken);
            var result = this.mapper.Map<Group, GroupModel>(groupEntity);
            return result;
        }

        /// <summary>
        /// Joins to the existing group.
        /// </summary>
        /// <param name="groupId">Group id to join to.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="IActionResult"/> instance.</returns>
        /// <exception cref="CustomValidationException"></exception>
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
            GroupMember groupMemberEntity = new()
            {
                GroupId = groupId,
                MemberApplicationUserId = this.currentUserProvider.GetApplicationUserId()
            };
            await this.groupMemberService.CreateGroupMemberAsync(groupMemberEntity, cancellationToken);
            return Ok();
        }
    }
}
