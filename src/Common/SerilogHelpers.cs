using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Enrichers.AspnetcoreHttpcontext;
using Serilog.Formatting.Compact;

namespace CleanArchitecture.Common
{
    public static class SerilogHelpers
    {
        /// <summary>
        /// Provides standardized, centralized Serilog wire-up for a suite of applications.
        /// </summary>
        /// <param name="loggerConfig">Provide this value from the UseSerilog method param</param>
        /// <param name="provider">Provide this value from the UseSerilog method param as well</param>
        /// <param name="applicationName">Represents the name of YOUR APPLICATION and will be used to segregate your app
        /// from others in the logging sink(s).</param>
        /// <param name="config">IConfiguration settings -- generally read this from appsettings.json</param>
        public static void WithConfiguration(this LoggerConfiguration loggerConfig,
            IServiceProvider provider, string applicationName, IConfiguration config)
        {
            var name = Assembly.GetExecutingAssembly().GetName();

            loggerConfig
                 .ReadFrom.Configuration(config)// minimum levels defined per project in json files 
                .Enrich.WithAspnetcoreHttpcontext(provider, AddCustomContextDetails)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Assembly", $"{name.Name}")
                .Enrich.WithProperty("Version", $"{name.Version}")
                .WriteTo.Seq("http://localhost:9001");
            ;
                //.WriteTo.File(new CompactJsonFormatter(),
                //    $@"C:\temp\Logs\{applicationName}.json");
            //.WriteTo.Logger(lc => lc
            //    .Filter.ByIncludingOnly(Matching.WithProperty("ElapsedMilliseconds"))
            //    .WriteTo.MSSqlServer(
            //        connectionString: @"Server=.\sqlexpress;Database=Logging;Trusted_Connection=True;",
            //        tableName: "PerfLog",
            //        autoCreateSqlTable: true,
            //        columnOptions: GetSqlColumnOptions()))
            //.WriteTo.Logger(lc => lc
            //    .Filter.ByIncludingOnly(Matching.WithProperty("UsageName"))
            //    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            //        {
            //            AutoRegisterTemplate = true,
            //            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
            //            IndexFormat = "usage-{0:yyyy.MM.dd}"
            //        }
            //    ))
            //.WriteTo.Logger(lc => lc
            //    .Filter.ByExcluding(Matching.WithProperty("ElapsedMilliseconds"))
            //    .Filter.ByExcluding(Matching.WithProperty("UsageName"))
            //    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            //        {
            //            AutoRegisterTemplate = true,
            //            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
            //            IndexFormat = "error-{0:yyyy.MM.dd}"
            //        }
            //    ));
        }


        private static UserInfo AddCustomContextDetails(IHttpContextAccessor ctx)
        {
            var context = ctx.HttpContext;
            var user = context?.User.Identity;
            if (user == null || !user.IsAuthenticated) return null;

            var i = 0;

            var userInfo = new UserInfo
            {
                Name = user.Name,
                Claims = context.User.Claims.ToDictionary(x => $"{x.Type} ({i++})", y => y.Value)
            };
            return userInfo;
        }
    }
}
