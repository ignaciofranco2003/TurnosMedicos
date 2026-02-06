# TurnosMedicos

API backend en .NET 8 con ASP.NET Core para gestion de Turnos medicos.

Cuenta con especialidades, médicos, obras sociales, pacientes y turnos. 

Incluye CRUD completos por entidad y autenticación JWT.

## Contenido
- [Stack y alcance](#stack-y-alcance)
- [Requisitos](#requisitos)
- [Configuración](#configuración)
- [Puesta en marcha rápida](#puesta-en-marcha-rápida)
- [Documentación y ejemplos](#documentación-y-ejemplos)
- [Endpoints principales](#endpoints-principales)
- [Rutas de autenticación](#Rutas-de-autenticación)

## Stack y alcance
- **Framework**: .NET 8 + ASP.NET Core
- **Persistencia**: Entity Framework Core con MySQL/MariaDB
- **Arquitectura**: controladores REST por entidad (CRUD)
- **Migraciones**: incluidas en `Migrations/`
- **Seguridad**: JWT para autorización en los endpoints
- **CORS**: habilitado para `http://localhost:3000`

## Requisitos
- .NET 8 SDK
- MySQL o MariaDB
- `dotnet-ef` (opcional, para aplicar migraciones)

## Configuración
La conexión a base de datos se arma con variables de entorno. Podés crear un `.env` en la raíz del repo con las siguientes claves:

```dotenv
DB__HOST=localhost
DB__PORT=3306
DB__NAME=prueba_db
DB__USER=usuario
DB__PASSWORD=contraseña
```

> Nota: la app construye la connection string a partir de `DB:HOST`, `DB:PORT`, `DB:NAME`, `DB:USER`, `DB:PASSWORD`.

## Puesta en marcha rápida
1. Restaurar dependencias:

   ```bash
   dotnet restore
   ```

2. (Opcional) Aplicar migraciones:

   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef database update --project TurnosMedicos
   ```

3. Ejecutar la API:

   ```bash
   dotnet run --project TurnosMedicos
   ```

> La variable `@baseUrl` en `TurnosMedicos/http` apunta a `https://localhost:8080`. Ajustala si usás otro puerto.

## Documentación y ejemplos

- **Colección de requests**: `TurnosMedicos/http` contiene ejemplos listos para ejecutar (GET/POST/PUT/DELETE) y una guía de uso. Recomendado abrir con la extensión REST Client de VS Code o importar a Postman.
- **Dump de base de datos**: el proyecto incluye un dump para cargar datos de ejemplo. Usalo si querés probar rápidamente los endpoints con datos reales.

## Endpoints principales
Resumen rápido por entidad (todos con `GET/POST/PUT/DELETE`). **Todos los endpoints requieren JWT** en el header `Authorization: Bearer <token>`.

| Recurso | Ruta base | Qué hace | DTO request | DTO response |
| --- | --- | --- | --- | --- |
| Especialidades | `/api/especialidades` | Administra especialidades médicas disponibles. | `EspecialidadRequestDto { NombreEspecialidad }` | `EspecialidadResponseDto { Id, NombreEspecialidad }` |
| Médicos | `/api/medicos` | Administra médicos y su información de contacto. | `MedicoRequestDto { Nombre, DNI, Telefono, Matricula, DuracionTurnoMin }` | `MedicoResponseDto` |
| Obras sociales | `/api/obrassociales` | Administra obras sociales y sus datos básicos. | `ObraSocialRequestDto { Nombre }` | `ObraSocialResponseDto` |
| Pacientes | `/api/pacientes` | Administra pacientes y su obra social asociada. | `PacienteRequestDto { Nombre, DNI, Telefono, TieneObraSocial, IdObraSocial? }` | `PacienteResponseDto` |
| Turnos | `/api/turnos` | Gestiona turnos, horarios y estados. | `TurnoRequestDto { IdPaciente, IdMedico, Inicio, Fin, Estado?, Observaciones? }` | `TurnoResponseDto` |

Además, `GET /api/turnos/estados` devuelve los posibles estados de turno.

## Rutas de autenticación:
- `POST /api/auth/register`: registra un usuario.
- `POST /api/auth/login`: valida credenciales y devuelve el JWT.
