using Microsoft.AspNetCore.Authorization;

namespace SSHConnectCore.Filters
{
    public class SSHConnectAuthorizeFilter : AuthorizeAttribute
    {
        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    // most of the authorization code from here http://stackoverflow.com/questions/25855698/how-can-i-retrieve-basic-authentication-credentials-from-the-header
        //    HttpContext httpContext = HttpContext.Current;
        //    string authHeader = httpContext.Request.Headers["Authorization"];

        //    if (authHeader != null && authHeader.StartsWith("Basic"))
        //    {
        //        string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();

        //        Encoding encoding = Encoding.GetEncoding("iso-8859-1");
        //        string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

        //        int seperatorIndex = usernamePassword.IndexOf(':');

        //        var username = usernamePassword.Substring(0, seperatorIndex);
        //        var password = usernamePassword.Substring(seperatorIndex + 1);

        //        var appSettingsUsername = ConfigurationManager.AppSettings["username"];
        //        var appSettingsPassword = ConfigurationManager.AppSettings["password"];

        //        // if the app settings are empty get details from local file
        //        if (string.IsNullOrEmpty(appSettingsUsername) || string.IsNullOrEmpty(appSettingsPassword))
        //        {
        //            var contents = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "authorized.txt");
        //            appSettingsUsername = contents[0];
        //            appSettingsPassword = contents[1];
        //        }

        //        if (username == appSettingsUsername && password == appSettingsPassword)
        //            return;
        //    }
        //    else
        //    {
        //        Logger.Log(this.GetType().Name, "The authorization header is either empty or isn't Basic.");
        //    }

        //    filterContext.Result = new HttpUnauthorizedResult();
        //}
    }
}