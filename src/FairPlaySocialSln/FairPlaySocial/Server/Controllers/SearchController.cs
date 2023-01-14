using AutoMapper;
using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Group;
using FairPlaySocial.Models.Pagination;
using FairPlaySocial.Models.Post;
using FairPlaySocial.Models.UserProfile;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FairPlaySocial.Server.Controllers
{
    /// <summary>
    /// Handles search operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.Roles.User)]
    public class SearchController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly SearchService searchService;

        /// <summary>
        /// <see cref="SearchController"/> constructor.
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/> instance.</param>
        /// <param name="searchService"><see cref="SearchService"/> instance.</param>
        public SearchController(IMapper mapper, SearchService searchService)
        {
            this.mapper = mapper;
            this.searchService = searchService;
        }

        /// <summary>
        /// Searches user profile by page info and search criteria.
        /// </summary>
        /// <param name="pageRequestModel"><see cref="PageRequestModel"/> instance, page info.</param>
        /// <param name="searchTerm">Serialized search criteria.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged <see cref="UserProfileModel">user profiles</see>.</returns>
        [HttpPost("[action]")]
        public async Task<PagedItems<UserProfileModel>> SearchUserProfilesAsync(
            [FromQuery] PageRequestModel pageRequestModel,
            [FromQuery] string searchTerm,
            CancellationToken cancellationToken)
        {
            var query = this.searchService.SearchUserProfiles(searchTerm, trackEntities: false)
                .Include(p=>p.ApplicationUser);
            PagedItems<UserProfileModel> result = new PagedItems<UserProfileModel>();
            result.PageSize = Constants.Pagination.DefaultPageSize;
            result.PageNumber = pageRequestModel.PageNumber;
            result.TotalItems = await query.CountAsync(cancellationToken);
            result.TotalPages = (int)Math.Ceiling((double)result.TotalItems / Constants.Pagination.DefaultPageSize);
            result.Items = await query.OrderByDescending(p => p.ApplicationUser.FullName)
                .Skip((pageRequestModel.PageNumber!.Value - 1) * Constants.Pagination.DefaultPageSize)
                .Take(Constants.Pagination.DefaultPageSize)
                .Select(p => this.mapper.Map<UserProfile, UserProfileModel>(p))
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }

        /// <summary>
        /// Searches posts by page info and search criteria.
        /// </summary>
        /// <param name="pageRequestModel"><see cref="PageRequestModel"/> instance, page info.</param>
        /// <param name="searchTerm">Serialized search criteria.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged <see cref="UserProfileModel">posts</see>.</returns>
        [HttpPost("[action]")]
        public async Task<PagedItems<PostModel>> SearchPostsAsync(
            [FromQuery] PageRequestModel pageRequestModel,
            [FromQuery] string searchTerm,
            CancellationToken cancellationToken)
        {
            var query = this.searchService.SearchPosts(searchTerm, trackEntities: false);
            PagedItems<PostModel> result = new PagedItems<PostModel>();
            result.PageSize = Constants.Pagination.DefaultPageSize;
            result.PageNumber = pageRequestModel.PageNumber;
            result.TotalItems = await query.CountAsync(cancellationToken);
            result.TotalPages = (int)Math.Ceiling((double)result.TotalItems / Constants.Pagination.DefaultPageSize);
            result.Items = await query.OrderByDescending(p => p.PostId)
                .Skip((pageRequestModel.PageNumber!.Value - 1) * Constants.Pagination.DefaultPageSize)
                .Take(Constants.Pagination.DefaultPageSize)
                .Select(p => this.mapper.Map<Post, PostModel>(p))
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }

        /// <summary>
        /// Searches groups by page info and search criteria.
        /// </summary>
        /// <param name="pageRequestModel"><see cref="PageRequestModel"/> instance, page info.</param>
        /// <param name="searchTerm">Serialized search criteria.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged <see cref="GroupModel">groups</see>.</returns>
        [HttpPost("[action]")]
        public async Task<PagedItems<GroupModel>> SearchGroupsAsync(
            [FromQuery] PageRequestModel pageRequestModel,
            [FromQuery] string searchTerm,
            CancellationToken cancellationToken)
        {
            var query = this.searchService.SearchGroups(searchTerm, trackEntities: false);
            PagedItems<GroupModel> result = new PagedItems<GroupModel>();
            result.PageSize = Constants.Pagination.DefaultPageSize;
            result.PageNumber = pageRequestModel.PageNumber;
            result.TotalItems = await query.CountAsync(cancellationToken);
            result.TotalPages = (int)Math.Ceiling((double)result.TotalItems / Constants.Pagination.DefaultPageSize);
            result.Items = await query.OrderByDescending(p => p.GroupId)
                .Skip((pageRequestModel.PageNumber!.Value - 1) * Constants.Pagination.DefaultPageSize)
                .Take(Constants.Pagination.DefaultPageSize)
                .Select(p => this.mapper.Map<Group, GroupModel>(p))
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
