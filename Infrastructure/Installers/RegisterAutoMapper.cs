using Icomm.Hosting.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Icomm.Infrastructure.Installers;

internal class RegisterAutoMapper : IServiceRegistration
{
    public void RegisterAppServices(IServiceCollection services, IConfiguration configuration) =>
        services.AddAutoMapper(typeof(RegisterAutoMapper).Assembly);
}