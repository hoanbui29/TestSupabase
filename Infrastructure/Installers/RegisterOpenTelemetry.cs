using Icomm.Hosting.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Icomm.Infrastructure.Installers;

internal class RegisterOpenTelemetry : IServiceRegistration
{
    public void RegisterAppServices(IServiceCollection services, IConfiguration configuration) =>
        services.AddIcOpenTelemetry(
            configuration,
            configure: options =>
            {
                options.TracingSources = options.MetricSources = new[]
                {
                    // Tracer.OpenTelemetry.Fody
                    Tracer.OpenTelemetry.ActivitySourceConstants.Name
                };
                options.Tracing += b =>
                {
                    //// https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/src/OpenTelemetry.Instrumentation.SqlClient
                    //configuration.ConfigureIfEnable(config => b.AddSqlClientInstrumentation(opt => config.Bind(opt)), "OpenTelemetry", "Instrumentation", "SqlClient");
                    //// https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src/OpenTelemetry.Instrumentation.GrpcCore
                    //configuration.ConfigureIfEnable(config => b.AddGrpcCoreInstrumentation(), "OpenTelemetry", "Instrumentation", "GrpcCore");

                    //// https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/src/OpenTelemetry.Instrumentation.GrpcNetClient
                    //configuration.ConfigureIfEnable(
                    //    config =>
                    //        b.AddGrpcClientInstrumentation(opt =>
                    //        {
                    //            config.Bind(opt);
                    //            opt.EnrichWithHttpRequestMessage = options!.Enrich;
                    //            opt.EnrichWithHttpResponseMessage = options!.Enrich;
                    //        }),
                    //    "OpenTelemetry",
                    //    "Instrumentation",
                    //    "GrpcClient"
                    //);

                    //// https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src/OpenTelemetry.Instrumentation.ElasticsearchClient
                    //configuration.ConfigureIfEnable(config => b.AddElasticsearchClientInstrumentation(opt => config.Bind(opt)), "OpenTelemetry", "Instrumentation", "ElasticsearchClient");
                    //// https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src/OpenTelemetry.Instrumentation.EntityFrameworkCore
                    //configuration.ConfigureIfEnable(config => b.AddEntityFrameworkCoreInstrumentation(opt => config.Bind(opt)), "OpenTelemetry", "Instrumentation", "EntityFrameworkCore");
                    //// https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src/OpenTelemetry.Instrumentation.Hangfire
                    //configuration.ConfigureIfEnable(config => b.AddHangfireInstrumentation(), "OpenTelemetry", "Instrumentation", "Hangfire");
                    //// https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src/OpenTelemetry.Instrumentation.Quartz
                    //configuration.ConfigureIfEnable(config => b.AddQuartzInstrumentation(opt => config.Bind(opt)), "OpenTelemetry", "Instrumentation", "Quartz");
                    //// https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src/OpenTelemetry.Instrumentation.StackExchangeRedis
                    //configuration.ConfigureIfEnable(config => b.AddRedisInstrumentation(configure: opt => config.Bind(opt)), "OpenTelemetry", "Instrumentation", "Redis");
                };
            }
        );
}