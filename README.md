# TeamTasks - Sistema de Gestión de Tareas

**API REST completa para gestión de usuarios y tareas** usando **.NET 10 Web API**, **SQL Server** y **Angular 21**.

---

## 📋 Contexto del Negocio

Una empresa requiere un sistema interno de gestión de tareas que permita registrar, asignar y hacer seguimiento al estado de las tareas asociadas a sus colaboradores. El sistema debe facilitar la administración de usuarios internos y el control centralizado de proyectos y actividades.

---

## ✨ Requerimientos Funcionales

### 3.1 Gestión de Usuarios
- ✅ Crear usuarios (POST `/api/users`)
- ✅ Listar usuarios (GET `/api/users`)
- ✅ Cada usuario cuenta con **nombre** y **correo electrónico** únicos

### 3.2 Gestión de Tareas
- ✅ Crear tareas (POST `/api/tasks`)
- ✅ Asignar una tarea a un usuario
- ✅ Listar tareas con filtros (GET `/api/tasks`)
- ✅ Cambiar el estado de una tarea (PUT `/api/tasks/{id}/status`)
- ✅ Estados permitidos: `Pending`, `InProgress`, `Done`

---

## 🔐 Reglas de Negocio Implementadas

| Regla | Estado | Detalles |
|-------|--------|----------|
| Título obligatorio | ✅ | Se valida en la capa de servicio |
| Usuario debe existir | ✅ | Validación de `UserId` contra la base de datos |
| Estado inicial `Pending` | ✅ | Asignado automáticamente al crear tarea |
| No permitir `Pending → Done` directo | ✅ | Implementado en `TaskStatusRules.cs` |
| Email único | ✅ | Constraint único en tabla `Users` |
| JSON válido en `AdditionalData` | ✅ | Validación ISJSON en BD + validación JSON en servicio |

---

## 🏗️ Arquitectura Backend (.NET 10)

### Estructura en Capas

```
TeamTasks.Api/
├── Controllers/          # Punto de entrada HTTP
├── Services/             # Lógica de negocio
├── Repositories/         # Acceso a datos
├── Data/                 # DbContext (EF Core)
├── DTOs/                 # Data Transfer Objects
├── Models/               # Modelos de dominio
└── Properties/           # Configuración
```

### Descripción de Capas

#### **Controllers** (`Controllers/`)
- **TasksController.cs**: Endpoints CRUD de tareas
  - `POST /api/tasks` - Crear tarea
  - `GET /api/tasks` - Listar todas las tareas
  - `PUT /api/tasks/{id}/status` - Actualizar estado

- **UsersController.cs**: Endpoints CRUD de usuarios
  - `POST /api/users` - Crear usuario
  - `GET /api/users` - Listar todos los usuarios

#### **Services** (`Services/`)
- **ITaskService / TaskService**: Orquesta lógica de tareas
  - Validaciones de negocio
  - Verificación de `UserId` existente
  - Validación de JSON en `AdditionalData`
  - Reglas de transición de estados

- **IUserService / UserService**: Orquesta lógica de usuarios
  - Validación de email único
  - Validación de campos requeridos

- **TaskStatusRules.cs**: Regla estática para transiciones de estado
  - Previene: `Pending → Done` directo

#### **Repositories** (`Repositories/`)
- **ITaskRepository / TaskRepository**: Acceso a datos de tareas
- **IUserRepository / UserRepository**: Acceso a datos de usuarios
- Métodos: `GetByIdAsync()`, `GetAllAsync()`, `AddAsync()`, `SaveChangesAsync()`

#### **Data** (`Data/`)
- **TeamTasksDbContext.cs**: DbContext con Entity Framework Core
  - Configuración de modelos
  - Índices y constraints
  - Conversión de enums

#### **DTOs** (`DTOs/`)
- **CreateUserDto**: `{ name: string, email: string }`
- **CreateTaskDto**: `{ title: string, description?: string, userId: number, additionalData?: string }`
- **UpdateTaskStatusDto**: `{ status: string }`

