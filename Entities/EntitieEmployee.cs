using System.ComponentModel.DataAnnotations;

namespace ManejoTrabajadores.Entities
{
  
    public class EntitieEmployee
    {
        // Se le indica a la tabla que esta propiedad sera la Clave primaria en la DB.
        [Key] public int EmployeeId { get; set; }
        
        [Required]
        public string Identificacion { get; set; }

        [Required]
        public string IdentificationType { get; set; } = string.Empty;

        [Required]
        public string Names { get; set; } = string.Empty;

        [Required]
        public string LastNames { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password{ get; set; } = string.Empty;
        
        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public int ContractNumber { get; set; }
        
        public string? CityOfResidence { get; set; }

        [Required]
        public string? TechnicalRank { get; set; }

        [Required]
        public string? ExtensionRank { get; set; }

        public string Role { get; private set; } = "Employee"; // Por defecto


    }

}
