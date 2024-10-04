﻿using System.ComponentModel.DataAnnotations;

namespace ManejoTrabajadores.Entities
{
    public class EntitieAdmin
    {
        [Key]
        public int AdminId { get; set; }
        public string Names { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role {  get; set; } = "Admin";
    }
}
