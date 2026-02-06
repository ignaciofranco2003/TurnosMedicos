using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Services;
using TurnosMedicos.Services.Interfaces;
using System;

namespace TurnosMedicos.Config;

public static class DependencyInjection
{
/* Para conexion a una DB con string
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("No se encontró la connection string 'DefaultConnection' en la configuración");

        services.AddDbContext<PersonaDb>(options =>
            options.UseMySql(cs, ServerVersion.AutoDetect(cs)));

        services.AddScoped<IPersonaService, PersonaService>();

        return services;
    }
*/

    public static IServiceCollection AddAppServices(this IServiceCollection services,IConfiguration config)
    {
        var host = config["DB:HOST"];
        var port = config["DB:PORT"];
        var name = config["DB:NAME"];
        var user = config["DB:USER"];
        var pass = config["DB:PASSWORD"];

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(user))
        {
            throw new InvalidOperationException(
                "Faltan variables de entorno de la base de datos (DB__*)");
        }

        var cs = $"Server={host};Port={port};Database={name};User={user};Password={pass};";

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(cs, ServerVersion.AutoDetect(cs)));


        services.AddScoped<ITurnoService, TurnoService>();
        services.AddScoped<IObraSocialService, ObraSocialService>();
        services.AddScoped<IEspecialidadService, EspecialidadService>();
        services.AddScoped<IPacienteService, PacienteService>();
        services.AddScoped<IMedicoService, MedicoService>();

        // Auth
        services.AddScoped<TurnosMedicos.Auth.Interfaces.IAuthService, TurnosMedicos.Auth.Services.AuthService>();


        return services;
    }
}
