﻿namespace GSoulavy.Template.WindowsService
{
    using System;
    using System.Diagnostics;

    using Configurations;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Services;

    internal class Program
    {
        private static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration(
                    (ctx, builder) =>
                    {
                        if (!ctx.HostingEnvironment.IsDevelopment())
                            builder.SetBasePath(Environment.GetEnvironmentVariable("ServiceBasePath"));

                        builder
                            .AddJsonFile("appsettings.json", false, true)
                            .AddJsonFile(
                                $"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json",
                                true,
                                true
                            )
                            .AddEnvironmentVariables();
                    }
                )
                .ConfigureLogging(
                    (hostContext, logging) =>
                    {
                        logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                    }
                )
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        services.Configure<HostedSettings>(
                            hostContext.Configuration.GetSection(nameof(HostedSettings))
                        );

                        services.AddHostedService<HostedService>();
                    }
                );
    }
}
