-- Instalación nueva; no sobrescribe una base existente. Ejecutar completo en SSMS.
USE master;
GO
IF DB_ID(N'BibliotecaCUN') IS NOT NULL
    THROW 51001,N'BibliotecaCUN ya existe. No se modificó. Use esa base si corresponde a esta versión o cambie de instancia.',1;
ELSE
BEGIN
    EXEC(N'CREATE DATABASE BibliotecaCUN');
    EXEC(N'USE BibliotecaCUN;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM sys.tables WHERE is_ms_shipped=0)
    THROW 51002,N''La base contiene tablas. Instalación detenida; no se han eliminado datos.'',1;

CREATE TABLE dbo.Autores(
 Id int NOT NULL CONSTRAINT PK_Autores PRIMARY KEY,
 Nombre nvarchar(100) NOT NULL,
 Apellido nvarchar(100) NOT NULL,
 CONSTRAINT CK_Autores_Id CHECK(Id>0),
 CONSTRAINT CK_Autores_Nombres CHECK(LEN(LTRIM(RTRIM(Nombre)))>0 AND LEN(LTRIM(RTRIM(Apellido)))>0)
);
CREATE TABLE dbo.Editoriales(
 Id int NOT NULL CONSTRAINT PK_Editoriales PRIMARY KEY,
 Nombre nvarchar(150) NOT NULL CONSTRAINT UQ_Editoriales_Nombre UNIQUE,
 CONSTRAINT CK_Editoriales_Id CHECK(Id>0),
 CONSTRAINT CK_Editoriales_Nombre CHECK(LEN(LTRIM(RTRIM(Nombre)))>0)
);
CREATE TABLE dbo.Usuarios(
 Id int NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
 Nombre nvarchar(100) NOT NULL,
 Apellido nvarchar(100) NOT NULL,
 Documento nvarchar(30) NOT NULL CONSTRAINT UQ_Usuarios_Documento UNIQUE,
 Telefono nvarchar(30) NOT NULL CONSTRAINT DF_Usuarios_Telefono DEFAULT N'''',
 Correo nvarchar(150) NOT NULL CONSTRAINT DF_Usuarios_Correo DEFAULT N'''',
 CONSTRAINT CK_Usuarios_Id CHECK(Id>0),
 CONSTRAINT CK_Usuarios_Datos CHECK(LEN(LTRIM(RTRIM(Nombre)))>0 AND LEN(LTRIM(RTRIM(Apellido)))>0 AND LEN(LTRIM(RTRIM(Documento)))>0)
);
CREATE TABLE dbo.Libros(
 Id int NOT NULL CONSTRAINT PK_Libros PRIMARY KEY,
 Codigo nvarchar(30) NOT NULL CONSTRAINT UQ_Libros_Codigo UNIQUE,
 ISBN varchar(13) NOT NULL CONSTRAINT UQ_Libros_ISBN UNIQUE,
 Titulo nvarchar(200) NOT NULL,
 IdAutor int NOT NULL CONSTRAINT FK_Libros_Autores REFERENCES dbo.Autores(Id),
 IdEditorial int NOT NULL CONSTRAINT FK_Libros_Editoriales REFERENCES dbo.Editoriales(Id),
 Categoria nvarchar(100) NOT NULL CONSTRAINT DF_Libros_Categoria DEFAULT N'''',
 Anio int NOT NULL,
 Existencias int NOT NULL,
 CONSTRAINT CK_Libros_Id CHECK(Id>0),
 CONSTRAINT CK_Libros_Obligatorios CHECK(LEN(LTRIM(RTRIM(Codigo)))>0 AND LEN(LTRIM(RTRIM(Titulo)))>0),
 CONSTRAINT CK_Libros_ISBN CHECK(LEN(ISBN) IN (10,13)),
 CONSTRAINT CK_Libros_Anio CHECK(Anio BETWEEN 1 AND 9999),
 CONSTRAINT CK_Libros_Existencias CHECK(Existencias BETWEEN 0 AND 1000000)
);
CREATE TABLE dbo.Prestamos(
 Id int NOT NULL CONSTRAINT PK_Prestamos PRIMARY KEY,
 IdUsuario int NOT NULL CONSTRAINT FK_Prestamos_Usuarios REFERENCES dbo.Usuarios(Id),
 IdLibro int NOT NULL CONSTRAINT FK_Prestamos_Libros REFERENCES dbo.Libros(Id),
 FechaPrestamo date NOT NULL,
 FechaLimite date NOT NULL,
 FechaDevolucion date NULL,
 Estado AS (CASE WHEN FechaDevolucion IS NULL THEN N''Prestado'' ELSE N''Devuelto'' END),
 CONSTRAINT CK_Prestamos_Id CHECK(Id>0),
 CONSTRAINT CK_Prestamos_Fechas CHECK(FechaLimite>=FechaPrestamo AND (FechaDevolucion IS NULL OR FechaDevolucion>=FechaPrestamo))
);
CREATE UNIQUE INDEX UX_Prestamos_Activo ON dbo.Prestamos(IdUsuario,IdLibro) WHERE FechaDevolucion IS NULL;
CREATE INDEX IX_Libros_Autor ON dbo.Libros(IdAutor);
CREATE INDEX IX_Libros_Editorial ON dbo.Libros(IdEditorial);
CREATE INDEX IX_Prestamos_Libro ON dbo.Prestamos(IdLibro,FechaDevolucion);
CREATE INDEX IX_Prestamos_Fechas ON dbo.Prestamos(FechaPrestamo,FechaLimite);

INSERT dbo.Autores(Id,Nombre,Apellido) VALUES(1,N''Gabriel'',N''García Márquez''),(2,N''George'',N''Orwell''),(3,N''Antoine'',N''de Saint-Exupéry'');
INSERT dbo.Editoriales(Id,Nombre) VALUES(1,N''Editorial de práctica A''),(2,N''Editorial de práctica B'');
INSERT dbo.Libros(Id,Codigo,ISBN,Titulo,IdAutor,IdEditorial,Categoria,Anio,Existencias) VALUES
 (1,N''LIB-001'',''9780307474728'',N''Cien años de soledad'',1,1,N''Novela'',1967,5),
 (2,N''LIB-002'',''9780451524935'',N''1984'',2,2,N''Distopía'',1949,3),
 (3,N''LIB-003'',''9780156012195'',N''El principito'',3,1,N''Literatura'',1943,4);
INSERT dbo.Usuarios(Id,Nombre,Apellido,Documento,Telefono,Correo) VALUES(1,N''Ana'',N''Ejemplo'',N''DEMO-001'',N''3000000000'',N''ana@example.com'');
EXEC(N''CREATE OR ALTER VIEW dbo.vInventario AS
SELECT l.Id,l.Codigo,l.ISBN,l.Titulo,l.Existencias,
 (SELECT COUNT(*) FROM dbo.Prestamos p WHERE p.IdLibro=l.Id AND p.FechaDevolucion IS NULL) AS Activos,
 l.Existencias-(SELECT COUNT(*) FROM dbo.Prestamos p WHERE p.IdLibro=l.Id AND p.FechaDevolucion IS NULL) AS Disponibles
FROM dbo.Libros l;'');
COMMIT;
');
    PRINT N'BibliotecaCUN creada correctamente.';
END;
GO
