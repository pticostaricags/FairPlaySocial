using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Server.CustomUserProvider;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using FairPlaySocial.Server.CustomLocalization.EF;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using FairPlaySocial.Server.Translations;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.Interceptors;
using PTI.Microservices.Library.Services;
using System.Threading.RateLimiting;
using System.Net;
using FairPlaySocial.Models.CustomHttpResponse;
using Microsoft.AspNetCore.RateLimiting;
using System.Reflection;
using FairPlaySocial.Notifications.Hubs.Post;
using FairPlaySocial.Notifications.Hubs.UserMessage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FairPlaySocial.ClientsConfiguration;
using Blazored.Toast;
using FairPlaySocial.Client.Services;
using FairPlaySocial.Common.Handlers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FairPlaySocial.Server
{
    /// <summary>
    /// Initializes and run the server.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initialized <see cref="Startup"/>
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Represents the system's initial/startup configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// <summary>
        /// Configures the System Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddScoped<LocalizationMessageHandler>();

            var faifairplaysocialApiAddress = Configuration["ApiBaseUrl"]!;
            services.AddHttpClient(
                        $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI",
                        client =>
                        {
                            client.BaseAddress = new Uri(faifairplaysocialApiAddress);
                            client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                            client.DefaultRequestVersion = HttpVersion.Version30;
                        })
                .AddHttpMessageHandler<LocalizationMessageHandler>()
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler(PollyHelper.GetRetryPolicy());

            services.AddHttpClient(
                $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI.Anonymous",
                client =>
                {
                    client.BaseAddress = new Uri(faifairplaysocialApiAddress);
                    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                    client.DefaultRequestVersion = HttpVersion.Version30;
                })
                .AddHttpMessageHandler<LocalizationMessageHandler>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler(PollyHelper.GetRetryPolicy());
            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(
                $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI"));

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI.Anonymous"));

            services.ConfigurePlatformOutputCache();
            services.ConfigurePlatformRateLimiter();
            services.AddSingleton<IStringLocalizerFactory, EFStringLocalizerFactory>();
            services.AddSingleton<IStringLocalizer, EFStringLocalizer>();
            services.AddLocalization();

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.TryAddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
            services.AddBlazoredToast();
            services.AddTransient<IToastService, ToastService>();
            services.AddTransient<ITextToSpeechService, TextToSpeechService>();
            services.AddTransient<IGeoLocationService, BlazorGeoLocationService>();
            services.AddTransient<ICultureSelectionService, BlazorCultureSelectionService>();
            services.AddTransient<IAnalyticsService, BlazorAnalyticsService>();
            services.AddMultiPlatformServices();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {

                var basePath = AppContext.BaseDirectory;
                var mainAppXmlFilename = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                var mainAppXmlPath = Path.Combine(basePath, mainAppXmlFilename);
                if (File.Exists(mainAppXmlPath))
                {
                    c.IncludeXmlComments(mainAppXmlPath, includeControllerXmlComments: true);
                }

                var modelsFileName = typeof(FairPlaySocial.Models.ApplicationUser.ApplicationUserModel).Assembly.GetName().Name + ".xml";
                var modelsXmlPath = Path.Combine(basePath, modelsFileName);
                if (File.Exists(modelsXmlPath))
                {
                    c.IncludeXmlComments(modelsXmlPath);
                }

                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = $"{Common.Global.Constants.Assemblies.MainAppAssemblyName} API"
                });
            });
            services.AddAutoMapper(configAction =>
            {
                configAction.AddMaps(new[] { typeof(Program).Assembly });
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<ICurrentUserProvider, CurrentUserProvider>();

            services.AddTransient<IEmailService, EmailService>();
            services.AddSignalR(hubOptions =>
            {
                hubOptions.MaximumReceiveMessageSize = 20 * 1024 * 1024;
            });
            services.AddPlatformServices(this.Configuration);

            services.AddTransient(serviceProvider =>
            {
                var currentUserProvider = serviceProvider.GetRequiredService<ICurrentUserProvider>();
                DbContextOptionsBuilder<FairPlaySocialDatabaseContext> dbContextOptionsBuilder =
                new();
                FairPlaySocialDatabaseContext? fairplaysocialDatabaseContext = null;
                bool useInMemoryDatabase = Convert.ToBoolean(Configuration["UseInMemoryDatabase"]);
                if (useInMemoryDatabase)
                {
                    dbContextOptionsBuilder
                    .UseInMemoryDatabase(
                        FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName);
                    fairplaysocialDatabaseContext =
                new(dbContextOptionsBuilder.Options, currentUserProvider);
                    fairplaysocialDatabaseContext.Database.EnsureCreated();
                    var userRole = new ApplicationRole()
                    {
                        ApplicationRoleId = 1,
                        Name = "User",
                        Description = "Normal Users"
                    };
                    if (!fairplaysocialDatabaseContext.ApplicationRole.Any(p => p.ApplicationRoleId == userRole.ApplicationRoleId))
                    {
                        fairplaysocialDatabaseContext.ApplicationRole.Add(userRole);
                        fairplaysocialDatabaseContext.SaveChanges();
                    }
                    var adminRole = new ApplicationRole()
                    {
                        ApplicationRoleId = 2,
                        Name = "Admin",
                        Description = "Admin Users"
                    };
                    if (!fairplaysocialDatabaseContext.ApplicationRole.Any(p => p.ApplicationRoleId == adminRole.ApplicationRoleId))
                    {
                        fairplaysocialDatabaseContext.ApplicationRole.Add(adminRole);
                        fairplaysocialDatabaseContext.SaveChanges();
                    }
                    var enCulture = new DataAccess.Models.Culture()
                    {
                        CultureId = 1,
                        Name = "en",
                    };
                    if (!fairplaysocialDatabaseContext.Culture.Any(p => p.CultureId == enCulture.CultureId))
                    {
                        fairplaysocialDatabaseContext.Culture.Add(enCulture);
                        fairplaysocialDatabaseContext.SaveChanges();
                    }
                }
                else
                {
                    dbContextOptionsBuilder.UseSqlServer(Configuration.GetConnectionString("Default"),
                        sqlServerOptionsAction =>
                        {
                            sqlServerOptionsAction.UseNetTopologySuite();
                            sqlServerOptionsAction.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 3,
                                maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                        });
                    fairplaysocialDatabaseContext =
                    new(dbContextOptionsBuilder.Options, currentUserProvider);
                }
                return fairplaysocialDatabaseContext;
            });
            services.AddHealthChecks()
                .AddDbContextCheck<FairPlaySocialDatabaseContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAdB2C"));

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "Role";
                options.SaveToken = true;
                options.Events.OnMessageReceived = (context) =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                    (    
                    path.StartsWithSegments(FairPlaySocial.Common.Global.Constants.Hubs.HomeFeedHub)
                    ||
                    path.StartsWithSegments(FairPlaySocial.Common.Global.Constants.Hubs.UserMessageHub)
                    )
                    )
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                };
                options.Events.OnTokenValidated = async (context) =>
                {
                    FairPlaySocialDatabaseContext fairplaysocialDatabaseContext =
                 context.HttpContext.RequestServices.GetRequiredService<FairPlaySocialDatabaseContext>();
                    ClaimsIdentity claimsIdentity = (context.Principal!.Identity as ClaimsIdentity)!;
                    var userObjectIdClaim = claimsIdentity.Claims.Single(p => p.Type ==
                    FairPlaySocial.Common.Global.Constants.Claims.ObjectIdentifier);
                    var user = await fairplaysocialDatabaseContext.ApplicationUser
                    .Include(p => p.ApplicationUserRole)
                    .ThenInclude(p => p.ApplicationRole)
                    .Where(p => p.AzureAdB2cobjectId.ToString() == userObjectIdClaim.Value)
                    .SingleOrDefaultAsync();
                    var fullName = claimsIdentity?.FindFirst(FairPlaySocial.Common.Global.Constants.Claims.Name)?.Value;
                    var emailAddress = claimsIdentity?.FindFirst(FairPlaySocial.Common.Global.Constants.Claims.Emails)?.Value;
                    if (user != null && user.ApplicationUserRole != null)
                    {
                        foreach (var singleRole in user.ApplicationUserRole)
                        {
                            claimsIdentity?.AddClaim(new Claim("Role", singleRole.ApplicationRole.Name));
                        }
                        user.FullName = fullName;
                        user.EmailAddress = emailAddress;
                        user.LastLogIn = DateTimeOffset.UtcNow;
                        await fairplaysocialDatabaseContext.SaveChangesAsync();

                    }
                    else
                    {
                        if (user == null)
                        {
                            var userRole = await fairplaysocialDatabaseContext.ApplicationRole
                            .SingleAsync(p => p.Name == FairPlaySocial.Common.Global.Constants.Roles.User);
                            user = new ApplicationUser()
                            {
                                LastLogIn = DateTimeOffset.UtcNow,
                                FullName = fullName,
                                EmailAddress = emailAddress,
                                AzureAdB2cobjectId = Guid.Parse(userObjectIdClaim.Value)
                            };
                            await fairplaysocialDatabaseContext.ApplicationUser.AddAsync(user);
                            await fairplaysocialDatabaseContext.SaveChangesAsync();
                            await fairplaysocialDatabaseContext.ApplicationUserRole.AddAsync(new ApplicationUserRole()
                            {
                                ApplicationUserId = user.ApplicationUserId,
                                ApplicationRoleId = userRole.ApplicationRoleId
                            });
                            await fairplaysocialDatabaseContext.SaveChangesAsync();
                            foreach (var singleRole in user.ApplicationUserRole)
                            {
                                claimsIdentity?.AddClaim(new Claim("Role", singleRole.ApplicationRole.Name));
                            }
                            var adminUsers = await fairplaysocialDatabaseContext.ApplicationUser
                            .Include(p => p.ApplicationUserRole).ThenInclude(p => p.ApplicationRole)
                            .Where(p => p.ApplicationUserRole.Any(aur => aur.ApplicationRole.Name ==
                              FairPlaySocial.Common.Global.Constants.Roles.Admin))
                            .ToListAsync();
                            var emailService = context.HttpContext.RequestServices
                            .GetRequiredService<IEmailService>();
                            foreach (var singleAdminUser in adminUsers)
                            {
                                await emailService.SendEmailAsync(toEmailAddress:
                                    singleAdminUser.EmailAddress, subject:
                                    $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName} - New User",
                                    body:
                                    $"<p>A new user has been created at: {context.Request.Host}</p>" +
                                    $"<p>Name: {user.FullName}</p>" +
                                    $"<p>Email: {user.EmailAddress}</p>",
                                    isBodyHtml: true, cancellationToken: CancellationToken.None);
                            }
                        }
                    }
                    claimsIdentity?.AddClaim(new Claim(
                            FairPlaySocial.Common.Global.Constants.Claims.ApplicationUserId,
                            user.ApplicationUserId.ToString()));
                };
            });
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            var shouldEnabledPTILibraries = Convert.ToBoolean(Configuration["EnablePTILibraries"]);
            if (shouldEnabledPTILibraries)
            {
                GlobalPackageConfiguration.RapidApiKey = Configuration["RapidApiKey"];
                GlobalPackageConfiguration.EnableHttpRequestInformationLog = false;

                services.AddTransient<CustomHttpClientHandler>();
                services.AddTransient<CustomHttpClient>();

                var azureTranslatorConfiguration =
                    this.Configuration.GetSection(nameof(AzureTranslatorConfiguration))
                    .Get<AzureTranslatorConfiguration>();
                if (azureTranslatorConfiguration != null)
                {
                    services.AddTransient<TranslationService>();
                    services.AddSingleton(azureTranslatorConfiguration);
                    services.AddTransient<AzureTranslatorService>();
                    //services.AddHostedService<BackgroundTranslationService>();
                }

                services.ConfigureAzureTextAnalyticsService(this.Configuration);
                services.AddTransient<TextAnalyticsService>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configure the Application Behavior and pipleline execution
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="sp"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IServiceProvider sp)
        {
            ConfigureRequestLocalization(app, sp);

            app.UseResponseCompression();
            ExceptionsHelper.HandleExceptions(app);
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOutputCache();
            app.UseRateLimiter();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<PostNotificationHub>(FairPlaySocial.Common.Global.Constants.Hubs.HomeFeedHub);
                endpoints.MapHub<UserMessageNotificationHub>(FairPlaySocial.Common.Global.Constants.Hubs.UserMessageHub);
                endpoints.MapHealthChecks("/health");
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private static void ConfigureRequestLocalization(IApplicationBuilder app, IServiceProvider sp)
        {
            var dbContext = sp.GetRequiredService<FairPlaySocialDatabaseContext>();

            var supportedCultures = dbContext.Culture.Select(p => new CultureInfo(p.Name)).ToList();
            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures.First().Name),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            app.UseRequestLocalization(options);
        }
    }
}
