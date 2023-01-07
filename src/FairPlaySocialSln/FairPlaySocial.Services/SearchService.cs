using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Services
{
    public class SearchService
    {
        private readonly FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext;

        public SearchService(FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext)
        {
            this.fairPlaySocialDatabaseContext = fairPlaySocialDatabaseContext;
        }

        public IQueryable<UserProfile> SearchUserProfiles(string searchTerm,
            bool trackEntities)
        {
            var query = this.fairPlaySocialDatabaseContext.UserProfile.Where(p => p.Bio.Contains(searchTerm));
            if (!trackEntities)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<Post> SearchPosts(string searchTerm, bool trackEntities)
        {
            var query = this.fairPlaySocialDatabaseContext.Post.Where(p => p.Text.Contains(searchTerm));
            if (!trackEntities)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<Group> SearchGroups(string searchTerm, bool trackEntities)
        {
            var query = this.fairPlaySocialDatabaseContext.Group.Where(p => p.Name.Contains(searchTerm));
            if (!trackEntities)
                query = query.AsNoTracking();
            return query;
        }
    }
}
