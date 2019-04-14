using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Blog.Core.AuthHelper.OverWrite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Blog.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        string assemblyPath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<ICaching, MemoryCaching>();  //把缓存注入（注入之后才能使用构造方式注入）

            services.AddScoped<Common.IRedisCacheManager, Common.RedisCacheManager>();
            //services.AddSingleton;

            #region AutoMapper
            services.AddAutoMapper(typeof(Startup));
            #endregion


            #region Cors跨域
            //跨域使用第二种方式，以后常用这种方式
            services.AddCors(c =>
            {
                c.AddPolicy("LimitRequests", policy =>
                {
                    policy.WithOrigins("http://localhost:8020", "http://www.baid.com") //支持多个端口    在定义策略 LimitRequests 的时候，源域名应该是客户端请求的端口域名，不是当前API的域名端口
                    .AllowAnyHeader()
                    .AllowAnyMethod(); //标头添加到策略
                });
            });

            //跨域使用第一种方式，以后不用
            // services.AddCors();
            #endregion

            #region Swgger
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Version = "v0.1.0",
                    Title = "Blog.Core API",
                    Description = "框架说明文档",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = "Blog.Core", Email = "Blog.Core@xxx.com", Url = "https://www.jianshu.com/u/94102b59cc2a" }
                });

                //启动注释
                //.Net Core 2.1获取路径
                string basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
                var xmlPath = System.IO.Path.Combine(basePath, "Blog.Core.xml");
                x.IncludeXmlComments(xmlPath, false);

                //添加model文档说明
                var xmlModelPath = System.IO.Path.Combine(basePath, "Blog.Core.Model.xml");
                x.IncludeXmlComments(xmlPath);




                #region Token绑定到ConfigureServices
                //添加head验证信息
                var security = new Dictionary<string, IEnumerable<string>> { { "Blog.Core", new string[] { } }, };
                x.AddSecurityRequirement(security);
                //方案名称“Blog.Core”可自定义，上下一致即可
                x.AddSecurityDefinition("Blog.Core", new ApiKeyScheme {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
                #endregion

            });
            #endregion

            #region Token服务注册
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            //增加授权权限
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("AdminOrClient", policy => policy.RequireRole("Admin", "Client").Build());

            });
            #endregion

            #region AutoFac            

            //实例化AutoFac 容器
            var builder = new Autofac.ContainerBuilder();
            builder.RegisterType<BlogLogAOP>();
            builder.RegisterType<BlogCacheAOP>();
            #region 带有接口层的服务注入

            //注册要通过反射来创建组件=>下面通过注册程序集来代替单个类注册
            //builder.RegisterType<Blog.Core.Services.AdvertisementServices>().As<Blog.Core.IServices.IAdvertisementServices>();
            //加载路径

            #region Services.dll注入
            var serviceDllFile = System.IO.Path.Combine(assemblyPath, "Blog.Core.Services.dll");
            var assemblyServices = System.Reflection.Assembly.LoadFile(serviceDllFile);//   System.Reflection.Assembly.Load("Blog.Core.Services");
            builder.RegisterAssemblyTypes(assemblyServices).AsImplementedInterfaces(); //指定已扫描程序集中的类型注册为提供所有其实现接口

            // AOP 开关，如果想要打开指定的功能，只需要在 appsettigns.json 对应对应 true 就行
            var cacheType =new List<Type>();
            if(Blog.Core.Common.Appsettings.app(new string[] { "AppSettings", "MemoryCachingAOP", "Enable" }).ObjToBool())
            {
                cacheType.Add(typeof(BlogCacheAOP));
            }
            if(Blog.Core.Common.Appsettings.app(new string[] { "AppSettings", "BlogLogAOP", "Enable" }).ObjToBool())
            {
                cacheType.Add(typeof(BlogLogAOP));

            }

            #region 日志注入相关设置
            builder.RegisterAssemblyTypes(assemblyServices)
                         .AsImplementedInterfaces()
                         .InstancePerLifetimeScope()
                         .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                         .InterceptedBy(cacheType.ToArray()); 
            #endregion



            #endregion

            #region Repositroy.dll注入
            var repositoryDllFile = System.IO.Path.Combine(assemblyPath, "Blog.Core.Repository.dll");
            var assemblyRepository = System.Reflection.Assembly.LoadFile(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblyRepository).AsImplementedInterfaces();
            #endregion 
            #endregion

            //将service填充到容器中
            builder.Populate(services);

            //使用已进行的组件登记创建新的容器
            var ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                if(env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }   
               
            }

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
               // x.RoutePrefix = "";

            });
            #endregion



            #region Authen

            //进行注册中间件
            app.UseMiddleware<JwtTokenAuth>();

            #endregion

            #region Cors

            #region 跨域第二种方式
            app.UseCors("LimitRequests");
            #endregion

            #region 跨域第一种方式
            //app.UseCors(c => c.WithOrigins("http://localhost:8020").AllowAnyHeader().AllowAnyMethod()); 

            #endregion

            #endregion

            app.UseMvc();
        }
    }
}
