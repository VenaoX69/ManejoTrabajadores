using System.ComponentModel.DataAnnotations;

namespace ManejoTrabajadores.Entities
{
    public enum RoleUser
    {
        User, 
        Admin
    }
    public class EntitieEmployee
    {
        [Key] public int EmployeeId { get; set; }
        public string IdentificationType { get; set; }
        public string Names { get; set; }
        public string LatsNames { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int ContractNumber { get; set; }
        public string CityOfResidence { get; set; }
        public string TechnicalRank { get; set; }
        public string ExtensionRank { get; set; }

        // Relación muchos a muchos con RoleUser a través de EmployeeRole
        public List<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();
    }

    public class EmployeeRole
    {
        [Key]
        public int Id { get; set; }

        // Llave foránea hacia EntitieEmployee
        public int EmployeeId { get; set; }
        public EntitieEmployee Employee { get; set; }

        // Almacenar RoleUser como un valor enum
        public RoleUser Role { get; set; }

    }
}
