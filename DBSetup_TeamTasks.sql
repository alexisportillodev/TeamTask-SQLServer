-- 1. Crear base de datos
CREATE DATABASE TeamTasks;
GO

USE TeamTasks;
GO

-- 2. Tabla Users
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 3. Tabla Tasks
CREATE TABLE Tasks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(150) NOT NULL,
    Description VARCHAR(MAX),
    UserId INT NOT NULL,
    Status VARCHAR(20) NOT NULL CHECK (Status IN ('Pending', 'InProgress', 'Done')),
    CreatedAt DATETIME DEFAULT GETDATE(),

    -- 🔥 JSON requerido
    AdditionalData NVARCHAR(MAX),

    CONSTRAINT FK_Tasks_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- 4. Índice (OBLIGATORIO)
CREATE INDEX IX_Tasks_UserId_Status
ON Tasks(UserId, Status);

-- 5. Validación JSON
ALTER TABLE Tasks
ADD CONSTRAINT CK_Tasks_AdditionalData_JSON
CHECK (AdditionalData IS NULL OR ISJSON(AdditionalData) = 1);

-- 6. Consulta requerida (filtro + orden)
DECLARE @UserId INT = 1;
DECLARE @Status VARCHAR(20) = NULL;

SELECT *
FROM Tasks
WHERE UserId = @UserId
AND (@Status IS NULL OR Status = @Status)
ORDER BY CreatedAt DESC;

-- 7. Uso de JSON
SELECT 
    Title,
    JSON_VALUE(AdditionalData, '$.priority') AS Priority
FROM Tasks
WHERE JSON_VALUE(AdditionalData, '$.priority') = 'High';
