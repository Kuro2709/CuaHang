using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Owin;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CuaHang.App_Start
{
    public class JwtConfig
    {
        public static void ConfigureJwt(IAppBuilder app)
        {
            var issuer = System.Configuration.ConfigurationManager.AppSettings["Jwt:Issuer"];
            var audience = System.Configuration.ConfigurationManager.AppSettings["Jwt:Audience"];
            var secret = System.Configuration.ConfigurationManager.AppSettings["Jwt:Key"];

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                }
            });
        }
    }
}
