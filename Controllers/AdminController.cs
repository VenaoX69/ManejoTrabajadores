using ManejoTrabajadores.ConectionDB;
using ManejoTrabajadores.DTO;
using ManejoTrabajadores.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks; // Para Task


namespace ManejoTrabajadores.Controllers
{
    [ApiController]
    [Route("api/Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configAdmin;
        private readonly AppDbContext _contextAdmin;

        public AdminController(AppDbContext contextAdmin, IConfiguration configAdmin)
        {
            _contextAdmin = contextAdmin;
            _configAdmin = configAdmin;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<EntitieAdmin>> GetAdmin(int id)
        {
            var admin = await _contextAdmin.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return admin;
        }


        [HttpPost]
        public async Task<ActionResult<EntitieAdmin>> CreateAdmin(EntitieAdmin admin)
        {
            _contextAdmin.Admins.Add(admin);
            await _contextAdmin.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdmin), new { id = admin.AdminId }, admin);

        }

        // Solicitud para iniciar sesion
        [HttpPost("loginAdmin")]
        public IActionResult Login([FromBody] AdminLogin adminLogin)
        {
            var admin = Authenticate(adminLogin);

            if (admin == null)
            {
                return NotFound("Administrador no encontrado");
            }

            var token = GenerateTokenAdmin(admin);
            return Ok(new { token });
        }

        // Autenticar al admin
        private EntitieAdmin Authenticate(AdminLogin adminLogin)
        {
            var admin = _contextAdmin.Admins.FirstOrDefault(e => e.Email == adminLogin.Email);
            if (admin != null && admin.Password == adminLogin.Password)
            {
                return admin;
            }
            return null;
        }

        // Generar token JWT Admin
        private string GenerateTokenAdmin(EntitieAdmin entitieAdmin)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configAdmin["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, entitieAdmin.Email),
                new Claim(ClaimTypes.Role, entitieAdmin.Role)
            };

            var token = new JwtSecurityToken(
                _configAdmin["Jwt:Issuer"],
                _configAdmin["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
