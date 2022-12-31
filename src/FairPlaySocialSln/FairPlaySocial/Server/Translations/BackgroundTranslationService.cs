using FairPlaySocial.Common.CustomAttributes.Localization;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Services;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.Models.AzureTranslator.Translate;
using System.Reflection;

namespace FairPlaySocial.Server.Translations
{
    /// <summary>
    /// 
    /// </summary>
    public class BackgroundTranslationService : BackgroundService
    {
        private readonly IServiceScopeFactory ServiceScopeFactory;

        private ILogger<BackgroundTranslationService> Logger { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="logger"></param>
        public BackgroundTranslationService(IServiceScopeFactory serviceScopeFactory,
            ILogger<BackgroundTranslationService> logger)
        {
            this.ServiceScopeFactory = serviceScopeFactory;
            this.Logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await ProcessAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(exception: ex, message: ex.Message);
            }
        }

        private async Task ProcessAsync(CancellationToken stoppingToken)
        {
            using var scope = this.ServiceScopeFactory.CreateScope();
            FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext =
                scope.ServiceProvider.GetRequiredService<FairPlaySocialDatabaseContext>();
            var translationService =
                scope.ServiceProvider.GetRequiredService<TranslationService>();
            var clientAppAssembly = typeof(Client.App).Assembly;
            var clientAppTypes = clientAppAssembly.GetTypes();

            var sharedUIAssembly = typeof(SharedUI._Imports).Assembly;
            var componentsTypes = sharedUIAssembly.GetTypes();

            var modelsAssembly = typeof(Models.ApplicationUser.ApplicationUserModel).Assembly;
            var modelsTypes = modelsAssembly.GetTypes();

            var servicesAssembly = typeof(TranslationService).Assembly;
            var servicesTypes = servicesAssembly.GetTypes();

            var commonAssembly = typeof(Common.Global.Constants).Assembly;
            var commonTypes = commonAssembly.GetTypes();

            var multiPlatformComponentsAssembly = typeof(MultiplatformComponents._Imports).Assembly;
            var multiPlatformTypes = multiPlatformComponentsAssembly.GetTypes();

            List<Type> typesToCheck = new();
            typesToCheck.AddRange(clientAppTypes);
            typesToCheck.AddRange(componentsTypes);
            typesToCheck.AddRange(modelsTypes);
            typesToCheck.AddRange(servicesTypes);
            typesToCheck.AddRange(commonTypes);
            typesToCheck.AddRange(multiPlatformTypes);

            foreach (var singleTypeToCheck in typesToCheck)
            {
                string typeFullName = singleTypeToCheck.FullName!;
                var fields = singleTypeToCheck.GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy
                    );
                foreach (var singleField in fields)
                {
                    var resourceKeyAttributes =
                        singleField.GetCustomAttributes<ResourceKeyAttribute>();
                    if (resourceKeyAttributes != null && resourceKeyAttributes.Any())
                    {
                        ResourceKeyAttribute keyAttribute = resourceKeyAttributes.Single();
                        var defaultValue = keyAttribute.DefaultValue;
                        string key = singleField.GetRawConstantValue()!.ToString()!;
                        var entity =
                            await fairPlaySocialDatabaseContext.Resource
                            .SingleOrDefaultAsync(p => p.CultureId == 1 &&
                            p.Key == key &&
                            p.Type == typeFullName, stoppingToken);
                        if (entity is null)
                        {
                            entity = new Resource()
                            {
                                CultureId = 1,
                                Key = key,
                                Type = typeFullName,
                                Value = keyAttribute.DefaultValue
                            };
                            await fairPlaySocialDatabaseContext.Resource
                                .AddAsync(entity, stoppingToken);
                        }
                    }
                }
            }
            if (fairPlaySocialDatabaseContext.ChangeTracker.HasChanges())
                await fairPlaySocialDatabaseContext.SaveChangesAsync(stoppingToken);
            var allEnglishUSKeys =
                await fairPlaySocialDatabaseContext.Resource
                .Include(p => p.Culture)
                .Where(p => p.Culture.Name == "en-US")
                .ToListAsync(stoppingToken);
            TranslateRequestTextItem[] translateRequestItems =
                allEnglishUSKeys.Select(p => new TranslateRequestTextItem()
                {
                    Text = p.Value
                }).ToArray();

            var additionalSupportedCultures = await fairPlaySocialDatabaseContext.Culture
                .Where(p => p.Name != "en-US").ToListAsync(cancellationToken: stoppingToken);
            foreach (var singleAdditionalCulture in additionalSupportedCultures)
            {
                var cultureTranslations = await
                    translationService.TranslateAsync(translateRequestItems,
                    "en",
                    singleAdditionalCulture.Name, stoppingToken);
                var cultureEntity = await fairPlaySocialDatabaseContext
                    .Culture.SingleAsync(p => p.Name == singleAdditionalCulture.Name, cancellationToken: stoppingToken);
                for (int iPos = 0; iPos < cultureTranslations.Length; iPos++)
                {
                    var singleEnglishUSKey = allEnglishUSKeys[iPos];
                    var translatedValue = cultureTranslations[iPos].translations.First().text;
                    var resourceEntity = await fairPlaySocialDatabaseContext.Resource
                        .SingleOrDefaultAsync(p => p.Key == singleEnglishUSKey.Key &&
                        p.Type == singleEnglishUSKey.Type &&
                        p.CultureId == cultureEntity.CultureId, cancellationToken: stoppingToken);

                    if (resourceEntity is null)
                    {
                        resourceEntity = new()
                        {
                            Key = singleEnglishUSKey.Key,
                            Type = singleEnglishUSKey.Type,
                            Value = translatedValue,
                            CultureId = cultureEntity.CultureId
                        };
                        await fairPlaySocialDatabaseContext.Resource.AddAsync(resourceEntity, stoppingToken);
                    }
                }
                if (fairPlaySocialDatabaseContext.ChangeTracker.HasChanges())
                    await fairPlaySocialDatabaseContext.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
