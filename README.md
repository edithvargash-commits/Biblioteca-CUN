# Sistema de Gestión de Biblioteca CUN

Proyecto académico de Programación Avanzada desarrollado en C#, Windows Forms, .NET 8 y SQL Server.

Permite administrar libros, usuarios, autores, editoriales, préstamos y devoluciones.

## Funciones

- Registrar, consultar, modificar y eliminar registros.
- Buscar libros por código, ISBN, título o autor.
- Controlar los ejemplares disponibles.
- Registrar préstamos y devoluciones.
- Consultar el inventario y el historial de préstamos.
- Exportar información en formato CSV.

## Tecnologías

- C# y programación orientada a objetos.
- Windows Forms.
- .NET 8.
- SQL Server.
- Microsoft.Data.SqlClient.
- Git y GitHub.

## Organización del proyecto

| Carpeta o archivo | Contenido |
|---|---|
| AcaBiblioteca.sln | Solución de Visual Studio |
| src/Biblioteca.Entidades | Clases y entidades |
| src/Biblioteca.Datos | Acceso a datos SQL y JSON |
| src/Biblioteca.Negocio | Validaciones y reglas de negocio |
| src/Biblioteca.Presentacion | Interfaz gráfica |
| tests | Pruebas del sistema |
| SQL | Scripts de base de datos |
| Documentacion | Informe técnico, diagramas y evidencias |

## Ejecución

1. Descargar o clonar el repositorio.
2. Abrir `AcaBiblioteca.sln` en Visual Studio 2022 con la carga de trabajo **Desarrollo de escritorio de .NET** y el SDK de .NET 8.
3. Restaurar los paquetes NuGet.
4. Establecer `Biblioteca.Presentacion` como proyecto de inicio.
5. Presionar **F5**.

La aplicación dispone de un modo de demostración local. Para trabajar con SQL Server, crear la base de datos mediante el script de la carpeta `SQL` y configurar la conexión desde el módulo **Conexión**.

## Documentación académica

- [Documentación técnica](Documentacion/DOCUMENTACION_TECNICA.md)
- [Diagramas](Documentacion/DIAGRAMAS.md)
- [Evidencias de funcionamiento](Documentacion/Evidencias/INDICE.md)

## Verificación

Se aprobaron 23 pruebas automatizadas de negocio y persistencia JSON.

También se realizaron comprobaciones manuales en Windows de la conexión con SQL Server, las operaciones de registro, modificación y eliminación, los préstamos, las devoluciones y la conservación de los datos.

Las capturas de funcionamiento se encuentran en `Documentacion/Evidencias`.

## Repositorio

[Repositorio público Biblioteca-CUN](https://github.com/edithvargash-commits/Biblioteca-CUN)
