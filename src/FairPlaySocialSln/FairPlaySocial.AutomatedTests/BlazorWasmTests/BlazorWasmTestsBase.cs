using FairPlaySocial.AutomatedTests.BlazorWasmTests.BlazorDevServer;
using FairPlaySocial.AutomatedTests.ClientServices;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FairPlaySocial.AutomatedTests.BlazorWasmTests
{
    public abstract class BlazorWasmTestsBase : ClientServicesTestsBase
    {
        public BlazorWasmTestsBase() : base()
        {
            var configJson = JsonSerializer.Serialize(ClientAppConfiguration);
            var configBytes = Encoding.UTF8.GetBytes(configJson);
            var configStream = new MemoryStream(configBytes);
            var config = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .Build();

            //var root0 = @"C:\Projects\pticostaricags\FairPlaySocial\src\FairPlaySocialSln\FairPlaySocial\Client\bin\Debug\net7.0\";
            var root = Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\","FairPlaySocial\\Client\\bin\\Debug\\net7.0\\");
            var location = Path.GetFullPath(Path.Combine(root, config[nameof(ClientAppConfiguration.ContentRoot)]!));

            
            this.ContentRoot = location;
            this.RootUri = new Uri(StartAndGetRootUri());
        }

        public Uri RootUri { get; set; }
        public IHost? Host { get; set; }
        public string PathBase { get; set; } = String.Empty;
        public string ContentRoot { get; set; } = String.Empty;


        protected string StartAndGetRootUri()
        {
            Host = CreateWebHost();
            RunInBackgroundThread(Host.Start);
            var url = Host.Services.GetRequiredService<IServer>().Features
                .Get<IServerAddressesFeature>()!
                .Addresses.Single();
            return url;
        }

        protected IHost CreateWebHost()
        {
            var host = "127.0.0.1";

            var args = new List<string>
            {
                "--urls", $"http://{host}:0",
                "--contentroot", ContentRoot,
                "--pathbase", PathBase,
            };

            return DevServer.BuildWebHost(args.ToArray());
        }

        protected static void RunInBackgroundThread(Action action)
        {
            var isDone = new ManualResetEvent(false);

            ExceptionDispatchInfo? edi = null;
            new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    edi = ExceptionDispatchInfo.Capture(ex);
                }

                isDone.Set();
            }).Start();

            if (!isDone.WaitOne(TimeSpan.FromSeconds(10)))
            {
                throw new TimeoutException("Timed out waiting for: " + action);
            }

            if (edi != null)
            {
                throw edi.SourceException;
            }
        }
    }
}
