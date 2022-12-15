using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Services
{
    [ServiceOfEntity(nameof(Post), primaryKeyType: typeof(long))]
    public partial class PostService
    {
        public IQueryable<Post>? GetPostHistoryByPostId(long postId)
        {
            var query = this._fairplaysocialDatabaseContext.Post
                .TemporalAll().Where(p => p.PostId == postId);
            return query;
        }
    }
}
