using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using MongoDB.Driver;

using Serilog;

namespace GatewayAPI
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
            services.AddControllers();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "CreditConveyorGateWayAPI", Version = "v1" }));

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(@"S:\C#\Web\MFO\CreditConveyorGateWayAPI\appsettings.json")
                .Build();
            services.AddSingleton(typeof(IConfiguration), config);

            var serilog = new LoggerConfiguration().
                MinimumLevel.Information().
                WriteTo.Console().
                WriteTo.MongoDB(new MongoClient().GetDatabase(config.GetSection("MongoDB:DBName").Value),
                                collectionName: config.GetSection("MongoDB:LogsCollection").Value).
                CreateLogger();
            services.AddSingleton(typeof(ILogger), serilog);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CreditConveyorGateWayAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
