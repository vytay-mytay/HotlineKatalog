using HotlineKatalog.DAL;
using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.DAL.Repository;
using HotlineKatalog.DAL.UnitOfWork;
using HotlineKatalog.Services.AutoMapperConfig;
using HotlineKatalog.WebSockets.Extentions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace HotlineKatalog
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
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Connection"));
                options.EnableSensitiveDataLogging(false);
            });

            #region Register services

            #region Basis services

            services.AddScoped<IDataContext>(provider => provider.GetService<DataContext>());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSingleton<HashUtility>();
            //services.AddSingleton<TemporaryDataManager>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<IJWTService, JWTService>();

            #endregion

            #region Business logic

            //services.AddTransient<IAccountService, AccountService>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IChatService, ChatService>();
            //services.AddTransient<IImageService, ImageService>();
            //services.AddTransient<IPostService, PostService>();
            //services.AddTransient<IFeedService, FeedService>();
            //services.AddTransient<IDiaryService, DiaryService>();
            //services.AddTransient<IContentService, ContentService>();
            //services.AddTransient<ISettingsService, SettingsService>();
            //services.AddTransient<IHashtagService, HashtagService>();
            //services.AddTransient<IFollowingService, FollowingService>();
            //services.AddTransient<ICommentService, CommentService>();
            //services.AddTransient<ILikeService, LikeService>();
            //services.AddTransient<ILibraryService, LibraryService>();
            //services.AddTransient<IAdsService, AdsService>();
            //services.AddTransient<IReportService, ReportService>();
            //services.AddTransient<IStatisticsService, StatisticsService>();

            #endregion

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfileConfiguration());
            });

            services.AddSingleton(config.CreateMapper());

            #endregion

            services.AddMvc(options =>
            {
                // Allow use optional parameters in actions
                options.AllowEmptyInputInBodyModelBinding = true;
                options.EnableEndpointRouting = false;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true)
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddWebSocketManager();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "HotlineKatalog",
                    Description = "A simple site parse ASP.NET Core Web API",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
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

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotlineKatalog");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
