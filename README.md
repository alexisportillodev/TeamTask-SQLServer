# 🗄️ Base de Datos – TeamTasks

## 📌 Descripción

Este proyecto implementa el modelo de base de datos para un sistema de gestión de tareas, permitiendo registrar usuarios y administrar tareas asociadas, incluyendo funcionalidades de filtrado, estados y manejo de información adicional en formato JSON.

---

## 🧱 Modelo de Datos

### 🔹 Tabla: Users

Almacena la información de los usuarios del sistema.

* **Id**: Identificador único (PK)
* **Name**: Nombre del usuario (obligatorio)
* **Email**: Correo electrónico (obligatorio, único)
* **CreatedAt**: Fecha de creación

---

### 🔹 Tabla: Tasks

Almacena las tareas asignadas a los usuarios.

* **Id**: Identificador único (PK)
* **Title**: Título de la tarea (obligatorio)
* **Description**: Descripción de la tarea
* **UserId**: Usuario asignado (FK a Users)
* **Status**: Estado de la tarea
  Valores permitidos:

  * `Pending`
  * `InProgress`
  * `Done`
* **CreatedAt**: Fecha de creación
* **AdditionalData**: Información adicional en formato JSON

---

## 🔗 Relaciones

* Una tarea pertenece a un usuario
* Relación: `Tasks.UserId → Users.Id`

---

## ⚡ Índices

Se creó un índice para optimizar consultas por usuario y estado:

* `IX_Tasks_UserId_Status`

---

## 🧠 Manejo de JSON

Se implementa una columna `AdditionalData` para almacenar información adicional en formato JSON.

### ✔ Validación

Se utiliza la función `ISJSON` para asegurar que el contenido sea válido.

### ✔ Ejemplo de estructura JSON

```json
{
  "priority": "High",
  "estimatedDate": "2026-04-25",
  "tags": ["backend", "api"]
}
```

### ✔ Consulta usando JSON

```sql
SELECT 
    Title,
    JSON_VALUE(AdditionalData, '$.priority') AS Priority
FROM Tasks
WHERE JSON_VALUE(AdditionalData, '$.priority') = 'High';
```

---

## 🔍 Consulta de tareas

Consulta que permite:

* Filtrar por usuario
* Filtrar por estado (opcional)
* Ordenar por fecha de creación

```sql
DECLARE @UserId INT = 1;
DECLARE @Status VARCHAR(20) = NULL;

SELECT *
FROM Tasks
WHERE UserId = @UserId
AND (@Status IS NULL OR Status = @Status)
ORDER BY CreatedAt DESC;
```

---

## ▶️ Ejecución

1. Ejecutar el script `DBSetup.sql` en SQL Server
2. Verificar creación de tablas:

   * Users
   * Tasks
3. Ejecutar las consultas de prueba incluidas

---

## 📌 Notas

* Se priorizó una estructura simple y alineada al requerimiento de la prueba técnica.
* El manejo de reglas de negocio (como validaciones de estado) se implementa en el backend.
* El uso de JSON permite flexibilidad para almacenar información adicional sin modificar el esquema.

---
