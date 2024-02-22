using EasyCaching.Core.Interceptor;
using EasyCaching.Interceptor.Castle;
using Icomm.Caching.Interceptor;
using Icomm.Hosting.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Icomm.Infrastructure.Installers;

internal class RegisterEasyCaching : IServiceRegistration
{
    public void RegisterAppServices(IServiceCollection services, IConfiguration configuration)
    {
        //services.AddScoped<IAspectCoreService, AspectCoreService>();

        services.AddEasyCaching(cfg =>
        {
            const string MSGPACK_CUSTOM = "msgpack-custom";
            cfg.WithMessagePack();
            cfg.WithMessagePack(
                x =>
                {
                    x.EnableCustomResolver = true;
                },
                MSGPACK_CUSTOM
            );
            cfg.UseInMemory(
                opt =>
                {
                    opt.EnableLogging = true;
                    opt.SerializerName = MSGPACK_CUSTOM;
                },
                "m1"
            );
        });
        services.TryAddSingleton<IEasyCachingKeyGenerator, ModRepositoryEasyCachingKeyGenerator>();
        services.ConfigureCastleInterceptor(options => options.CacheProviderName = "m1");
    }
}