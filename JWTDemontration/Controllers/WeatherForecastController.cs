using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JWTDemontration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Get(string userName, string passWord)
        {
            if (userName != null && passWord != null)
            {
                if (userName != "Bharath" || passWord != "password")
                {
                    throw new UnauthorizedAccessException();
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is my secret key!!!"));

                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Email, "bharath@bharatyh.com"),
                new Claim(JwtRegisteredClaimNames.Sub, "bharath@bharatyh.com"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

                var token = new JwtSecurityToken(
                    issuer: "bharath", // this should be in the config file
                    audience: "Kumar",// this should be in the config file
                    claims,
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: cred
                    );


                var secToekn = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { secToekn });

            }

            throw new UnauthorizedAccessException();
        }
    }
}