#### **Models** (`Models/`)
- **User**: `{ Id, Name, Email, CreatedAt, Tasks[] }`
- **TaskItem**: `{ Id, Title, Description, UserId, Status, CreatedAt, AdditionalData, User }`
- **TaskStatus**: Enum `{ Pending = 0, InProgress = 1, Done = 2 }`

### Características Implementadas

- ✅ **Inyección de dependencias**: Servicios y Repositorios registrados en `Program.cs`
- ✅ **Entity Framework Core**: ORM para SQL Server
- ✅ **CORS configurado**: Permite requests desde `http://localhost:4200` (Angular)
- ✅ **Manejo de errores**: Try-catch en controllers, validaciones en servicios
- ✅ **Async/Await**: Operaciones asincrónicas con `CancellationToken`
- ✅ **OpenAPI/Swagger**: Disponible en `/openapi/v1.json`

---

## 🗄️ Base de Datos (SQL Server)

### Modelo de Datos

#### Tabla `Users`
```sql
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT GETDATE()
);
```

#### Tabla `Tasks`
```sql
CREATE TABLE Tasks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(150) NOT NULL,
    Description VARCHAR(MAX),
    UserId INT NOT NULL,
    Status VARCHAR(20) NOT NULL CHECK (Status IN ('Pending', 'InProgress', 'Done')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    AdditionalData NVARCHAR(MAX),
    
    CONSTRAINT FK_Tasks_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT CK_Tasks_AdditionalData_JSON CHECK (AdditionalData IS NULL OR ISJSON(AdditionalData) = 1)
);
```

### Índices

```sql
CREATE INDEX IX_Tasks_UserId_Status ON Tasks(UserId, Status);
```
**Propósito**: Optimizar consultas de filtrado por usuario y estado.

### Validación JSON

- **Constraint**: `CK_Tasks_AdditionalData_JSON`
- **Función**: `ISJSON(AdditionalData) = 1` garantiza que solo JSON válido se almacene
- **Ejemplo de datos válidos**:
  ```json
  {
    "priority": "High",
    "estimatedDate": "2026-05-15",
    "tags": ["backend", "urgent"],
    "metadata": {
      "assignedBy": "manager@company.com",
      "department": "Engineering"
    }
  }
  ```

### Consultas Avanzadas con JSON

#### 1. **Obtener tareas de un usuario con estado específico**
```sql
DECLARE @UserId INT = 1;
DECLARE @Status VARCHAR(20) = 'InProgress';

SELECT 
    t.Id,
    t.Title,
    t.Description,
    t.Status,
    t.CreatedAt,
    t.AdditionalData
FROM Tasks t
WHERE t.UserId = @UserId
AND (@Status IS NULL OR t.Status = @Status)
ORDER BY t.CreatedAt DESC;
```

#### 2. **Filtrar tareas por prioridad en JSON**
```sql
SELECT 
    t.Id,
    t.Title,
    JSON_VALUE(t.AdditionalData, '$.priority') AS Priority,
    t.Status
FROM Tasks t
WHERE JSON_VALUE(t.AdditionalData, '$.priority') = 'High'
AND t.Status != 'Done'
ORDER BY t.CreatedAt DESC;
```

#### 3. **Extraer múltiples valores del JSON**
```sql
SELECT 
    t.Id,
    t.Title,
    JSON_VALUE(t.AdditionalData, '$.priority') AS Priority,
    JSON_VALUE(t.AdditionalData, '$.estimatedDate') AS EstimatedDate,
    JSON_QUERY(t.AdditionalData, '$.tags') AS Tags
FROM Tasks t
WHERE t.AdditionalData IS NOT NULL;
```

#### 4. **Contar tareas por prioridad**
```sql
SELECT 
    JSON_VALUE(t.AdditionalData, '$.priority') AS Priority,
    COUNT(*) AS TaskCount
FROM Tasks t
WHERE t.AdditionalData IS NOT NULL
GROUP BY JSON_VALUE(t.AdditionalData, '$.priority')
ORDER BY TaskCount DESC;
```

