# Sistema de Gestión de Biblioteca

**Corporación Unificada Nacional de Educación Superior - CUN**  
**Asignatura:** Programación Avanzada  
**Docente:** Verónica Castro Munar  
**Ficha:** 54465  

**Estudiante:** Edith Yesenia Vargas Herrera  

**Lugar y fecha:** Bogotá D. C., 9 de noviembre de 2026  


**Página 1 - Portada**

<div style="page-break-after: always;"></div>

## Contraportada

## Contraportada

Sistema de Gestión de Biblioteca desarrollado como proyecto académico de la asignatura Programación Avanzada de la Corporación Unificada Nacional de Educación Superior — CUN.

La aplicación permite administrar libros, autores, editoriales, usuarios, préstamos y devoluciones. Está desarrollada en C#, Windows Forms y .NET 8, con almacenamiento en SQL Server y una estructura organizada en capas.

El documento presenta los requerimientos, la arquitectura, el modelo de datos, los módulos del sistema y las evidencias de las pruebas realizadas.

**Página 2 - Contraportada**

<div style="page-break-after: always;"></div>

## Tabla de contenido

1. Introducción, problema y objetivos - página 4.
2. Requerimientos y alcance - página 5.
3. Arquitectura y orientación a objetos - página 6.
4. Casos de uso y reglas - página 7.
5. Modelo de datos y diccionario - página 8.
6. Persistencia, SQL y transacciones - página 9.
7. Interfaz y módulos - página 10.
8. Verificación y evidencias - página 11.
9. Conclusiones, recomendaciones y referencias - página 12.



**Página 3 - Contenido**

<div style="page-break-after: always;"></div>

## 1. Introducción, problema y objetivos

La administración dispersa de libros y préstamos dificulta conocer la disponibilidad, asociar obras con sus autores y recuperar el historial de circulación. La solución centraliza esos registros y separa la captura de datos de sus validaciones y persistencia.

El objetivo general es desarrollar una aplicación de escritorio con C#, Windows Forms y SQL Server para gestionar los procesos de una biblioteca, aplicando programación orientada a objetos e integridad relacional.

Los objetivos específicos son: implementar CRUD de catálogos y usuarios; validar campos, unicidad y relaciones; registrar préstamos y devoluciones coherentes; consultar inventario e historial; organizar el código en capas; y aportar evidencia reproducible de compilación y pruebas.

El proyecto tiene un alcance académico y está orientado a la gestión de una biblioteca de pequeño volumen. Incluye la administración de libros, autores, editoriales y usuarios, el registro de préstamos y devoluciones, y la consulta de reportes. Cada préstamo vincula a un usuario con un ejemplar de un título. El sistema utiliza SQL Server como almacenamiento principal y dispone de un modo de demostración local con archivos JSON.

**Página 4 - Introducción**

<div style="page-break-after: always;"></div>

## 2. Requerimientos y alcance

| ID | Requerimiento implementado | Evidencia en código |
|---|---|---|
| RF01 | CRUD de libros, búsqueda y disponibilidad | FrmLibros; BibliotecaServicio |
| RF02 | CRUD autores y restricciones | FrmAutores; Eliminar<Autor> |
| RF03 | CRUD editoriales según descripción del ACA | FrmEditoriales |
| RF04 | CRUD usuarios, documento único y correo | FrmUsuarios; Validar |
| RF05 | Registro, consulta, edición activa y eliminación de préstamos | FrmPrestamos |
| RF06 | Devolución con estado y disponibilidad coherentes | Devolver; EstadoBiblioteca.Disponibles |
| RF07 | Inventario, catálogos, usuarios e historial, activos/devueltos/vencidos | FrmReportes |
| RF08 | Menú organizado | FrmPrincipal |

Los campos obligatorios se verifican en negocio antes de guardar. Los errores de regla se presentan como advertencias; las excepciones de acceso a datos generan un mensaje de diagnóstico. La capa de negocio no depende de Windows Forms. El SQL usa parámetros para los valores y nombres internos constantes para tablas y columnas. Se previenen documentos, códigos, ISBN y editoriales duplicados.

La exportación CSV representa el conjunto mostrado en pantalla. Los controles de fecha aplican a préstamos; no restringen el inventario ni los catálogos.

**Página 5 - Requerimientos**

<div style="page-break-after: always;"></div>

## 3. Arquitectura y orientación a objetos

```mermaid
flowchart TD
 P["Presentación: formularios"] --> N["Negocio: BibliotecaServicio"]
 N --> I["Contrato: IAlmacen"]
 I --> S["AlmacenSql"]
 I --> J["AlmacenJson"]
 S --> B["SQL Server: BibliotecaCUN"]
 J --> F["Archivo local de demostración"]
```

