using FairPlaySocial.AutomatedTests.BlazorWasmTests.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.AutomatedTests.BlazorWasmTests.BlazorDevServer
{
    // https://github.com/dotnet/aspnetcore/blob/09a036f2f397f40116d5ba2d5ce51ce92f88f03c/src/Components/WebAssembly/DevServer/src/Server/Startup.cs
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseDeveloperExceptionPage();
            EnableConfiguredPathbase(app, configuration);

            app.UseWebAssemblyDebugging();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                // In development, serve everything, as there's no other way to configure it.
                // In production, developers are responsible for configuring their own production server
                ServeUnknownFileTypes = true,
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("index.html");
            });
        }

        private static void EnableConfiguredPathbase(IApplicationBuilder app, IConfiguration configuration)
        {
            var pathBase = configuration.GetValue<string>(nameof(ClientAppConfiguration.PathBase));
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);

                // To ensure consistency with a production environment, only handle requests
                // that match the specified pathbase.
                app.Use((context, next) =>
                {
                    if (context.Request.PathBase == pathBase)
                    {
                        return next();
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        return context.Response.WriteAsync($"The server is configured only to " +
                            $"handle request URIs within the PathBase '{pathBase}'.");
                    }
                });
            }
        }
    }
}
