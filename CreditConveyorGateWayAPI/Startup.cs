using AspNetCoreRateLimit;

using BCHGrpcService;

using CashboxGrpcService;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using MongoDB.Driver;

using Serilog;

using System;
using System.Collections.Generic;
using System.Text;

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
            services.AddOptions();
            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            services.AddInMemoryRateLimiting();

            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "CreditConveyorGateWayAPI", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Your JWT for Authorization header",
                    Type = SecuritySchemeType.Http
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                                        {
                                            Id = "Bearer",
                                            Type = ReferenceType.SecurityScheme
                                        }
                        },
                        new List<string>()
                    }
                });
            });

            services.AddSingleton(typeof(IConfiguration), Configuration);

            var serilog = new LoggerConfiguration().
                MinimumLevel.Information().
                WriteTo.Console().
                WriteTo.MongoDB(new MongoClient().GetDatabase(Configuration.GetSection("MongoDB:DBName").Value),
                                collectionName: Configuration.GetSection("MongoDB:LogsCollection").Value).
                CreateLogger();

            services.AddSingleton(typeof(ILogger), serilog);
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddGrpcClient<BCHGrpc.BCHGrpcClient>(options =>
                options.Address = new Uri(Configuration.GetSection("BCHGrpcService:AddressAndPort").Value)
            );

            services.AddGrpcClient<Cashbox.CashboxClient>(options =>
                options.Address = new Uri(Configuration.GetSection("CashboxGrpcService:AddressAndPort").Value)
            );

            services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("JWT:key").Value)),

                    ValidateIssuer = true,
                    ValidIssuer = Configuration.GetSection("JWT:validIssuer").Value,

                    ValidateAudience = true,
                    ValidAudience = Configuration.GetSection("JWT:validAudience").Value,

                    ValidateLifetime = true
                }
                );
            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIpRateLimiting();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CreditConveyorGateWayAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}