| Proyecto | Responsabilidad |
|---|---|
| Biblioteca.Entidades | Datos del dominio, propiedades y cálculos simples |
| Biblioteca.Datos | Leer estados y ejecutar cambios transaccionales |
| Biblioteca.Negocio | Validar y coordinar operaciones |
| Biblioteca.Presentacion | Capturar entradas, mostrar tablas y mensajes |
| Biblioteca.Pruebas | Comprobar invariantes y persistencia |

Las clases usan propiedades y constructores. `Registro` centraliza Id y las entidades heredan esa identidad. `Autor.NombreCompleto`, `Prestamo.Estado` y `EstadoBiblioteca.Disponibles` encapsulan valores calculados. `BibliotecaServicio` encapsula reglas y copia la entrada antes de modificar el estado persistente. Las colecciones tipadas representan cada tabla.

`IAlmacen` permite polimorfismo entre SQL y JSON sin cambiar el servicio. La interfaz es inyectada por constructor. `FrmCrud` reutiliza layout y acciones en formularios derivados. Las propiedades de datos son mutables para permitir serialización, carga y enlace; las operaciones de la aplicación pasan por el servicio para preservar las reglas.

Véase `DIAGRAMAS.md` para el diagrama de clases completo y los contratos de persistencia. Las propiedades persistidas se detallan en la página 8.

**Página 6 - Arquitectura**

<div style="page-break-after: always;"></div>

## 4. Casos de uso y reglas

| Caso | Actor y precondición | Flujo y resultado | Alternativa |
|---|---|---|---|
| CU01 Registrar libro | Operador; autor y editorial existentes | Completar datos, Guardar; aparece en catálogo | ISBN/código repetido o inválido: no guarda |
| CU02 Actualizar catálogo | Operador; fila existente | Seleccionar, editar y Guardar | Autor/editorial referenciados no se eliminan |
| CU03 Registrar usuario | Operador | Introducir identificación y datos; Guardar | Documento repetido o correo inválido: rechazo |
| CU04 Prestar | Operador; usuario/libro existentes | Elegir fechas; Guardar; baja disponibilidad | Stock cero o préstamo activo duplicado: rechazo |
| CU05 Devolver | Operador; préstamo activo | Seleccionar fecha y Registrar devolución | Fecha incoherente o devolución repetida: rechazo |
| CU06 Corregir préstamo | Operador; préstamo activo | Cambiar usuario/libro/fechas; Guardar | Devuelto: edición rechazada |
| CU07 Eliminar préstamo | Operador; fila existente | Confirmar eliminación; se borra historial | Cancelar conserva el registro |
| CU08 Reportar | Operador; datos disponibles | Tipo/rango, Consultar y Exportar CSV | Sin filas: se informa |
| CU09 Cambiar conexión | Operador; servidor instalado | Probar y guardar; activa SQL | Conexión fallida: conserva modo anterior |

Reglas centrales: ejemplares totales no negativos; disponibilidad nunca negativa después de operaciones de la aplicación; ISBN-10/13 con checksum; claves únicas; referencias existentes; fecha límite no anterior al préstamo; préstamo no futuro; devolución entre préstamo y hoy. Un usuario solo puede tener un préstamo activo por título. Se permiten préstamos posteriores tras devolver.

La disponibilidad se calcula, no se actualiza con incrementos/decrementos manuales. Esto evita sumar dos veces por una devolución repetida. El estado depende exclusivamente de la presencia de FechaDevolucion.

**Página 7 - Casos de uso**

<div style="page-break-after: always;"></div>

## 5. Modelo de datos y diccionario

```mermaid
erDiagram
 Autores ||--o{ Libros : escribe
 Editoriales ||--o{ Libros : publica
 Libros ||--o{ Prestamos : circula
 Usuarios ||--o{ Prestamos : solicita
```

Cada registro de préstamo corresponde a un título y un usuario; no se necesita DetallePrestamos. Las relaciones no usan borrado en cascada.

| Tabla | Campo | SQL | Restricción o significado |
|---|---|---|---|
| Todas | Id | int | PK, > 0; asignado por la aplicación dentro de la transacción |
| Autores | Nombre, Apellido | nvarchar(100) cada uno | Obligatorios |
| Editoriales | Nombre | nvarchar(150) | Obligatorio, único |
| Usuarios | Nombre, Apellido | nvarchar(100) cada uno | Obligatorios |
| Usuarios | Documento | nvarchar(30) | Obligatorio, único |
| Usuarios | Telefono | nvarchar(30) | Opcional, cadena vacía por defecto |
| Usuarios | Correo | nvarchar(150) | Opcional; formato validado por negocio |
| Libros | Codigo | nvarchar(30) | Obligatorio, único |
| Libros | ISBN | varchar(13) | Obligatorio, único; longitud 10/13 en SQL; checksum en negocio |
| Libros | Titulo | nvarchar(200) | Obligatorio |
| Libros | IdAutor | int | FK a Autores.Id |
| Libros | IdEditorial | int | FK a Editoriales.Id |
| Libros | Categoria | nvarchar(100) | Opcional, texto libre |
| Libros | Anio | int | SQL 1..9999; negocio hasta el año siguiente |
| Libros | Existencias | int | Total poseído; 0..1.000.000 |
| Prestamos | IdUsuario | int | FK a Usuarios.Id |
| Prestamos | IdLibro | int | FK a Libros.Id |
| Prestamos | FechaPrestamo | date | Obligatoria |
| Prestamos | FechaLimite | date | Igual/posterior al préstamo |
| Prestamos | FechaDevolucion | date NULL | Ausencia significa activo |
| Prestamos | Estado | Computado | Prestado/Devuelto; no se escribe directamente |

