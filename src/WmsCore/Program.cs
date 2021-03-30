using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using NLog.Web;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using YL.Utils.Configs;

namespace YL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!Debugger.IsAttached)
            {
                string dir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                Environment.CurrentDirectory = dir;
            }
            if (args.Contains("-s"))
            {
                CreateWebHostBuilder(args).Build().RunAsService();
            }
            else
            {
                CreateWebHostBuilder(args).Build().Run();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = ConfigUtil.GetConfiguration;
            if (string.IsNullOrWhiteSpace(config["urls"]))
            {
                return WebHost.CreateDefaultBuilder(args)
                 .UseNLog()
                 .UseStartup<Startup>();
            }
            else
            {
                return WebHost.CreateDefaultBuilder(args)
                    .UseConfiguration(ConfigUtil.GetConfiguration) 
                    .UseNLog()
                    .UseStartup<Startup>();
            }
        }
    }
}