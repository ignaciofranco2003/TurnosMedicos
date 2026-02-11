# TurnosMedicos

API REST en **.NET 8** para la gestión de un centro médico: **Especialidades**, **Médicos**, **Obras Sociales**, **Pacientes** y **Turnos**. Incluye autenticación con JWT y autorización por roles.

## Qué resuelve este proyecto

- Administración de catálogos:
  - Especialidades.
  - Obras Sociales.
- Gestión operativa:
  - Médicos (incluye relación N..N con especialidades).
  - Pacientes.
  - Turnos (con estado y observaciones).
- Seguridad:
  - Registro/Login.
  - Protección de endpoints con JWT (Bearer token).
  - Roles para separar permisos entre `Admin` y `Usuario`.

## Resumen técnico

- Controladores REST + capa de servicios.
- Entity Framework Core con migraciones en `Migrations/`.
- Requests de ejemplo en `TurnosMedicos/http/`.
- Ejemplos de respuestas JSON (documentación) en `TurnosMedicos/JsonResponses/`.

## Requisitos
- .NET 8 SDK
- MySQL o MariaDB (cadena de conexión preparada desde variables de entorno)
- dotnet-ef (opcional, para aplicar migraciones)

## Variables de entorno
El proyecto usa variables de entorno para la conexión a la base de datos. Puedes crear un archivo `.env` en la raíz del proyecto (ya existe `.env` en la solución) con las siguientes variables:

```
DB__HOST=localhost
DB__PORT=3306
DB__NAME=prueba_db
DB__USER=usuario
DB__PASSWORD=contraseña
```

> Nota: el código construye la connection string a partir de `DB:HOST`, `DB:PORT`, `DB:NAME`, `DB:USER`, `DB:PASSWORD`.

## Ejecutar la API
1. Restaurar dependencias:

   `dotnet restore`

2. (Opcional) Aplicar migraciones si quieres crear/actualizar la base de datos:

   `dotnet tool install --global dotnet-ef` (si no lo tenés)

   `dotnet ef database update --project TurnosMedicos`

3. Ejecutar la API:

   `dotnet run --project TurnosMedicos`

Por defecto la variable `TurnosMedicos_HostAddress` en `TurnosMedicos/http` apunta a `https://localhost:8080`. Si ejecutás en otro puerto, actualizá la URL.

## Autenticación y roles

La API utiliza JWT.

1) `POST /api/auth/login` devuelve un token.
2) En las demás requests enviar el header:

`Authorization: Bearer <token>`

### Roles

- `Admin`: administrador completo.
- `Usuario`: administrador con permisos recortados (por ejemplo, secretaria).

### Permisos (resumen)

- `Especialidades` (`/api/especialidades`)
  - `Admin`: CRUD.
  - `Usuario`: lectura (ideal para combos/listas); altas/bajas/modificación solo `Admin`.
- `Obras Sociales` (`/api/obrassociales`)
  - `Admin`: CRUD.
  - `Usuario`: lectura; altas/bajas/modificación solo `Admin`.
- `Médicos` (`/api/medicos`)
  - Lectura: cualquier usuario autenticado.
  - ABM + asignar especialidades: solo `Admin`.
- `Pacientes` (`/api/pacientes`)
  - `Admin` y `Usuario`: crear/leer/actualizar.
  - **Eliminar**: solo `Admin`.
- `Turnos` (`/api/turnos`)
  - `Admin` y `Usuario`: crear/leer/actualizar + estados.
  - **Eliminar**: solo `Admin`.

## Habilitar Swagger (opcional)
En `Program.cs` las llamadas a Swagger están comentadas. Para habilitar la UI descomentar las líneas:

```csharp
//builder.Services.AddSwaggerGen();
//app.UseSwagger();
//app.UseSwaggerUI();
```

y mover `AddSwaggerGen()` antes de `builder.Build()` (si corresponde) o seguir la configuración típica de Swagger en .NET.

## Endpoints principales
Los endpoints están organizados por controlador. Para ejemplos de solicitudes listas para usar, ver la carpeta `TurnosMedicos/http` (archivos `.http`). A continuación un resumen rápido:

- Especialidades: `GET/POST/PUT/DELETE /api/especialidades`
  - DTO request: `EspecialidadRequestDto { NombreEspecialidad }`
  - DTO response: `EspecialidadResponseDto { Id, NombreEspecialidad }`

- Médicos: `GET/POST/PUT/DELETE /api/medicos`
  - DTO request/response: `MedicoRequestDto` / `MedicoResponseDto`:
    - `Nombre`, `DNI`, `Telefono`, `Matricula`, `DuracionTurnoMin`
  - Asignación de especialidades (reemplaza las existentes):
    - `POST /api/medicos/{id}/especialidades`
    - Request:
      - `AsignarEspecialidadesRequestDto { EspecialidadesNombres: string[] }`
    - Nota: el front envía una lista con **uno o varios** nombres.

- Obras sociales: `GET/POST/PUT/DELETE /api/obrassociales`
  - DTO request/response: `ObraSocialRequestDto` / `ObraSocialResponseDto`:
    - `Nombre`

- Pacientes: `GET/POST/PUT/DELETE /api/pacientes`
  - DTO request/response: `PacienteRequestDto` / `PacienteResponseDto`:
    - `Nombre`, `DNI`, `Telefono`, `TieneObraSocial`, `IdObraSocial` (nullable)

- Turnos: `GET/POST/PUT/DELETE /api/turnos` + `GET /api/turnos/estados`
  - DTO request: `TurnoRequestDto`:
    - `IdPaciente`, `IdMedico`, `Inicio` (DateTime), `Fin` (DateTime), `Estado?`, `Observaciones?`
  - DTO response: `TurnoResponseDto`:
    - `Id`, `IdPaciente?`, `IdMedico?`, `Inicio`, `Fin`, `Estado`, `Observaciones?`, `NombrePaciente?`, `NombreMedico?`

## Observaciones importantes
- Los controladores capturan excepciones y devuelven 500 en errores no previstos. Para validaciones específicas (por ejemplo en `Turno`) se lanzan `ArgumentException` y se devuelven 400.
- Reemplazá los ids en los ejemplos por los existentes en tu base de datos.
- El proyecto ya incluye migraciones en `Migrations/` si querés usar la DB proporcionada por esas migraciones.

## Archivo con ejemplos de consumo
`TurnosMedicos/http` contiene ejemplos listos para ejecutar (GET/POST/PUT/DELETE) para cada endpoint. Recomendado abrir con la extensión REST Client de VS Code o importar a Postman.
