# JsonResponses

Esta carpeta contiene ejemplos de **respuestas JSON** de la API (modo documentación), organizadas por recurso y endpoint.

> Nota: son ejemplos estáticos para ayudar al front / QA. Los valores (`id`, fechas, etc.) pueden variar según la base.

## Estructura

- `auth/`
- `especialidades/`
- `medicos/`
- `obrassociales/`
- `pacientes/`
- `turnos/`

Cada endpoint suele tener archivos como:

- `get_all.200.json`
- `get_by_id.200.json`
- `get_by_id.404.json`
- `create.201.json`
- `update.200.json`
- `delete.200.json`
- `*.500.json`

Los nombres siguen el patrón:

`<accion>.<status>.json`

Ejemplo: `create.201.json`.
