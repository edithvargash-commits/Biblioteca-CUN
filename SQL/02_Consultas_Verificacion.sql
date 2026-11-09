USE BibliotecaCUN;
GO
SELECT * FROM dbo.vInventario;
SELECT p.Id,u.Documento,u.Nombre,u.Apellido,l.Titulo,p.FechaPrestamo,p.FechaLimite,p.FechaDevolucion,p.Estado
FROM dbo.Prestamos p INNER JOIN dbo.Usuarios u ON u.Id=p.IdUsuario INNER JOIN dbo.Libros l ON l.Id=p.IdLibro;
-- Debe devolver cero filas despues de operar desde la aplicacion:
SELECT * FROM dbo.vInventario WHERE Disponibles<0;
-- En esta version Estado se calcula desde FechaDevolucion: no puede desincronizarse.
