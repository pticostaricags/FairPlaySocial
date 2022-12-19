using FairPlaySocial.Models.Post;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.SharedUI.Pages.Public
{
    public partial class Post
    {
        [Parameter]
        public long? PostId { get; set; }
        private bool IsBusy { get; set; } = false;
        private PostModel? PostModel { get; set; }
    }
}
