using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Auth
{
    public class MyAuthPolicy : AuthorizationHandler<MyAuthPolicy, HubInvocationContext>, IAuthorizationRequirement
    {
        private readonly string[] badWords = { 
            "moron", 
            "bitch", 
            "bastard", 
            "whore" 
        };

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MyAuthPolicy requirement, HubInvocationContext resource)
        {
            foreach(var word in badWords)
            {
                if (context.User.Identity.Name.Contains(word))
                {
                    context.Fail();
                }
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
