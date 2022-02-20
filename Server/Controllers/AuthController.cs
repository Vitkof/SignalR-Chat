using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Auth;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly FakeUsers _users;
        private readonly AuthOptions _options = new();


        public AuthController(IOptions<FakeUsers> fakeUsers)
        {
            _users = fakeUsers.Value;
        }

        [HttpPost("token")]
        public IActionResult Token([FromBody] AuthModel authModel)
        {
            var user = _users.Users.FirstOrDefault(x => x.Login == authModel.Login);

            if (user == null)
                return BadRequest();

            if (user.PasswordHash != authModel.Password.GetSha1())
                return BadRequest();

            var token = GetJwt(user);

            return Ok(token);
        }

        private string GetJwt(FakeUser fakeUser)
        {
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                notBefore: now,
                claims: GetClaims(fakeUser),
                expires: now.AddMinutes(_options.Lifetime),
                signingCredentials: _options.SigningCredentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);
            return tokenString;
        }

        private static IEnumerable<Claim> GetClaims(FakeUser fake)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, fake.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, fake.Role),
                new Claim(ClaimTypes.NameIdentifier, fake.Login)
            };

            return claims;
        }
    }
}
