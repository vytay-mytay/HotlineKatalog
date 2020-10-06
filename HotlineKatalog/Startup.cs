using HotlineKatalog.DAL;
using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.DAL.Repository;
using HotlineKatalog.DAL.UnitOfWork;
using HotlineKatalog.ScheduledTasks;
using HotlineKatalog.Services.AutoMapperConfig;
using HotlineKatalog.Services.Interfaces;
using HotlineKatalog.Services.Job;
using HotlineKatalog.Services.Services;
using HotlineKatalog.WebSockets.Extentions;
using HotlineKatalog.WebSockets.Handlers;
using HotlineKatalog.WebSockets.Middlewares;
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
            //services.AddSingleton<BrowserService>();
            //services.AddScoped<IJWTService, JWTService>();

            #endregion

            #region Business logic

            services.AddTransient<IComfyParseService, ComfyParseService>();
            services.AddTransient<IAddDBService, AddDBService>();
            services.AddTransient<AbstractParse, EldoradoParseService>();
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

            services.AddRouting();

            #region Scheduled tasks
            //Scheduled tasks
            services.AddSingleton<IScheduledTask, ParseJob>();
            services.AddScheduler((sender, args) =>
            {
                Console.Write(args.Exception.Message);
                args.SetObserved();
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseDefaultFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(5),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.Map("/webSocket", (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(serviceProvider.GetService<WebSocketMessageHandler>()));

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotlineKatalog");
            });

            app.UseStaticFiles();


            #region Error handler

            // Different middleware for api and ui requests  
            //app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
            //{
            //    var localizer = serviceProvider.GetService<IStringLocalizer<ErrorsResource>>();
            //    var logger = loggerFactory.CreateLogger("GlobalErrorHandling");

            //    // Exception handler - show exception data in api response
            //    appBuilder.UseExceptionHandler(new ExceptionHandlerOptions
            //    {
            //        ExceptionHandler = async context =>
            //        {
            //            var errorModel = new ErrorResponseModel(localizer);
            //            var result = new ContentResult();

            //            var exception = context.Features.Get<IExceptionHandlerPathFeature>();

            //            if (exception.Error is NotConfirmEmailException)
            //            {
            //                var ex = (NotConfirmEmailException)exception.Error;

            //                result = errorModel.Error(ex);
            //            }
            //            else if (exception.Error is CustomException)
            //            {
            //                var ex = (CustomException)exception.Error;

            //                result = errorModel.Error(ex);
            //            }
            //            else
            //            {
            //                var message = exception.Error.InnerException?.Message ?? exception.Error.Message;
            //                logger.LogError($"{exception.Path} - {message}");

            //                errorModel.AddError("general", message);
            //                result = errorModel.InternalServerError(env.IsDevelopment() ? exception.Error.StackTrace : null);
            //            }

            //            context.Response.StatusCode = result.StatusCode.Value;
            //            context.Response.ContentType = result.ContentType;

            //            await context.Response.WriteAsync(result.Content);
            //        }
            //    });

            //    // Handles responses with status codes (correctly executed requests, without any exceptions)
            //    appBuilder.UseStatusCodePages(async context =>
            //    {
            //        string message = "";

            //        List<ErrorKeyValue> errors = new List<ErrorKeyValue>();

            //        switch (context.HttpContext.Response.StatusCode)
            //        {
            //            case 400:
            //                message = "Bad Request";
            //                break;
            //            case 401:
            //                message = "Unauthorized";
            //                errors.Add(new ErrorKeyValue("token", "Token invalid"));
            //                break;
            //            case 403:
            //                message = "Forbidden";
            //                break;
            //            case 404:
            //                message = "Not found";
            //                break;
            //            case 500:
            //                message = "Internal Server Error";
            //                break;
            //        }

            //        context.HttpContext.Response.ContentType = "application/json";
            //        await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorResponseModel(localizer)
            //        {
            //            Code = message,
            //            StackTrace = "",
            //            Errors = errors
            //        }, new JsonSerializerSettings { Formatting = Formatting.Indented }));
            //    });
            //});

            //app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
            //{
            //    appBuilder.UseExceptionHandler("/Error");
            //    appBuilder.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
            //});

            #endregion


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
