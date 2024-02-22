using Icomm.Hosting.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Icomm.Infrastructure.Installers;

internal class RegisterConfigServer : IServiceRegistration
{
    public void RegisterAppServices(IServiceCollection services, IConfiguration configuration) =>
        services.AddConfigManager(configuration: configuration);
}