#### 5. **Actualizar un valor dentro del JSON**
```sql
UPDATE Tasks
SET AdditionalData = JSON_MODIFY(AdditionalData, '$.priority', 'Low')
WHERE Id = 1;
```

#### 6. **Expandir array de JSON (tags)**
```sql
SELECT 
    t.Id,
    t.Title,
    JSON_VALUE(tag, '$') AS Tag
FROM Tasks t
CROSS APPLY OPENJSON(t.AdditionalData, '$.tags') AS tags(tag)
WHERE t.AdditionalData IS NOT NULL;
```

---

## 🎨 Frontend (Angular 21)

### Estructura

```
frontend/
├── src/
│   ├── app/
│   │   ├── app.config.ts          # Configuración de Angular
│   │   ├── app.ts                 # Componente raíz
│   │   ├── components/
│   │   │   ├── task-form/         # Formulario de crear/editar tareas
│   │   │   ├── task-list/         # Listado de tareas con filtros
│   │   │   └── user-panel/        # Panel de selección de usuarios
│   │   ├── models/
│   │   │   ├── task.ts            # Modelo TaskItem
│   │   │   └── user.ts            # Modelo User
│   │   └── services/
│   │       ├── api.config.ts      # Configuración de URL base
│   │       ├── task.service.ts    # Servicio de tareas
│   │       └── user.service.ts    # Servicio de usuarios
│   ├── index.html
│   ├── main.ts                    # Entry point
│   └── styles.css
├── package.json
├── angular.json
└── tsconfig.json
```

### Características Implementadas

- ✅ **Componentes standalone**: Arquitectura moderna de Angular
- ✅ **Signals**: Estado reactivo con `signal()` y `computed()`
- ✅ **Reactive Forms**: Validación de formularios
- ✅ **Servicios inyectables**: `TaskService`, `UserService`
- ✅ **HttpClient**: Comunicación con API
- ✅ **Manejo de errores**: `catchError()` en observables
- ✅ **Filtrado por estado**: Computed signal `filteredTasks`
- ✅ **TypeScript 5.9**: Tipado fuerte

### Componentes Principales

#### **task-form**
- Crear nuevas tareas
- Seleccionar usuario
- Capturar título, descripción y datos adicionales (JSON)

#### **task-list**
- Mostrar todas las tareas
- Filtrar por estado (`Pending`, `InProgress`, `Done`)
- Cambiar estado de tarea
- Mostrar información JSON adicional

#### **user-panel**
- Listar usuarios disponibles
- Mostrar contador de tareas por usuario

---

## ⚙️ Decisiones Técnicas

### Backend

| Decisión | Justificación |
|----------|---------------|
| **.NET 10** | Versión LTS reciente con soporte a largo plazo |
| **Entity Framework Core** | ORM moderno, integrado con .NET, facilita consultas tipadas |
| **Inyección de dependencias** | Patrón estándar en .NET, facilita testing y mantenibilidad |
| **Servicios y Repositories** | Separación de responsabilidades y reutilización de código |
| **DTOs** | Desacoplamiento entre API y modelos internos |
| **CORS explícito** | Seguridad: solo permite requests del frontend autorizado |
| **TaskStatusRules estático** | Reutilizable en controladores, servicios y tests |
| **Validación JSON en servicio + BD** | Doble validación: seguridad y consistencia |

### Base de Datos

| Decisión | Justificación |
|----------|---------------|
| **SQL Server** | Integración nativa con .NET, funciones JSON avanzadas |
| **Índice compuesto** `(UserId, Status)` | Optimiza consultas de filtrado comunes |
| **JSON en NVARCHAR(MAX)** | Flexibilidad para datos adicionales sin cambiar esquema |
| **ISJSON constraint** | Garantiza integridad de datos a nivel BD |
| **Foreign Key con Restrict** | Evita borrar usuarios con tareas asociadas |
| **Identity(1,1)** | Auto-incremento automático en claves primarias |

### Frontend

