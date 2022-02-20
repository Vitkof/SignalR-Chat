using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Auth
{
    public class Authenticator
    {
        private readonly AuthOptions _authOptions;
        private readonly JwtSecurityTokenHandler _jwtTokenHandler;


        internal Authenticator(AuthOptions ao)
        {
            _authOptions = ao;
            _jwtTokenHandler = new JwtSecurityTokenHandler();
        }


        public bool IsValidToken(string token)
        {

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                RequireAudience = true,
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidIssuer = _authOptions.Issuer,
                ValidAudience = _authOptions.Audience,
                IssuerSigningKey = _authOptions.PublicKey,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            try
            {
                var principal = _jwtTokenHandler.ValidateToken(token, validationParams, out validatedToken);
            }
            catch (Exception)
            {
                return false;
            }
            return validatedToken != null;
        }
    }    
}
