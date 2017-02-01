using Microsoft.AspNetCore.Mvc.Filters;
using SSHConnectCore.Controllers;
using SSHConnectCore.Utilities;

namespace SSHConnectAPI.Filters
{
    public class ConnectionActionFilter : ActionFilterAttribute

    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as SSHConnectController;
            if (controller == null)
            {
                Logger.Log(this.GetType().Name, "Incorrect controller expected SSHConnectController");
                return;
            }

            controller.CheckConnection();
        }

    }
}