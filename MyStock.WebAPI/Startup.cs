using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundTasksSample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using Microsoft.IdentityModel.Tokens;
using MyStock.Data;
using MyStock.WebAPI.Notifications;
using MyStock.WebAPI.Services;
using MyStock.WebAPI.Services.Abstraction;
using MyStock.WebAPI.ViewModels.Fillers;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;
using System.Net;

namespace MyStock.WebAPI
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddCors();


            string conString = Configuration.GetConnectionString("StockContext");
            StockContext.MyConnectionString = conString;

            services
          .AddDbContext<StockContext>(options =>
              options.UseMySql(
                 conString,
                  o => o.MigrationsAssembly("MyStock.WebAPI")
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
                //�������־�ŻῪ��ϵͳ�ĺ�̨����
                //������mysql ��������פ���������ר����ȡ���ݣ���С�����������������ѹ����
                //Ȼ���������������Ϲر�������񣬼�С��Դռ�á�
                //��Ҫ������wrapper ������timed service�еĶ�ʱ��
                services.AddHostedService<WrappeHostedService>();
            }
            #endregion



            #region snippet3
            //�ṩ��̨���з���
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            #endregion

            int threadNum = Configuration.GetValue<int>("MaxThreadNum");
            BaseDoWorkViewModel.DefaultMaxThreadNum = threadNum;


            services.AddSingleton<BackServiceUtil>();
            services.AddScoped<PullAllStockNamesViewModel>();
            services.AddScoped<PullStockF10ViewModel>();
            services.AddScoped<PullStockIndex1dViewModel>();
            services.AddScoped<PullRealTimeViewModel>();
            services.AddScoped<PullMarginDataViewModel>();
            services.AddScoped<PullMarketDealDataViewModel>();
            services.AddScoped<CalcLimitNumViewModel>();
            services.AddScoped<CalcRealTimeLimitNumViewModel>();
            services.AddScoped<PullIndex30mViewModel>();
            services.AddScoped<PullHuShenTongInTradeTimeViewModel>();


            //http client

            services.AddHttpClient("163.com", client =>
             {
                 client.BaseAddress = new Uri("http://quotes.money.163.com");

                 client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html, application/xhtml+xml, */*");
                 client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.5");
                 client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                 client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");


                 client.DefaultRequestHeaders.TryAddWithoutValidation("KeepAlive", "true");
                 client.DefaultRequestHeaders.ExpectContinue = true;

             }).ConfigurePrimaryHttpMessageHandler(
                x => new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }
                )
             .AddPolicyHandler(GetRetryPolicy());

        }

        static Polly.Retry.AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
        }

        private void printSystemInfo()
        {
            var temp2 = Assembly.GetEntryAssembly();
            string ret = temp2.GetName().Version.ToString();
            System.Console.WriteLine($"Service version is {ret}");
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            IConfigurationSection myArraySection = Configuration.GetSection("corsOrigins");
            var corsOrigins = (from i in myArraySection.AsEnumerable()
                               where i.Value != null
                               select i.Value).ToArray();

            foreach (var item in corsOrigins)
                System.Console.WriteLine("allow cors have " + item);
            printSystemInfo();

            app.UseCors(builder => builder
                .WithOrigins(corsOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
            // app.UseHttpsRedirection();

            Utils.DbInitializer.MigrateLatest(app);
            Utils.DbInitializer.Seed(app);
            //��Ҫ���еĳ��򶼶���flag���в���
            //Utils.DbInitializer.CheckIfClearFlags(app);

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationsHub>("/notifications");
                endpoints.MapControllers();
            });
        }
    }
}
