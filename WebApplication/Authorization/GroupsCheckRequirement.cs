using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Authorization
{
    public class GroupsCheckRequirement : IAuthorizationRequirement
    {
        public string groups;

        public GroupsCheckRequirement(string groups)
        {
            this.groups = groups;
        }
    }

    public class GroupsCheckHandler : AuthorizationHandler<GroupsCheckRequirement>
    {
        private readonly ITokenAcquisition tokenAcquisition;

        private HttpContext HttpContext { get; }

        public GroupsCheckHandler(ITokenAcquisition tokenAcquisition,IHttpContextAccessor httpContextAccessor)
        {
            this.tokenAcquisition = tokenAcquisition;
            HttpContext= httpContextAccessor.HttpContext;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                  GroupsCheckRequirement requirement)
        {
            var claims = context.User.Claims;

            var checkifclaimexists = claims.Any(i => i.Value.Equals(requirement.groups));

            if (checkifclaimexists)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
