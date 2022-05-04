using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MongoDB.Driver;

using Serilog;

namespace BCHGrpcService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(@"appsettings.json")
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
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<Services.BCHGrpcService>();

                endpoints.MapGet("/", async context => await context
                    .Response
                    .WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909")
                    .ConfigureAwait(false));
            });
        }
    }
}