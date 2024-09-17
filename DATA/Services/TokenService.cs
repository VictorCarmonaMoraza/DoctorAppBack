using DATA.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MODEL.Entity;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace DATA.Services
{
    public class TokenService : ITokenService
    {
        // Clave de seguridad simétrica usada para firmar el token
        private readonly SymmetricSecurityKey _key;

        // Constructor que toma la clave desde la configuración
        public TokenService(IConfiguration config)
        {
            // Se obtiene la clave secreta de la configuración, convertida a bytes
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        // Método que crea y retorna el JWT (token) para el usuario proporcionado
        public string CreateToken(User user)
        {
            // 1. Crear los "claims" (reclamaciones) que son información adicional dentro del token.
            // En este caso, se incluye el nombre de usuario como parte de las reclamaciones.
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            // 2. Crear las credenciales de firma.
            // Se usa la clave simétrica (_key) y se especifica el algoritmo de firma HMAC-SHA512.
            var credentiales = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // 3. Definir el descriptor del token.
            // Este objeto contiene los claims, la expiración del token y las credenciales de firma.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentiales
            };

            // 4. Crear el token utilizando el tokenHandler de JWT.
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor); // Se crea el token

            // 5. Retornar el token en formato string.
            return tokenHandler.WriteToken(token); // Convierte el token a su formato JWT
        }
    }
}
