using Microsoft.AspNetCore.Mvc.Filters;
using SSHConnectCore.Utilities;

namespace SSHConnectCore.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException (ExceptionContext context)
        {
            Logger.Log(this.GetType().Name, context.Exception.Message);
        }
    }
}
