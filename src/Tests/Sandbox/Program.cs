﻿namespace Sandbox
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using CommandLine;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using PressCenters.Data;
    using PressCenters.Data.Common;
    using PressCenters.Data.Common.Repositories;
    using PressCenters.Data.Models;
    using PressCenters.Data.Repositories;
    using PressCenters.Data.Seeding;
    using PressCenters.Services.Data;
    using PressCenters.Services.Messaging;

    using Sandbox.Code;

    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"{typeof(Program).Namespace} ({string.Join(" ", args)}) starts working...");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider(true);

            // Seed data on application startup
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                ApplicationDbContextSeeder.Seed(dbContext, serviceScope.ServiceProvider);
            }

            using (var serviceScope = serviceProvider.CreateScope())
            {
                serviceProvider = serviceScope.ServiceProvider;

                return await Parser.Default.ParseArguments<SandboxOptions>(args).MapResult<SandboxOptions, Task<int>>(
                    async (SandboxOptions opts) => await SandboxCode(opts, serviceProvider),
                    async _ => await Task.FromResult(255));
            }
        }

        private static async Task<int> SandboxCode(SandboxOptions options, IServiceProvider serviceProvider)
        {
            var sw = Stopwatch.StartNew();

            //// serviceProvider.GetService<INewsService>().SaveImageLocallyAsync("https://prb.bg/upload/55508/%D0%93%D0%B5%D1%80%D0%B1+%D0%92%D0%B8%D1%82%D1%80%D0%B0%D0%B6.JPG", 191333, @"C:\Temp\wwwroot", false).GetAwaiter().GetResult();
            //// new UpdateSearchTextSandbox().Work(serviceProvider).GetAwaiter().GetResult();
            await new GetAllNewsSandbox().Work(serviceProvider);
            //// new DownloadImagesSandbox().Work(serviceProvider).GetAwaiter().GetResult();

            Console.WriteLine(sw.Elapsed);
            return 0;
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            var loggerFactory = new LoggerFactory();
            services.AddDbContext<ApplicationDbContext>(
                options => options
                    .UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        sqlServerOptions => sqlServerOptions.CommandTimeout(600)).EnableSensitiveDataLogging()
                    .UseLoggerFactory(loggerFactory));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSingleton<ILoggerFactory>(
                provider =>
                {
                    var factory = new LoggerFactory();
                    factory.AddProvider(new OneLineConsoleLoggerProvider(true));
                    return factory;
                });
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender, NullMessageSender>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<INewsService, NewsService>();
            services.AddTransient<ITagsService, TagsService>();
        }
    }
}
