using IRepository;
using IServices;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;
using Repository;
using Services;
using Services.Outside;
using SoapCore;
using SqlSugar;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Linq;
using System.ServiceModel;
using System.Text;
using WMSCore.Outside;
using YL.Core.Entity;
using YL.Core.Orm.SqlSugar;
using YL.NetCore.Attributes;
using YL.NetCore.Conventions;
using YL.NetCore.DI;
using YL.NetCore.Middlewares;
using YL.NetCoreApp.Extensions;
using YL.Utils.Json;

namespace YL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
         

        //IServiceProvider This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            WCSApiAccessor.Host = Configuration["Host:WCS"];
            MESApiAccessor.Host = Configuration["Host:MES"];
            WMSBaseApiAccessor.WMSProxy = Configuration["Host:WMSPROXY"];

            services.AddMvc(option =>
            {
                option.Filters.Add<BaseExceptionAttribute>();
                //option.Filters.Add<FilterXSSAttribute>();
                option.Conventions.Add(new ApplicationDescription("title", Configuration["sys:title"]));
                option.Conventions.Add(new ApplicationDescription("company", Configuration["sys:company"]));
                option.Conventions.Add(new ApplicationDescription("customer", Configuration["sys:customer"]));
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1" });
            });
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            services.AddTimedJob();
            services.AddOptions();
            services.AddXsrf();
            services.AddXss();
            services.AddAuthentication(c =>
            {
                c.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                c.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(cfg =>
            {
                cfg.LoginPath = "/Login/Index";
                cfg.LogoutPath = "/Login/Logout";
                //cfg.ExpireTimeSpan = TimeSpan.FromDays(1);
                //cfg.Cookie.Expiration = TimeSpan.FromDays(1);
                cfg.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                cfg.Cookie.Path = "/";
                cfg.Cookie.HttpOnly = true;
                //cfg.SlidingExpiration = true;
            });
            var sqlSugarConfig = SqlSugarConfig.GetConnectionString(Configuration);
            services.AddSqlSugarClient<SqlSugarClient>(config =>
            {
                config.ConnectionString = sqlSugarConfig.Item2;
                config.DbType = sqlSugarConfig.Item1;
                config.IsAutoCloseConnection = true;
                config.InitKeyType = InitKeyType.Attribute;
                //config.IsShardSameThread = false;
            });
            services.AddJson(o =>
            {
                o.JsonType = JsonType.Jil;
            });
            services.AddSoapCore();
            services.AddDIProperty();
            services.AddHttpContextAccessor();
            services.AddHtmlEncoder();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddBr(); //br压缩
            services.AddResponseCompression();//添加压缩
            services.AddResponseCaching(); //响应式缓存
            services.AddMemoryCache();
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            //@1 DependencyInjection 注册
            services.AddNlog(); //添加Nlog
            RegisterBase(services);
            ServiceExtension.RegisterAssembly(services, "Services");
            ServiceExtension.RegisterAssembly(services, "Repository");
            var bulid = services.BuildServiceProvider();
            ServiceResolve.SetServiceResolve(bulid);



            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = sqlSugarConfig.Item1,
                ConnectionString = sqlSugarConfig.Item2,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
            });

            //OutputDataBase(db);
            InitDataBase(db);
            InitSystemData(db);
            db.Close();
        }

        private static void OutputDataBase(SqlSugarClient db)
        {
            System.IO.File.WriteAllText("d:\\Wms_reservoirarea.json", db.Queryable<Wms_reservoirarea>().ToList().ToJson());
            System.IO.File.WriteAllText("d:\\Wms_storagerack.json", db.Queryable<Wms_storagerack>().ToList().ToJson());
            System.IO.File.WriteAllText("d:\\Wms_inventorybox.json", db.Queryable<Wms_inventorybox>().ToList().ToJson());
        }

        private static bool InitDataBase(SqlSugarClient db)
        {
            Logger logger = LogManager.GetLogger("Startup");
            try
            {
                logger.Log(NLog.LogLevel.Info, "数据库整备开始");
                db.MappingTables = SqlSugarConfig.listTable;
                Type[] entityTypes = typeof(BaseEntity).Assembly.GetTypes().Where(x => x.Namespace == "YL.Core.Entity" && x.Name != typeof(BaseEntity).Name ).ToArray();
                foreach (Type type in entityTypes)
                {
                    try
                    {
                        db.CodeFirst.InitTables(type);
                    }
                    catch(Exception ex)
                    {
                        logger.Log(NLog.LogLevel.Error, ex, $"创建表{type}失败");

                    }
                }
                InitTableData<Sys_dept>(db, "Sys_dept.json");
                InitTableData<Sys_dict>(db, "Sys_dict.json");
                InitTableData<Sys_menu>(db, "Sys_menu.json");
                InitTableData<Sys_role>(db, "Sys_role.json");
                InitTableData<Sys_rolemenu>(db, "Sys_rolemenu.json");
                InitTableData<Sys_serialnum>(db, "Sys_serialnum.json");
                InitTableData<Sys_user>(db, "Sys_user.json");
                TestData.Create(db,logger); 

                logger.Log(NLog.LogLevel.Info, "数据库整备结束");
                return true;
            }
            catch(Exception ex)
            {
                logger.Log(NLog.LogLevel.Error, ex, "数据库整备失败");
                throw new InvalidOperationException("数据库整备失败"); 
            }
        }

        protected static void InitTableData<T>(SqlSugarClient sqlClient,string fileName) where T : class, new()
        {
            if (sqlClient.Queryable<T>().Any()) return;
            string fullPath = System.IO.Path.Combine(AppContext.BaseDirectory, "DataBase",fileName);
            string value = System.IO.File.ReadAllText(fullPath);
            T[] datas = JsonConvert.DeserializeObject<T[]>(value);
            sqlClient.Insertable(datas).ExecuteCommand();
        }

 

        /// <summary>
        /// 泛型注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="injection"></param>
        private static void RegisterBase(IServiceCollection services, ServiceLifetime injection = ServiceLifetime.Scoped)
        {
            switch (injection)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
                    services.AddScoped(typeof(IBaseServices<>), typeof(BaseServices<>));
                    services.AddScoped<MESHookController>();
                    services.AddScoped<WCSHookController>();
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));
                    services.AddSingleton(typeof(IBaseServices<>), typeof(BaseServices<>));
                    services.AddSingleton<MESHookController>();
                    services.AddSingleton<WCSHookController>();
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
                    services.AddTransient(typeof(IBaseServices<>), typeof(BaseServices<>));
                    services.AddTransient<MESHookController>();
                    services.AddTransient<WCSHookController>();
                    break;
            }
        }

        private void InitSystemData(SqlSugarClient db)
        {
            Wms_warehouse[] warehouses = db.Queryable<Wms_warehouse>().ToArray();
            foreach(Wms_warehouse warehouse in warehouses)
            {
                WMSApiManager.Regist(warehouse);
            }



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRequestResponseLogging();
            app.UseGlobalCore();
            app.UseExecuteTime();
            app.UseTimedJob();
            app.UseResponseCompression();  //使用压缩
            app.UseResponseCaching();    //使用缓存

            app.UseStaticFiles(); //使用静态文件
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseSoapEndpoint<MESHookController>("/Outside/MesHook.asmx", new BasicHttpBinding());
            app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Index}/{id?}");
                // Areas
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                  );
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}