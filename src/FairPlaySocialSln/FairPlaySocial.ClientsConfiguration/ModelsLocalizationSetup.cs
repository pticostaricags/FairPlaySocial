using FairPlaySocial.Models.Group;
using FairPlaySocial.Models.Post;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientsConfiguration
{
    public class ModelsLocalizationSetup
    {
        public static void ConfigureModelsLocalizers(IServiceProvider services)
        {
            var localizerFactory = services.GetRequiredService<IStringLocalizerFactory>();
            CreatePostModelLocalizer.Localizer =
                localizerFactory.Create(typeof(CreatePostModelLocalizer))
                as IStringLocalizer<CreatePostModelLocalizer>;
            CreateGroupModelLocalizer.Localizer =
                localizerFactory.Create(typeof(CreateGroupModelLocalizer))
                as IStringLocalizer<CreateGroupModelLocalizer>;
        }
    }
}
