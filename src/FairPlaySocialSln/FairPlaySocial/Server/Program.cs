using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Notifications.Hubs;
using FairPlaySocial.Server;
using FairPlaySocial.Server.CustomUserProvider;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(configAction =>
{
    configAction.AddMaps(new[] { typeof(Program).Assembly });
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<ICurrentUserProvider, CurrentUserProvider>();

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSignalR();
AddPlatformServices(builder);

builder.Services.AddTransient(serviceProvider =>
{
    var currentUserProvider = serviceProvider.GetRequiredService<ICurrentUserProvider>();
    DbContextOptionsBuilder<FairPlaySocialDatabaseContext> dbContextOptionsBuilder =
    new();
    FairPlaySocialDatabaseContext? fairplaysocialDatabaseContext = null;
    bool useInMemoryDatabase = Convert.ToBoolean(builder.Configuration["UseInMemoryDatabase"]);
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
        if (!fairplaysocialDatabaseContext.ApplicationRole.Any(p=>p.ApplicationRoleId == userRole.ApplicationRoleId))
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
    }
    else
    {
        dbContextOptionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
            sqlServerOptionsAction =>
            {
                sqlServerOptionsAction.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
            });
        fairplaysocialDatabaseContext =
        new(dbContextOptionsBuilder.Options, currentUserProvider);
    }
    return fairplaysocialDatabaseContext;
});
builder.Services.AddHealthChecks()
    .AddDbContextCheck<FairPlaySocialDatabaseContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
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
            (path.StartsWithSegments(FairPlaySocial.Common.Global.Constants.Hubs.HomeFeedHub)))
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
    };
});
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});


var app = builder.Build();
app.UseResponseCompression();
ExceptionsHelper.HandleExceptions(app);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<NotificationHub>(FairPlaySocial.Common.Global.Constants.Hubs.HomeFeedHub);
app.MapHealthChecks("/health");
app.MapFallbackToFile("index.html");
app.Run();

static void AddPlatformServices(WebApplicationBuilder builder)
{
    builder.Services.AddTransient<ApplicationUserService>();
    builder.Services.AddTransient<PhotoService>();
    builder.Services.AddTransient<UserPreferenceService>();
}
