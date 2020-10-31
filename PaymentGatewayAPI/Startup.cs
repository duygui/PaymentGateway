using System.Reflection;
using AspNetCoreRateLimit;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentGatewayAPI.DataAccess;
using PaymentGatewayAPI.DataMigration;
using PaymentGatewayAPI.Filters;
using PaymentGatewayAPI.Repositories;

namespace PaymentGatewayAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddControllers();
            services.AddSingleton<IDataOperations, DataOperations>();
            services.AddSingleton<IPaymentFactory, CreditCardPaymentFactory>();
            services.AddSingleton<IPaymentRepository, PaymentRepository>();

            services.AddSwaggerGen(c => c.OperationFilter<HeaderParameterOperationFilter>());
            services.AddFluentMigratorCore()
                .ConfigureRunner(c => c
                .AddPostgres()
                .WithGlobalConnectionString(Configuration.GetConnectionString("Default"))
                .ScanIn(Assembly.GetExecutingAssembly()).For.All());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            app.UseClientRateLimiting();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway API V1");
            });

            Migrator.EnsureDatabase(Configuration.GetConnectionString("Default"));
            app.Migrate();

        }
    }
}
