using ManejoTrabajadores.ConectionDB;
using ManejoTrabajadores.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManejoTrabajadores.Controllers
{
    [ApiController]
    [Route("api/employee")] //  Estos atributos le dicen a ASP.NET Core que esta clase es un controlador de API y define la ruta base como api/Employee.
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context) // Se instancia la base de datos para poder utilizar EF Core
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntitieEmployee>>> GetEmployee()
        {
            return await _context.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntitieEmployee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            { 
                return NotFound();
            }

            return employee;
        }

        [HttpPost]
        public async Task<ActionResult<EntitieEmployee>> CreateEmployee(EntitieEmployee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateEmployee(int id, EntitieEmployee employee)
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

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id)
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

        private bool EmployeeExists (int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
        

    }
}
