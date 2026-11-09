# Sistema de Gestión de Biblioteca CUN

**Edición rosa y nude.** Tipografía editorial, marca BIBLIOTECA completa, ilustración de libro integrada, tarjetas e interfaz renovada. Véase [DISENO_ROSA.md](Documentacion/DISENO_ROSA.md).

Aplicación académica reconstruida en C#, Windows Forms, .NET 8 y SQL Server. Incluye proyectos separados de entidades, datos, negocio, presentación y pruebas.

## Inicio

1. Extraer todo el ZIP en una carpeta nueva, por ejemplo `Documentos/BibliotecaCUN`.
2. Abrir **AcaBiblioteca.sln** con Visual Studio 2022 (17.8 o posterior), carga **Desarrollo de escritorio de .NET**, SDK .NET 8.
3. Restaurar paquetes NuGet y establecer **Biblioteca.Presentacion** como proyecto de inicio.
4. Presionar **F5**. La primera apertura usa la demostración local con datos de ejemplo.
5. Para usar SQL Server, seguir [la guía de uso](Documentacion/GUIA_DE_USO.md).

## Funciones

- Inicio con indicadores y navegación.
- CRUD de libros, autores, editoriales y usuarios.
- Búsqueda de libros por código, ISBN, título y autor.
- Listas de autores y editoriales cargadas desde el almacenamiento activo.
- Préstamos, edición de préstamos activos, devolución y eliminación confirmada.
- Inventario, historial, activos, devueltos y vencidos; exportación CSV compatible con Excel.
- Validación ISBN-10/13, unicidad, relaciones, campos y fechas.
- Disponibilidad calculada a partir de los ejemplares totales y préstamos activos.
- SQL parametrizado, transacciones y control de escrituras concurrentes desde la aplicación.
- Demostración persistente JSON separada de SQL Server.

## Estructura

| Ruta | Contenido |
|---|---|
| AcaBiblioteca.sln | Solución de Visual Studio con cinco proyectos |
| src/Biblioteca.Entidades | Clases del dominio y excepción de validación |
| src/Biblioteca.Datos | Contrato IAlmacen y almacenes SQL/JSON |
| src/Biblioteca.Negocio | Validaciones y operaciones de biblioteca |
| src/Biblioteca.Presentacion | Formularios Windows Forms |
| tests/Biblioteca.Pruebas | 23 pruebas automáticas y prueba SQL optativa |
| SQL | Instalación nueva y consultas de comprobación |
| Documentacion | Memoria técnica, manual, diagramas y validación |
| AplicacionWindows | Publicación adicional para Windows x64, sin runtime ni firma |
| historial.bundle | Historial Git portable del desarrollo de esta entrega |

## Comandos (terminal en esta carpeta, Windows)

```powershell
dotnet restore AcaBiblioteca.sln
dotnet build AcaBiblioteca.sln -c Release
dotnet run --project src/Biblioteca.Presentacion
dotnet run --project tests/Biblioteca.Pruebas
```

Publicar para un equipo con .NET Desktop Runtime 8 x64:

```powershell
dotnet publish src/Biblioteca.Presentacion -c Release -r win-x64 --self-contained false -o Publicacion
```

Las dependencias de NuGet quedan fijadas en los proyectos; `Microsoft.Data.SqlClient` usa 6.1.7. Se requiere acceso a NuGet para la primera restauración.

## Estado de verificación

Compilación Release y publicación Windows x64 realizadas; 23 pruebas automatizadas de negocio/JSON aprobadas. El usuario comprobó la interfaz en Windows, la conexión SQL, el ciclo de préstamo/devolución, la conservación de datos y el CRUD. Se incorporan siete capturas reales en Documentacion/Evidencias y en la memoria técnica. La prueba SQL automatizada y la prueba STA de interfaz siguen pendientes de ejecución; no se confunden con las comprobaciones manuales. La portada académica y la publicación GitHub siguen pendientes. El ejecutable no está firmado y puede quedar bloqueado por las políticas de Windows. No se incluye ninguna instrucción para desactivar esas políticas.

## GitHub

Se entrega historial Git local, sin publicación remota. Véase [GIT_Y_GITHUB.md](Documentacion/GIT_Y_GITHUB.md). La cuenta y el repositorio remoto deben ser elegidos por el equipo; no se ha publicado contenido en su nombre.
