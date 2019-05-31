using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WIM.Services.Security.Authentication.JWTBearer;

namespace GageStatsServices
{
    public class testClass:JWTBearerAuthenticationEvents
    {
        public override Task TokenValidated(TokenValidatedContext context)
        {
            return base.TokenValidated(context);
        }
    }
}
