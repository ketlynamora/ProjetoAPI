using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjetoAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegurancaController : ControllerBase
    {
        private IConfiguration _config;

        public SegurancaController(IConfiguration Configuration)
        {
            _config = Configuration;
        }

        private bool ValidarUsuario(TokenLogin login)
        {
            if (login.Usuario == "Ketlyn" && login.Senha == "proxys1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GerarToken()
        {
            var issuer = _config["Jwt:Issuer"];
            var audicence = _config["Jwt:Audience"];
            var expiry = DateTime.Now.AddMinutes(120);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer : issuer, 
                audience: audicence,
                expires: expiry,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);

            return stringToken;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login ([FromBody]TokenLogin login)
        {
            bool resultado = ValidarUsuario(login);
            if (resultado)
            {
                var tokenString = GerarToken();
                return Ok(new TokenRetorno { Token = tokenString, DataTokenGerado = DateTime.Now });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
