using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundTasksSample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Stock.Data;
using Stock.WebAPI.Services;
using Stock.WebAPI.Services.Abstraction;
using Stock.WebAPI.ViewModels.Fillers;

namespace Stock.WebAPI
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
            services.AddCors();

            services
          .AddDbContext<StockContext>(options =>
              options.UseMySql(
                  Configuration.GetConnectionString("StockContext"),
                  o => o.MigrationsAssembly("Stock.WebAPI")
              )
          );


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JWTSecretKey"))
                        )
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/notifications")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MyMappingProfile()));
            services.AddSingleton(mappingConfig.CreateMapper());

            services.AddSingleton<IAuthService>(
                new AuthService(
                    Configuration.GetValue<string>("JWTSecretKey"),
                    Configuration.GetValue<int>("JWTLifespan")
                )
            );

            services.AddControllers();
            services.AddSignalR().AddJsonProtocol();


            services.AddSingleton<TimedService>();
            #region snippet1

            if (Configuration.GetValue<bool>("EnableTimedService"))
            {
                //有这个标志才会开启系统的后台服务
                //可以在mysql 服务器上驻存这个服务，专门来取数据，减小主服务器的网络带宽压力。
                //然后在其他服务器上关闭这个服务，减小资源占用。
                //需要副作用wrapper 来开启timed service中的定时器
                services.AddHostedService<WrappeHostedService>();
            }
            #endregion



            #region snippet3
            //提供后台队列服务
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            #endregion

            int threadNum = Configuration.GetValue<int>("MaxThreadNum");
            BaseDoWorkViewModel.MaxThreadNum = threadNum;


            services.AddSingleton<BackServiceUtil>();
            services.AddScoped<PullAllStockNamesViewModel>();
            services.AddScoped<F10FHPGFillerViewModel>();
            services.AddScoped<DayDataFillerViewModel>();
            services.AddScoped<RealTimeDataFillerViewModel>();


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
