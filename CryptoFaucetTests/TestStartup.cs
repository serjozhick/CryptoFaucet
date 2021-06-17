using CryptoFacet;
using CryptoFaucet.Database;
using CryptoFaucet.Logging;
using CryptoFaucet.Services;
using CryptoFaucetTests.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Reflection;

namespace CryptoFaucetTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void InitializeServices(IServiceCollection services)
        {
            base.InitializeServices(services);

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<FaucetDbContext>));
            services.Remove(descriptor);

            descriptor = services.SingleOrDefault(
               d => d.ServiceType == typeof(IFaucetDbContext));
            services.Remove(descriptor);
            services.AddSingleton<IFaucetDbContext, DbMock>();

            var hostedServices = services.Where(
               d => d.ServiceType == typeof(IHostedService));
            hostedServices?.ToList().ForEach(hostedService => services.Remove(hostedService));

            descriptor = services.SingleOrDefault(
               d => d.ServiceType == typeof(ILog));
            services.Remove(descriptor);
            services.AddSingleton<ILog, LogMock>();

            descriptor = services.SingleOrDefault(
               d => d.ServiceType == typeof(IExchangeRateService));
            services.Remove(descriptor);
            services.AddSingleton<IExchangeRateService, RateServiceMock>();

            services.AddControllers().AddApplicationPart(typeof(Startup).GetTypeInfo().Assembly).AddControllersAsServices();
        }
    }
}
