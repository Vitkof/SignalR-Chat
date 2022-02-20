using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace Server.Auth
{
    public static class JwtExtensions
    {
        private static AuthOptions _ao = new();

        internal static void OptionsFunctions(JwtBearerOptions options)
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                RequireAudience = true,
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidIssuer = _ao.Issuer,
                ValidAudience = _ao.Audience,
                IssuerSigningKey = _ao.PublicKey,
                ClockSkew = TimeSpan.Zero
            };
            
            options.SaveToken = true;
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = mrcontext =>
                {
                    var accessToken = mrcontext.Request.Query["token"];
                    if (!string.IsNullOrWhiteSpace(accessToken) &&
                    mrcontext.Request.Path.StartsWithSegments("/messages"))
                    {
                        mrcontext.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        }

        internal static string GetSha1(this string content)
        {
            byte[] hash;
            using var sha1 = new SHA1Managed();
            hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(content));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }


        public static void AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    OptionsFunctions(options);
                });
        }

        
        public static void AddJwtAuthentication(this IServiceCollection services,
                                                string rsaKeyPath, 
                                                string file,
                                                string issuer, 
                                                string audience)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var rsaKey = AuthOptions.GetPublic(rsaKeyPath, file);
                    _ao = new AuthOptions
                    {
                        Issuer = issuer,
                        Audience = audience,
                        SigningCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256Signature),
                        PublicKey = rsaKey
                    };
                    OptionsFunctions(options);
                });            
        }

        public static void AddJwtAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options => {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy("BadWordsPolicy", policy =>
                {
                    policy.Requirements.Add(new MyAuthPolicy());
                });
            });
        }
    }
}
