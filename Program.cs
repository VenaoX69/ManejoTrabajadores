using ManejoTrabajadores.ConectionDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Configurar la autenticaci�n de Jwt
builder.Services.AddAuthentication(options => // Se configura el esquema de autenticaci�n de la aplicaci�n, que en este caso ser� JWT.
{
    // Esto asegura que la aplicaci�n sabe que debe usar tokens JWT para autenticar y manejar desaf�os.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //DefaultAuthenticateScheme define el esquema predeterminado que ser� utilizado cuando la aplicaci�n intente autenticar a un usuario. En este caso, son tokens JWT.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // DefaultChallengeScheme: Define c�mo manejar los desaf�os de autenticaci�n, como cuando un usuario no autenticado intenta acceder a un recurso protegido. En este caso, se indica que el desaf�o ser� tambi�n mediante JWT.
})
.AddJwtBearer(options => // Este m�todo a�ade la autenticaci�n JWT a la aplicaci�n.
{
    options.TokenValidationParameters = new TokenValidationParameters // Definici�n de las reglas para validar los tokens JWT.
    {
        ValidateIssuer = true, // Indica si se debe validar el emisor del token (Issuer).
        ValidateAudience = true, // Verifica que el p�blico para el cual fue creado el token coincida con la aplicaci�n.
        ValidateLifetime = true, // Valida que el token a�n no haya expirado. Esencial para limitar el tiempo que el token sera v�lido.
        ValidateIssuerSigningKey = true, // Se v�lida la firma del token utilizando una clave secreta. Garantizando que el token no ha sido manipulado y fue emitido por un servidor leg�timo.
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Especifica el emisor permitidos, que se obtienen de la configuraci�n (appsettings.json).
        ValidAudience = builder.Configuration["Jwt:Audience"],// Especifica el p�blico permitidos, que se obtienen de la configuraci�n (appsettings.json).
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Este se encarga de firmar el token, asegur�ndose de que sea leg�timo. En este caso, la clave se obtiene de la configuraci�n (Jwt:Key) y se codifica como un arreglo de bytes.
    };
});

// Configuraci�n de EF Core con SQL Server
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

app.UseAuthentication(); // Se indica que a la App que debe habilitar el middleware de autenticaci�n. Cada solicitud intentar� aunteticar al usuario si el recurso lo requiere.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