| Decisión | Justificación |
|----------|---------------|
| **Angular 21** | Framework moderno con características actuales |
| **Standalone components** | Simplifica la arquitectura, reduce boilerplate |
| **Signals** | Reactividad performante sin RxJS excesivo |
| **HttpClient** | Cliente estándar y tipado para HTTP |
| **TypeScript 5.9** | Tipado fuerte, mejor experiencia en editor |

---

## 📦 Requisitos

- **.NET SDK 10** ([Descargar](https://dotnet.microsoft.com/download))
- **SQL Server** (LocalDB o instancia completa) ([Descargar](https://www.microsoft.com/es-es/sql-server/sql-server-downloads))
- **Node.js 18+** y **npm 11+** (para frontend)
- **Angular CLI** (instalado vía npm)

---

## 🚀 Guía de Instalación y Ejecución

### Paso 1: Crear la Base de Datos

1. Abre **SSMS** (SQL Server Management Studio) o cualquier cliente SQL
2. Conecta a tu servidor SQL (LocalDB: `(localdb)\MSSQLLocalDB`)
3. Ejecuta el script **`DBSetup_TeamTasks.sql`**:
   ```sql
   -- Copiar y ejecutar todo el contenido de DBSetup_TeamTasks.sql
   ```
4. Verifica que se crearon las tablas:
   ```sql
   SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_CATALOG = 'TeamTasks';
   ```

### Paso 2: Configurar la Cadena de Conexión

Edita `backend/TeamTasks.Api/appsettings.json`:

#### Opción A: LocalDB (Windows)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TeamTasks;Trusted_Connection=True;"
  }
}
```

#### Opción B: SQL Server (Windows Authentication)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TeamTasks;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

#### Opción C: SQL Server (Usuario/Contraseña)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TeamTasks;User Id=sa;Password=TuPassword123;TrustServerCertificate=True"
  }
}
```

### Paso 3: Ejecutar el Backend

```bash
# Navega a la carpeta del backend
cd backend

# Restaura las dependencias
dotnet restore

# Ejecuta la API (por defecto en https://localhost:5001)
dotnet run --project TeamTasks.Api
```

Debería ver: `Started application in [...] ms`

### Paso 4: Ejecutar el Frontend

Abre **otra terminal**:

```bash
# Navega a la carpeta del frontend
cd frontend

# Instala dependencias
npm install

# Inicia el servidor de desarrollo (en http://localhost:4200)
ng serve
```

Abre el navegador en `http://localhost:4200`

### Paso 5: Verificar Conectividad

#### 5a. Probar Backend directamente

En Postman o VS Code (REST Client):

```http
GET https://localhost:5001/api/users
```

Respuesta esperada:
```json
[]
```

---

## 📝 Ejemplos de Uso

### 1. Crear un Usuario

```http
POST https://localhost:5001/api/users
Content-Type: application/json

{
  "name": "Juan García",
  "email": "juan.garcia@company.com"
}
```

**Respuesta 200 OK**:
```json
{
  "id": 1,
  "name": "Juan García",
  "email": "juan.garcia@company.com",
  "createdAt": "2026-04-22T10:30:00"
}
```

### 2. Listar Usuarios

```http
GET https://localhost:5001/api/users
```

**Respuesta 200 OK**:
```json
[
  {
    "id": 1,
    "name": "Juan García",
    "email": "juan.garcia@company.com",
    "createdAt": "2026-04-22T10:30:00"
  }
]
```

### 3. Crear una Tarea

```http
POST https://localhost:5001/api/tasks
Content-Type: application/json

{
  "title": "Implementar API REST",
  "description": "Crear endpoints para gestión de tareas",
  "userId": 1,
  "additionalData": "{\"priority\": \"High\", \"estimatedDate\": \"2026-05-15\", \"tags\": [\"backend\", \"urgent\"]}"
}
```

**Respuesta 200 OK**:
```json
{
  "id": 1,
  "title": "Implementar API REST",
  "status": "Pending",
  "userId": 1
}
```

### 4. Listar Todas las Tareas

```http
GET https://localhost:5001/api/tasks
```

**Respuesta 200 OK**:
```json
[
  {
    "id": 1,
    "title": "Implementar API REST",
    "description": "Crear endpoints para gestión de tareas",
    "status": "Pending",
    "userId": 1,
    "createdAt": "2026-04-22T10:35:00",
    "additionalData": "{\"priority\": \"High\", \"estimatedDate\": \"2026-05-15\", \"tags\": [\"backend\", \"urgent\"]}"
  }
]
```

### 5. Cambiar Estado de Tarea

```http
PUT https://localhost:5001/api/tasks/1/status
Content-Type: application/json

{
  "status": "InProgress"
}
```

**Respuesta 200 OK**:
```json
{
  "id": 1,
  "title": "Implementar API REST",
  "status": "InProgress",
  "userId": 1
}
```

### 6. Intentar Transición Inválida (Pending → Done)

```http
PUT https://localhost:5001/api/tasks/1/status
Content-Type: application/json

{
  "status": "Done"
}
```

**Respuesta 400 Bad Request**:
```json
{
  "error": "Cannot change status from Pending directly to Done."
}
```

---

## 🧪 Tests Unitarios

### Ejecutar Tests

```bash
cd backend

# Ejecutar todos los tests
dotnet test TeamTasks.slnx -c Release

# Ejecutar con verbose output
dotnet test TeamTasks.slnx -c Release -v detailed

# Ejecutar solo un proyecto de tests
dotnet test TeamTasks.Tests -c Release
```

### Cobertura de Tests

- **TaskStatusRulesTests.cs**: Valida todas las transiciones de estado permitidas y rechazadas

Ejemplo:
```csharp
[Theory]
[InlineData(TaskStatus.Pending, TaskStatus.Done)]
public void CannotChangeFromPendingToDoneDirectly(TaskStatus from, TaskStatus to)
{
    var result = TaskStatusRules.CanChangeStatus(from, to);
    Assert.False(result);
}
```

---

## ✅ Funcionalidades Implementadas

| Funcionalidad | Estado | Detalles |
|---------------|--------|----------|
| **Crear usuario** | ✅ | POST `/api/users` con validación de email único |
| **Listar usuarios** | ✅ | GET `/api/users` |
| **Crear tarea** | ✅ | POST `/api/tasks` con validación de userId y JSON |
| **Listar tareas** | ✅ | GET `/api/tasks` |
| **Cambiar estado tarea** | ✅ | PUT `/api/tasks/{id}/status` con reglas de transición |
| **Validar Pending→Done** | ✅ | Bloqueada a nivel de servicio |
| **Índice en BD** | ✅ | `IX_Tasks_UserId_Status` para optimizar consultas |
| **JSON en AdditionalData** | ✅ | Validación ISJSON + constraint en BD |
| **Consultas JSON avanzadas** | ✅ | Ejemplos en README con `JSON_VALUE`, `JSON_QUERY`, `OPENJSON` |
| **CORS configurado** | ✅ | Permite frontend en `localhost:4200` |
| **Frontend Angular** | ✅ | Componentes para CRUD de usuarios y tareas |
| **Filtrado en frontend** | ✅ | Filtro por estado de tarea con Signals |
| **Tests unitarios** | ✅ | `TaskStatusRulesTests.cs` |

---

## ⏳ Funcionalidades Pendientes / Mejoras Futuras

| Funcionalidad | Prioridad | Notas |
|---------------|-----------|-------|
| **Autenticación y Autorización** | Alta | Implementar JWT o OAuth2 |
| **Paginación en listados** | Media | Agregar skip/take en repositorios |
| **Búsqueda por texto** | Media | Buscar tareas por título o descripción |
| **Historial de cambios** | Media | Auditoría de transiciones de estado |
| **Asignación de tareas a múltiples usuarios** | Baja | Tabla de unión M2M |
| **Comentarios en tareas** | Baja | Tabla Comments con FK a Tasks |
| **Notificaciones en tiempo real** | Baja | SignalR para actualizaciones |
| **Validaciones en frontend** | Media | Mostrar errores específicos en formularios |
| **Tests de integración** | Alta | Tests E2E con la BD real |
| **Documentación Swagger/OpenAPI completa** | Media | Anotaciones [SwaggerResponse] en controllers |
| **Docker** | Baja | Containerizar backend y BD |
| **CI/CD Pipeline** | Media | GitHub Actions para tests y deployment |

---

## 📊 Manejo Avanzado de JSON en SQL Server

### Casos de Uso Implementados

#### 1. **Validación de JSON Válido**
```sql
-- Constraint en tabla Tasks
CONSTRAINT CK_Tasks_AdditionalData_JSON
CHECK (AdditionalData IS NULL OR ISJSON(AdditionalData) = 1)
```

#### 2. **Extracción de Valores Escalares**
```sql
-- Obtener prioridad de una tarea
SELECT JSON_VALUE(AdditionalData, '$.priority') AS Priority
FROM Tasks WHERE Id = 1;
-- Resultado: "High"
```

#### 3. **Extracción de Objetos/Arrays**
```sql
-- Obtener array de tags
SELECT JSON_QUERY(AdditionalData, '$.tags') AS Tags
FROM Tasks WHERE Id = 1;
-- Resultado: ["backend", "urgent"]
```

#### 4. **Iteración sobre Arrays**
```sql
-- Expandir array de tags
SELECT Tag
FROM Tasks
CROSS APPLY OPENJSON(AdditionalData, '$.tags') AS json_tags(Tag)
WHERE Id = 1;
-- Resultados: backend, urgent
```

#### 5. **Actualización de Valores JSON**
```sql
-- Cambiar prioridad de una tarea
UPDATE Tasks
SET AdditionalData = JSON_MODIFY(AdditionalData, '$.priority', 'Low')
WHERE Id = 1;
```

#### 6. **Inserción de Propiedades Nuevas**
```sql
-- Agregar nueva propiedad al JSON
UPDATE Tasks
SET AdditionalData = JSON_MODIFY(AdditionalData, '$.reviewedBy', 'manager@company.com')
WHERE Id = 1;
```

---

## 🐛 Solución de Problemas

### Error: "Cannot open database 'TeamTasks'"
**Causa**: El script SQL no se ejecutó correctamente
**Solución**:
1. Abre SSMS
2. Conecta al servidor SQL
3. Ejecuta nuevamente `DBSetup_TeamTasks.sql`
4. Verifica que no haya errores

### Error: "The host tried to reuse a disposed object"
**Causa**: Problema con DbContext
**Solución**:
1. Detén el servidor backend
2. Limpia la carpeta `bin` y `obj`
3. Ejecuta `dotnet clean` y `dotnet build`
4. Reinicia con `dotnet run`

### Error CORS: "Access to XMLHttpRequest blocked"
**Causa**: El frontend no está autorizado
**Solución**: Verifica que en `Program.cs` esté configurado:
```csharp
policy.WithOrigins("http://localhost:4200")
```

### Error: "Email already exists"
**Causa**: Intento de crear usuario con email duplicado
**Solución**: Usa un email diferente o borra el usuario existente desde SQL

---

## 📚 Tecnologías y Versiones

```
Frontend:
├── Angular: 21.2.0
├── TypeScript: 5.9.2
├── RxJS: 7.8.0
├── Node.js: 18+ (recomendado 20+)
└── npm: 11.11.0

Backend:
├── .NET: 10.0
├── Entity Framework Core: 10.0.x
├── C#: 12.0
└── SQL Server: 2019+ (o LocalDB)

Testing:
└── xUnit: 2.x (incluido en TeamTasks.Tests)
```

---

## 🔗 Referencias

- [.NET 10 Docs](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Angular 21 Guide](https://angular.io/docs)
- [SQL Server JSON Functions](https://learn.microsoft.com/en-us/sql/relational-databases/json/json-functions-sql-server)
- [HTTP Status Codes](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status)

---

## 📄 Licencia

Este proyecto es de carácter educativo. Todos los derechos reservados.

---

**Última actualización**: 22 de Abril de 2026
**Versión**: 1.0
