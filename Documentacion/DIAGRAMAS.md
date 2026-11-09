# Diagramas de implementación

## Entidades del catálogo

```mermaid
classDiagram
 direction TB
 Registro <|-- Autor
 Registro <|-- Editorial
 Registro <|-- Libro
 class Registro {
  +int Id
 }
 class Autor {
  +string Nombre
  +string Apellido
  +string NombreCompleto
  +Autor()
  +Autor(nombre, apellido)
 }
 class Editorial {
  +string Nombre
  +Editorial()
  +Editorial(nombre)
 }
 class Libro {
  +string Codigo
  +string ISBN
  +string Titulo
  +int IdAutor
  +int IdEditorial
  +string Categoria
  +int Anio
  +int Existencias
 }
```

## Usuarios y circulación

```mermaid
classDiagram
 direction TB
 Registro <|-- Usuario
 Registro <|-- Prestamo
 class Registro {
  +int Id
 }
 class Usuario {
  +string Nombre
  +string Apellido
  +string Documento
  +string Telefono
  +string Correo
  +string NombreCompleto
 }
 class Prestamo {
  +int IdUsuario
  +int IdLibro
  +DateTime FechaPrestamo
  +DateTime FechaLimite
  +DateTime? FechaDevolucion
  +string Estado
  +bool Vencido
 }
```

Los constructores por defecto implícitos de Libro, Usuario y Prestamo inicializan propiedades con los valores definidos en Modelos.cs. NombreCompleto, Estado y Vencido son propiedades de solo lectura calculadas.

## Reglas y almacenamiento

```mermaid
classDiagram
 direction TB
 BibliotecaServicio --> IAlmacen
 IAlmacen <|.. AlmacenSql
 IAlmacen <|.. AlmacenJson
 IAlmacen --> EstadoBiblioteca
 class BibliotecaServicio {
  +Consultar() EstadoBiblioteca
  +Guardar(registro) int
  +Eliminar(id) void
  +Devolver(id, fecha) void
  +NormalizarIsbn(valor) string
  +IsbnValido(valor) bool
  +Validar(estado) void
 }
 class IAlmacen {
  <<interface>>
  +string Descripcion
  +Leer() EstadoBiblioteca
  +Ejecutar(operacion) void
 }
 class AlmacenSql {
  +AlmacenSql(cadena)
  +Leer() EstadoBiblioteca
  +Ejecutar(operacion) void
 }
 class AlmacenJson {
  +AlmacenJson(rutaArchivo)
  +Leer() EstadoBiblioteca
  +Ejecutar(operacion) void
 }
 class EstadoBiblioteca {
  +List~Autor~ Autores
  +List~Editorial~ Editoriales
  +List~Libro~ Libros
  +List~Usuario~ Usuarios
  +List~Prestamo~ Prestamos
  +Disponibles(idLibro) int
 }
```

Guardar y Eliminar son métodos genéricos restringidos a Registro. Ejecutar recibe una Action de EstadoBiblioteca. Se abrevia la firma para legibilidad del diagrama; el código es la definición completa.

## Modelo relacional

```mermaid
erDiagram
 Autores ||--o{ Libros : escribe
 Editoriales ||--o{ Libros : publica
 Libros ||--o{ Prestamos : circula
 Usuarios ||--o{ Prestamos : solicita
 Autores {
  int Id PK
  nvarchar Nombre
  nvarchar Apellido
 }
 Editoriales {
  int Id PK
  nvarchar Nombre UK
 }
 Libros {
  int Id PK
  nvarchar Codigo UK
  varchar ISBN UK
  nvarchar Titulo
  int IdAutor FK
  int IdEditorial FK
  nvarchar Categoria
  int Anio
  int Existencias
 }
 Usuarios {
  int Id PK
  nvarchar Nombre
  nvarchar Apellido
  nvarchar Documento UK
  nvarchar Telefono
  nvarchar Correo
 }
 Prestamos {
  int Id PK
  int IdUsuario FK
  int IdLibro FK
  date FechaPrestamo
  date FechaLimite
  date FechaDevolucion
 }
```

Estado es una columna computada de Prestamos. FechaDevolucion admite NULL. Ver las longitudes, índices y restricciones en SQL/01_Crear_Biblioteca.sql.
