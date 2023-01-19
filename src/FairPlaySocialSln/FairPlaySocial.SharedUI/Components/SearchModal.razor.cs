using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Search;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.SharedUI.Components
{
    public partial class SearchModal
    {
        [Inject]
        private INavigationService? NavigationService { get; set; }
        private readonly SearchModel searchModel = new();
        private bool ShowSearchModal { get; set; } = false;

        private void OpenSearchModal()
        {
            this.ShowSearchModal = true;
        }

        private void HideSearchModal()
        {
            this.ShowSearchModal = false;
        }

        private void OnSearch()
        {
            this.HideSearchModal();
            switch (this.searchModel.SearchType)
            {
                case SearchType.UserProfiles:
                    this.NavigationService!.NavigateToSearchUserProfiles(searchTerm: this.searchModel.SearchTerm!);
                    break;
                case SearchType.Posts:
                    this.NavigationService!.NavigateToSearchPosts(searchTerm: this.searchModel.SearchTerm!);
                    break;
                case SearchType.Groups:
                    this.NavigationService!.NavigateToSearchGroups(searchTerm: this.searchModel.SearchTerm!);
                    break;
            }
        }
    }
}
