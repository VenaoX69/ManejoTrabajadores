using ManejoTrabajadores.ConectionDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Configurar la autenticación de Jwt
builder.Services.AddAuthentication(options => // Se configura el esquema de autenticación de la aplicación, que en este caso será JWT.
{
    // Esto asegura que la aplicación sabe que debe usar tokens JWT para autenticar y manejar desafíos.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //DefaultAuthenticateScheme define el esquema predeterminado que será utilizado cuando la aplicación intente autenticar a un usuario. En este caso, son tokens JWT.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // DefaultChallengeScheme: Define cómo manejar los desafíos de autenticación, como cuando un usuario no autenticado intenta acceder a un recurso protegido. En este caso, se indica que el desafío será también mediante JWT.
})
.AddJwtBearer(options => // Este método añade la autenticación JWT a la aplicación.
{
    options.TokenValidationParameters = new TokenValidationParameters // Definición de las reglas para validar los tokens JWT.
    {
        ValidateIssuer = true, // Indica si se debe validar el emisor del token (Issuer).
        ValidateAudience = true, // Verifica que el público para el cual fue creado el token coincida con la aplicación.
        ValidateLifetime = true, // Valida que el token aún no haya expirado. Esencial para limitar el tiempo que el token sera válido.
        ValidateIssuerSigningKey = true, // Se válida la firma del token utilizando una clave secreta. Garantizando que el token no ha sido manipulado y fue emitido por un servidor legítimo.
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Especifica el emisor permitidos, que se obtienen de la configuración (appsettings.json).
        ValidAudience = builder.Configuration["Jwt:Audience"],// Especifica el público permitidos, que se obtienen de la configuración (appsettings.json).
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Este se encarga de firmar el token, asegurándose de que sea legítimo. En este caso, la clave se obtiene de la configuración (Jwt:Key) y se codifica como un arreglo de bytes.
    };
});

// Configuración de EF Core con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ManejoEmpleados")));

// Add services to the container.
// Configurar servicios de contorladores
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); // Se indica que a la App que debe habilitar el middleware de autenticación. Cada solicitud intentará aunteticar al usuario si el recurso lo requiere.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
