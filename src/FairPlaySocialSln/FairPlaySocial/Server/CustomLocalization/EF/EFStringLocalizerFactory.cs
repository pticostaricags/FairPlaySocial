using FairPlaySocial.DataAccess.Data;
using Microsoft.Extensions.Localization;

namespace FairPlaySocial.Server.CustomLocalization.EF
{
    /// <summary>
    /// Handles EF-based lcoalization
    /// </summary>
    public class EFStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly FairPlaySocialDatabaseContext _db;

        /// <summary>
        /// Initializes <see cref="EFStringLocalizerFactory"/>
        /// </summary>
        /// <param name="db"></param>
        public EFStringLocalizerFactory(FairPlaySocialDatabaseContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates the localizer
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <returns></returns>
        public IStringLocalizer Create(Type resourceSource)
        {
            var localizerType = typeof(EFStringLocalizer<>)
                .MakeGenericType(resourceSource);
            var instance = Activator.CreateInstance(localizerType, args: new object[] { _db }) as IStringLocalizer;
            return instance!;
        }

        /// <summary>
        /// Create the localizeer using the location
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            return new EFStringLocalizer(_db);
        }
    }
}
