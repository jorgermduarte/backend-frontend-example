namespace dotnetbackened.adapters.attributes
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.Text;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : Attribute, IAuthorizationFilter
    {
        private const string AUTHORIZATION_HEADER = "Authorization";
        private const string BASIC_SCHEME = "Basic";
        private readonly EUserRoles[] _roles;

        public BasicAuthAttribute(params EUserRoles[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers[AUTHORIZATION_HEADER].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith(BASIC_SCHEME, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authHeader.Substring(BASIC_SCHEME.Length).Trim();
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');

            if (credentials.Length != 2 || !IsAuthorized(credentials[0], credentials[1], out EUserRoles[] userRoles))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (_roles.Length > 0 && !_roles.Any(role => userRoles.Contains(role)))
            {
                context.Result = new ForbidResult();
            }
        }

        private bool IsAuthorized(string username, string password, out EUserRoles[] roles)
        {
            roles = GetRolesForUser(username, password);
            return roles != null;
        }


        private EUserRoles[] GetRolesForUser(string username, string password)
        {
            // example to get roles from database
            if (username == "admin" && password == "password")
            {
                return new[] { EUserRoles.Admin, EUserRoles.User };
            }
            else if (username == "user" && password == "password")
            {
                return new[] { EUserRoles.User };
            }
            return null;
        }
        public enum EUserRoles
        {
            Admin,
            User,
            Manager,
            Guest
        }
    }

}
