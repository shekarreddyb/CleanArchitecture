using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence.Interceptors
{
    public class EfQueryInterceptor : DbCommandInterceptor
    {
        //private ILogger<EfQueryInterceptor> _logger;
        //private readonly Stopwatch _timer;
        public EfQueryInterceptor() : base()
        {
            //_logger = logger;
           // _timer = new Stopwatch();
        }



        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result, CancellationToken cancellationToken)
        {
            //_timer?.Restart();
            Log.Logger.Information("Running EF Query");
            return Task.FromResult(result);
        }

        public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default) {
            Exception ex = new Exception("EF Query Failed", eventData.Exception);
            Log.Logger.Fatal("EF Error, All gone...., time to leave planet. All Gone {0}", eventData);
            return Task.CompletedTask;
        }
    }
}
