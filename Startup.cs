using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreApiExample.Infrastructure;
using AspNetCoreApiExample.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreApiExample
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
            JwtConfigModel.Init(Configuration);

            services.AddDbContext<MyDBContext>(options =>
            {
                options.UseInMemoryDatabase("TestDB");
            });

            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
            services.AddTransient<MyDBContext>();


            services.AddScoped<ITokenValidatorService, TokenValidatorService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.RequireHttpsMetadata = false;
           options.SaveToken = true;

           options.TokenValidationParameters = new TokenValidationParameters
           {
               ClockSkew = TimeSpan.Zero,
               ValidateIssuer = true,
               ValidateAudience = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = Configuration["Jwt:Issuer"],
               ValidAudience = Configuration["Jwt:Issuer"],
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
           };
           options.Events = new JwtBearerEvents
           {
               OnAuthenticationFailed = context =>
               {
                   var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                        .CreateLogger(nameof(JwtBearerEvents));
                   logger.LogError("Authentication failed.", context.Exception);
                   return Task.CompletedTask;
               },
               OnTokenValidated = context =>
               {
                   var tokenValidatorService = context.HttpContext.RequestServices
                        .GetRequiredService<ITokenValidatorService>();
                   return tokenValidatorService.ValidateAsync(context);
               },
               OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                         .CreateLogger(nameof(JwtBearerEvents));
                    logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);
                    return Task.CompletedTask;
                }
           };
       });


            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                        //.WithOrigins("http://localhost:3000")
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        //.AllowCredentials()
                        .Build();
                });
            });

            services.AddMemoryCache();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