Hay un índice único filtrado sobre (IdUsuario, IdLibro) para préstamos sin devolución, e índices para relaciones y fechas. La vista `vInventario` expone totales, activos y disponibles. Los Id no son IDENTITY: se asignan con máximo + 1 bajo bloqueo transaccional; no modifique manualmente identificadores ni use este esquema como generador distribuido.

**Página 8 - Datos**

<div style="page-break-after: always;"></div>

## 6. Persistencia, SQL y transacciones

`AlmacenSql` abre una conexión por operación. La lectura obtiene un bloqueo compartido de aplicación y lee las cinco tablas en transacción Serializable. Una escritura obtiene el bloqueo exclusivo del mismo recurso, carga el estado vigente, aplica y valida el cambio, sincroniza solo filas cambiadas y confirma. Una excepción antes del commit revierte el cambio.

Los parámetros llevan SqlDbType explícito y longitud para textos. Los identificadores de tabla/columna se construyen desde constantes internas del repositorio, nunca desde la búsqueda ni desde entradas del usuario. Las cadenas de texto con comillas se envían como valores y no forman parte del código SQL.

El bloqueo `sp_getapplock` serializa las escrituras que usan esta aplicación y evita que dos solicitudes calculen la misma disponibilidad al mismo tiempo. El servicio revisa todo el estado después de cambiarlo. Las escrituras manuales o herramientas externas que omitan el contrato no quedan protegidas por ese bloqueo. Las FK, CHECK y restricciones UNIQUE siguen actuando en SQL, pero el checksum ISBN, fecha respecto a hoy y disponibilidad agregada se validan en negocio, no mediante triggers.

La lectura del estado completo simplifica una solución académica de bajo volumen; no es una estrategia adecuada para millones de registros. Para escalar se recomiendan repositorios por agregado, consultas paginadas y procedimientos transaccionales específicos. Las ediciones normales no incorporan rowversion: dos operadores que editen la misma fila conservarán el último guardado válido. El control de stock sí se evalúa con el estado transaccional vigente.

`AlmacenJson` usa un archivo de bloqueo exclusivo compartido entre instancias y reemplaza el archivo de datos mediante un temporal. Si la validación falla no escribe. La demostración no se replica a SQL.

**Página 9 - Persistencia**

<div style="page-break-after: always;"></div>

## 7. Interfaz y módulos

El formulario principal tiene menú lateral y área de contenido. La edición actual usa fondo marfil, navegación nude, acciones rosa profundo y tipografía Georgia en títulos con Segoe UI para datos y controles. Las tablas son de solo lectura; para editar se selecciona una fila y se usan los campos inferiores. Los listados y los catálogos se recargan con Actualizar.

| Módulo | Controles y operaciones |
|---|---|
| Inicio | Indicadores: títulos, disponibles, activos, vencidos, usuarios |
| Libros | TextBox, ComboBox autor/editorial, NumericUpDown año/totales, DataGridView |
| Autores | Nombre y apellido; CRUD y filtro |
| Editoriales | Nombre; CRUD y filtro |
| Usuarios | Identificación, nombres y contacto; CRUD y filtro |
| Préstamos | ComboBox usuario/libro; DateTimePicker; CRUD activo y devolución |
| Reportes | ComboBox de tipo, filtros de fecha, DataGridView y CSV |
| Conexión | Selección de almacenamiento, cadena y prueba previa a guardar |

### Capturas reales de funcionamiento

Las siguientes capturas documentan el funcionamiento del sistema en Windows con datos de prueba.

**Figura 1. Inicio y panel general.**

![Figura 1. Inicio y panel general](Evidencias/01_Inicio.png)

La pantalla inicial muestra 3 títulos, 12 ejemplares disponibles, ningún préstamo activo ni vencido y 1 lector registrado. El historial reciente conserva el préstamo devuelto de Ana Ejemplo. Se observa la conexión a BibliotecaCUN en SQL Server.

