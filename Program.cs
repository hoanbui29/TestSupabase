using Autofac;
using Autofac.Extensions.DependencyInjection;
using Context;
using EasyCaching.Interceptor.Castle;
using Icomm;
using Infrastructure.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TestSupabase;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args).UseConsoleLifetime().Build();
        var logger = builder.Services.GetService<ILogger<Program>>()!;
        try
        {
            logger.LogInformation("Starting host");
            await builder.RunAsync();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Host unexpectedly terminated");
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(
                (host, configBuilder) =>
                    configBuilder
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile(
                            $"appsettings.{host.HostingEnvironment.EnvironmentName}.json",
                            optional: true,
                            reloadOnChange: true
                        )
                        .AddJsonFile("appsettings.Logs.json")
                        .AddJsonFile(
                            $"appsettings.Logs.{host.HostingEnvironment.EnvironmentName}.json",
                            optional: true,
                            reloadOnChange: false
                        )
                        //.AddConfigManagerHttpProvider(environment: host.HostingEnvironment.EnvironmentName)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args)
            )
            .UseIcommLog()
            .ConfigureServices(
                (hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    //Register services in Installers folder
                    services.AddServicesInAssembly(configuration: configuration, typeof(Program));
                    services.AddHostedService<ServiceMain>();
                    services.AddScoped<ISupabaseClient, SupabaseClient>();
                    services.Configure<SupabaseSettings>(configuration.GetSection(nameof(SupabaseSettings)));
                }
            )
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder => builder.ConfigureCastleInterceptor());
}
