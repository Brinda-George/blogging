using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace BlogWebsite.Filters
{
    public class Authenticationfilter : FilterAttribute, IAuthenticationFilter
    {
        void IAuthenticationFilter.OnAuthentication(AuthenticationContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            //( &&context.HttpContext.User.IsInRole(superAdminRole)
            // || context.HttpContext.User.IsInRole(adminRole))
            {
                // do nothing
            }
            else
            {
                context.Result = new HttpUnauthorizedResult(); // mark unauthorized
            }
        }

        void IAuthenticationFilter.OnAuthenticationChallenge(AuthenticationChallengeContext context)
        {
            if (context.Result == null || context.Result is HttpUnauthorizedResult)
            {
                context.Result = new RedirectToRouteResult("Default",
                    new System.Web.Routing.RouteValueDictionary{
                        {"controller", "Blog"},
                        {"action", "Login"},
                        {"returnUrl", context.HttpContext.Request.RawUrl}
                    });
            }
        }
    }
}