*Nota. Captura de funcionamiento del sistema con datos de prueba.*

**Figura 2. Catálogo de libros.**

![Figura 2. Catálogo de libros](Evidencias/02_Libros.png)

Se muestran tres títulos con código, ISBN, autor, editorial, categoría, año, ejemplares totales y disponibilidad. La captura documenta la consulta del catálogo y los campos del formulario; por sí sola no prueba una inserción o edición.

*Nota. Captura de funcionamiento del sistema con datos de prueba.*

**Figura 3. Gestión de usuarios.**

![Figura 3. Gestión de usuarios](Evidencias/03_Usuarios.png)

Se observa el usuario ficticio Ana Ejemplo, con documento DEMO-001 y correo de prueba ana@example.com. El módulo presenta búsqueda y campos para mantener los datos del lector.

*Nota. Captura de funcionamiento del sistema con datos de prueba.*

**Figura 4. Catálogo de autores.**

![Figura 4. Catálogo de autores](Evidencias/04_Autores.png)

La consulta presenta a Gabriel García Márquez, George Orwell y Antoine de Saint-Exupéry. Se incluyen campos de nombre y apellido y acciones de mantenimiento.

*Nota. Captura de funcionamiento del sistema con datos de prueba.*

**Figura 5. Catálogo de editoriales.**

![Figura 5. Catálogo de editoriales](Evidencias/05_Editoriales.png)

Se listan Editorial de práctica A y Editorial de práctica B. Son registros de demostración, no una afirmación bibliográfica sobre las ediciones de los libros.

*Nota. Captura de funcionamiento del sistema con datos de prueba.*

**Figura 6. Préstamos y devoluciones.**

![Figura 6. Préstamos y devoluciones](Evidencias/06_Prestamos_Devoluciones.png)

El registro número 1 relaciona a Ana Ejemplo con Cien años de soledad. Se observan préstamo del 09/11/2026, límite del 23/11/2026, devolución del 09/11/2026, estado Devuelto y vencido No.

*Nota. Captura de funcionamiento del sistema con datos de prueba.*

**Figura 7. Reporte de inventario.**

![Figura 7. Reporte de inventario](Evidencias/07_Reporte_Inventario.png)

El reporte consolida tres títulos: 5, 3 y 4 ejemplares totales respectivamente. Todos presentan cero préstamos activos y la misma cantidad disponible, consistente con la devolución registrada. El inventario suma 12 ejemplares disponibles.

*Nota. Captura de funcionamiento del sistema con datos de prueba.*

**Página 10 - Interfaz**

<div style="page-break-after: always;"></div>

## 8. Verificación y evidencias

La verificación del sistema incluyó compilación, pruebas automatizadas de negocio y persistencia JSON, y pruebas manuales de funcionamiento en Windows con SQL Server.

| Verificación | Estado |
|---|---|
| Compilación de cinco proyectos | Aprobada |
| Publicación para Windows x64 | Aprobada |
| 23 pruebas automatizadas de negocio y persistencia JSON | Aprobadas |
| Integridad y extracción del ZIP | Comprobadas al empaquetar |
| Funcionamiento de la interfaz en Windows | Comprobado mediante pruebas manuales y capturas |
| Conexión y operaciones con SQL Server | Comprobadas mediante pruebas manuales |
| Evidencias de funcionamiento | Siete capturas incorporadas |
| Publicación en GitHub | Repositorio público Biblioteca-CUN |

Las pruebas manuales permitieron comprobar las operaciones de registro, consulta, modificación y eliminación, así como los préstamos, las devoluciones y la actualización de los ejemplares disponibles.

**Página 11 - Verificación**

<div style="page-break-after: always;"></div>

## 9. Conclusiones, recomendaciones y referencias

Se recomienda realizar copias de seguridad periódicas de la base de datos y conservar el código fuente actualizado en el repositorio.


### Referencias

Microsoft. (s. f.). *Windows Forms for .NET documentation*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/desktop/winforms/?view=netdesktop-8.0

Microsoft. (s. f.). *Install, update, and deploy Microsoft.Data.SqlClient*. Microsoft Learn. https://learn.microsoft.com/en-us/sql/connect/ado-net/download-microsoft-sqlclient-data-provider

Microsoft. (s. f.). *sys.sp_getapplock (Transact-SQL)*. Microsoft Learn. https://learn.microsoft.com/en-us/sql/relational-databases/system-stored-procedures/sp-getapplock-transact-sql

Microsoft. (s. f.). *Smart App Control frequently asked questions*. Microsoft Support. https://support.microsoft.com/en-us/windows/security/threat-malware-protection/smart-app-control-frequently-asked-questions

*Programación Avanzada - ACA Final 2026*. (2026). [Guía de actividad académica].

**Página 12 - Cierre**
