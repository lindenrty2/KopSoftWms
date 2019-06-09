using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.SelfHost;

namespace InterfaceMocker.Service
{
    public class ServiceHost
    { 
        private HttpSelfHostServer _httpServer = null;
        public void Start(int port)
        { 
            try
            {
                //Assembly.Load("WebApi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                string url = string.Format("http://localhost:{0}/", port);
                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(url);
                //configuration.TransferMode = TransferMode.Streamed;

                // 配置 http 服务的路由
                var cors = new EnableCorsAttribute("*", "*", "*");//跨域允许设置
                config.EnableCors(cors);

                config.Formatters
                    .XmlFormatter.SupportedMediaTypes.Clear();
                //默认返回 json
                config.Formatters
                    .JsonFormatter.MediaTypeMappings.Add(
                    new QueryStringMapping("datatype", "json", "application/json"));

                _httpServer = new HttpSelfHostServer(config);
                _httpServer.Configuration.Routes.MapHttpRoute(
                   name: "DefaultApi",
                   routeTemplate: "api/{controller}/{id}",
                   defaults: new { id = RouteParameter.Optional });
                _httpServer.OpenAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
             
        }
         
    }
}
