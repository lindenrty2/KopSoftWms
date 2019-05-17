﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SqlSugar;
using System.Text;
using YL.Core.Entity;
using YL.Utils.Configs;

namespace YL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = ConfigUtil.GetConfiguration;
            if (string.IsNullOrWhiteSpace(config["urls"]))
            {
                return WebHost.CreateDefaultBuilder(args)
                 .UseStartup<Startup>();
            }
            else
            {
                return WebHost.CreateDefaultBuilder(args)
                    .UseConfiguration(ConfigUtil.GetConfiguration)
                    .UseStartup<Startup>();
            }
        }
    }
}