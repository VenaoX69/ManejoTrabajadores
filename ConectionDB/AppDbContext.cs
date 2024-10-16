﻿using ManejoTrabajadores.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManejoTrabajadores.ConectionDB
{
    public class AppDbContext : DbContext // Se hereda de DbContext, que es la clase base en EF Core
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } // options son las configuraciones de conexion a la base de datos en el appsettings.json.
        public DbSet<EntitieEmployee> Employees { get; set; } = null!; // Conjunto de datos que representa la tabla de empleados en la base de datos.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntitieEmployee>()
                .HasIndex(e => e.Identificacion)
                .IsUnique();

            modelBuilder.Entity<EntitieEmployee>()
                .HasIndex( e => e.Email)
                .IsUnique();

            modelBuilder.Entity<EntitieEmployee>()
                .HasIndex( e => e.ContractNumber)
                .IsUnique();
        }

        public DbSet<EntitieAdmin> Admins { get; set; } = null!;
    }
}
