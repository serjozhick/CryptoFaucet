using CryptoFaucet.Database;
using CryptoFaucet.Documentation;
using CryptoFaucet.Jobs;
using CryptoFaucet.Logging;
using CryptoFaucet.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Reflection;

namespace CryptoFacet
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected virtual void InitializeServices(IServiceCollection services)
        {
            services.AddSingleton<ILog, NlogLogger>();
            services.AddScoped<IFaucetDbContext, FaucetDbContext>();
            services.AddSingleton<IExchangeRateService, ExchangeRateService>();
            services.AddScoped<IExchangeService, ExchangeService>();
            services.AddScoped<IFaucetAccountService, FaucetAccountService>();
            services.AddScoped<IClaimService, ClaimService>();
            services.AddHostedService<RefillJob>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.Converters.Add(new StringEnumConverter
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    });
                });;

            services.AddDbContext<FaucetDbContext>(options =>
                options.UseSqlite(Configuration["DataSource"]));

            InitializeServices(services);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Crypto Faucet API",
                        Version = "v1"
                    });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                options.SchemaFilter<EnumTypesSchemaFilter>(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Crypto Faucet API");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
