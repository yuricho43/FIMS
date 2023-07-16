using Serilog;
using Serilog.Configuration;

namespace Fims.Web.Server.Infrastructure.Extensions
{
    public static class SerilogCustomSinkExtensions
    {
        public static LoggerConfiguration SerilogCustomSink(this LoggerSinkConfiguration loggerConfiguration)
        {
            return loggerConfiguration.Sink(new SerilogCustomSink());
        }
    }
}