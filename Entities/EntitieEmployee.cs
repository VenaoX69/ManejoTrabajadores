using System.ComponentModel.DataAnnotations;

namespace ManejoTrabajadores.Entities
{
  
    public class EntitieEmployee
    {
        // Se le indica a la tabla que esta propiedad sera la Clave primaria en la DB.
        [Key] public int EmployeeId { get; set; }
        public string IdentificationType { get; set; } = string.Empty;
        public string Names { get; set; } = string.Empty;
        public string LastNames { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password{ get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int ContractNumber { get; set; }
        public string? CityOfResidence { get; set; }
        public string? TechnicalRank { get; set; }
        public string? ExtensionRank { get; set; }
        public string Role { get; private set; } = "Employee"; // Por defecto


    }

}
