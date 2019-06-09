using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoapCore;
using System.ServiceModel;

namespace InterfaceMocker.Service
{
    public class ServiceHost
    { 
        //private HttpSelfHostServer _httpServer = null;
        //public void Start(int port)
        //{ 
        //    try
        //    {
        //        //Assembly.Load("WebApi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
        //        string url = string.Format("http://localhost:{0}/", port);
        //        HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(url);
        //        //configuration.TransferMode = TransferMode.Streamed;

        //        // 配置 http 服务的路由
        //        var cors = new EnableCorsAttribute("*", "*", "*");//跨域允许设置
        //        config.EnableCors(cors);

        //        config.Formatters
        //            .XmlFormatter.SupportedMediaTypes.Clear();
        //        //默认返回 json
        //        config.Formatters
        //            .JsonFormatter.MediaTypeMappings.Add(
        //            new QueryStringMapping("datatype", "json", "application/json"));

        //        _httpServer = new HttpSelfHostServer(config);
        //        _httpServer.Configuration.Routes.MapHttpRoute(
        //           name: "DefaultApi",
        //           routeTemplate: "api/{controller}/{id}",
        //           defaults: new { id = RouteParameter.Optional });
        //        _httpServer.OpenAsync().Wait();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
             
        //}

        public void Start(int port)
        {

            string url = string.Format("http://localhost:{0}/",port);
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls(url).Build().RunAsync();

        }

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
                services.AddMvc(option =>
                {  
                    
                }).SetCompatibilityVersion(CompatibilityVersion.Latest);
                //services.Configure<CookiePolicyOptions>(options =>
                //{
                //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //    options.CheckConsentNeeded = context => true;
                //    options.MinimumSameSitePolicy = SameSiteMode.None;
                //});c
                
                services.AddOptions(); 
                services.AddSoapCore(); 
                services.AddHttpContextAccessor();  
                services.AddResponseCompression();//添加压缩 
        
            }
          
            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage(); 
                }
              
                app.UseResponseCompression();  //使用压缩 
             

                app.UseSoapEndpoint<MESController>("/mes.asmx", new BasicHttpBinding()); 
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
            }
        }
    }
}
