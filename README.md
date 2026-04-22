# TeamTasks (SQL Server + .NET 10)

API simple para gestión de **usuarios** y **tareas** usando **SQL Server** y **.NET 10 Web API**.

## Avance

- **Base de datos**: script `DBSetup_TeamTasks.sql` (tablas `Users` y `Tasks`, índice `IX_Tasks_UserId_Status`, validación JSON en `AdditionalData`)
- **Backend** (`backend/`):
  - Arquitectura simple en capas: `Controllers`, `Services`, `Repositories`, `DTOs`, `Models`
  - Endpoints implementados:
    - `POST /api/users`
    - `GET /api/users`
    - `POST /api/tasks`
    - `GET /api/tasks`
    - `PUT /api/tasks/{id}/status`
  - Reglas de negocio:
    - `Title` obligatorio
    - `UserId` debe existir
    - estado inicial: `Pending`
    - no se permite `Pending → Done`
  - Tests unitarios: regla de transición de estado (`TaskStatusRules`)

## Requisitos

- **.NET SDK 10**
- **SQL Server** (o **LocalDB**) + SSMS (o cualquier cliente SQL)

## Iniciar en local

### 1) Crear la base de datos

Ejecuta el script:

- `DBSetup_TeamTasks.sql`

Esto crea la base de datos `TeamTasks` y sus tablas.

### 2) Configurar la cadena de conexión

Edita `backend/TeamTasks.Api/appsettings.json`:

- **LocalDB** (ejemplo):
  - `Server=(localdb)\\MSSQLLocalDB;Database=TeamTasks;Trusted_Connection=True;`
- **SQL Server** (Windows Auth, ejemplo):
  - `Server=localhost;Database=TeamTasks;Trusted_Connection=True;TrustServerCertificate=True`
- **SQL Server** (usuario/clave, ejemplo):
  - `Server=localhost;Database=TeamTasks;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True`

### 3) Ejecutar la API

Desde la raíz del repo:

```bash
dotnet run --project backend/TeamTasks.Api
```

### 4) Probar rápido (ejemplos)

Crear usuario:

```http
POST /api/users
Content-Type: application/json

{ "name": "Alex", "email": "alex@test.com" }
```

Crear tarea (estado inicial siempre `Pending`):

```http
POST /api/tasks
Content-Type: application/json

{
  "title": "Primera tarea",
  "description": "Demo",
  "userId": 1,
  "additionalData": "{\"priority\":\"High\"}"
}
```

Actualizar estado (no permite `Pending -> Done` directo):

```http
PUT /api/tasks/1/status
Content-Type: application/json

{ "status": "InProgress" }
```

## Tests

```bash
dotnet test backend/TeamTasks.slnx -c Release
```
