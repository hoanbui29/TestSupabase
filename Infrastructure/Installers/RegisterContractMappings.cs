using Icomm.Hosting.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Icomm.Infrastructure.Installers;

internal class RegisterContractMappings : IServiceRegistration
{
    public void RegisterAppServices(IServiceCollection services, IConfiguration configuration)
    { }
}