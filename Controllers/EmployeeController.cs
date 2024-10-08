using ManejoTrabajadores.ConectionDB;
using ManejoTrabajadores.DTO;
using ManejoTrabajadores.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManejoTrabajadores.Controllers
{
    [ApiController]
    [Route("api/employee")] //  Estos atributos le dicen a ASP.NET Core que esta clase es un controlador de API y define la ruta base como api/Employee.
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context, IConfiguration config) // Se instancia la base de datos para poder utilizar EF Core
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntitieEmployee>>> GetEmployee() // Se crea método Get para obtener a todos los empleados registrados.
        {
            return await _context.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntitieEmployee>> GetEmployee(int id) // Se crea método que obtendrá a un empleado por id
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            { 
                return NotFound();
            }

            return employee;
        }

        [HttpPost]
        public async Task<ActionResult<EntitieEmployee>> CreateEmployee(EntitieEmployee employee) // Método para crear un nuevo empleado.
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEmployee(int id, EntitieEmployee employee) // Método para editar infor de un empleado.
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }
            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)            
            {
                if (!EmployeeExists(id)) 
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id) // Método para elminar un empleado.
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool EmployeeExists (int id) // Verifica que el usuario existe
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
        
        // Método para manejar solicitud de Login
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);

            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }
            var token = GenerateToken(user);
            return Ok( new { token });
        }

        // Método para autenticar el usuario
        private EntitieEmployee Authenticate(UserLogin userLogin)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Email == userLogin.Email); // Busca en la base de datos un empleado cuyo email coincida con el email proporcionado.
            if (employee != null && employee.Password == userLogin.Password) // Compara la contraseña ingresada por el usuario con la almacenada en la base de datos
            {
                return employee; // Si las credenciales coinciden, devuelve el objeto EntitieEmployee correspondiente al empleado autenticado.
            }
            return null;
        }

        // Método para generar el token JWT.
        private string GenerateToken(EntitieEmployee entitieEmployee)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); // Se utiliza la clave de seguridad (_config["Jwt:Key"]) que está definida en el archivo appsettings.json. Esta clave debe ser secreta y se usa para firmar el token.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // Se usan las credenciales de firma para generar el token. Aquí se utiliza el algoritmo de firma HmacSha256.

            var claims = new[] // Un claim es una declaración sobre una entidad (generalmente el usuario).
            {
                new Claim(ClaimTypes.NameIdentifier, entitieEmployee.Email), // Se utiliza para identificar al usuario (en este caso, su email).
                new Claim(ClaimTypes.Role, entitieEmployee.Role)// Se añade el rol del usuario (en este caso, el primer rol en EmployeeRoles).
            };

            var token = new JwtSecurityToken( // Aquí se define el token:
                _config["Jwt:Issuer"], // Especifica quién emite el token y quién debe aceptarlo (también definidos en appsettings.json).
                _config["Jwt:Audience"], // Especifica quién emite el token y quién debe aceptarlo (también definidos en appsettings.json).
                claims, // Son los datos sobre el usuario y su rol.
                expires: DateTime.Now.AddMinutes(30), // Esta es la fecha y hora de expiración del token.
                signingCredentials: credentials); // las credenciales que se usarán para firmar el token.

            return new JwtSecurityTokenHandler().WriteToken(token); // Finalmente, se utiliza el método WriteToken para devolver el token como una cadena.
        }

    }
}
