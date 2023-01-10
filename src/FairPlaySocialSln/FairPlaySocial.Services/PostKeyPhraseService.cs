using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Services
{
    [ServiceOfEntity(entityName: nameof(PostKeyPhrase), primaryKeyType: typeof(long))]
    public partial class PostKeyPhraseService
    {
        public async Task<IEnumerable<PostKeyPhrase>?> CreatePostKeyPhrasesAsync(long postId,
            IEnumerable<string> postKeyPhrases, CancellationToken cancellationToken)
        {
            var postKeyPhrasesEntities = postKeyPhrases.Select(p => new PostKeyPhrase() 
            {
                PostId = postId,
                Phrase = p
            });
            await this._fairplaysocialDatabaseContext.PostKeyPhrase.AddRangeAsync(
                postKeyPhrasesEntities, cancellationToken);
            await this._fairplaysocialDatabaseContext.SaveChangesAsync(cancellationToken);
            return postKeyPhrasesEntities;
        }
    }
}
