# Estado de verificación de la entrega

Fecha de actualización: 9 de noviembre de 2026.

## Comprobaciones automatizadas en el entorno de construcción

- Solución compilada en Release; edición final publicada para Windows x64.
- 23 pruebas de negocio y persistencia JSON aprobadas; salida en resultado_pruebas.txt.
- ZIP comprobado por CRC y extracción completa con comparación SHA-256.
- Siete capturas copiadas sin modificar sus bytes y enlaces relativos comprobados.
- Prueba de regresión Windows Forms incorporada y compilada: Biblioteca.PruebasInterfaz. No se declara ejecutada.
- Prueba SQL optativa incorporada y compilada. No se declara ejecutada.

## Comprobaciones en el equipo del usuario

| Comprobación | Evidencia y alcance |
|---|---|
| Inicio de la aplicación y estilo aprobado | Captura actual 01_Inicio.png |
| Conexión con SQL Server | Confirmación del usuario; pie SQL Server · BibliotecaCUN en capturas |
| Catálogo, autores, editoriales y usuarios | Capturas 02 a 05 |
| Registrar préstamo y devolución | Capturas anteriores de confirmación y captura actual 06 |
| Restituir disponibilidad tras devolver | Captura actual 07: Cien años de soledad tiene 5 disponibles y 0 activos |
| Persistencia al cerrar y reabrir | Captura anterior del historial conservado tras reinicio |
| Exportación CSV | Captura anterior del reporte abierto en Excel |
| CRUD de autores, editoriales, usuarios y libros | El usuario informó la ejecución correcta; estas capturas de consulta no sustituyen evidencia individual de cada operación |

Las capturas actuales corresponden al diseño aprobado. Las imágenes recreadas para presentar propuestas no se utilizan como evidencia técnica.

## Pendientes para la entrega académica

- Completar portada cuando el usuario confirme los datos y si presentará individualmente o en grupo.
- Publicar en la cuenta y repositorio GitHub que el usuario elija.
- Ejecutar las pruebas adicionales SQL y STA si se requiere evidencia automatizada de integración/interfaz.
- Revisar el documento en el visor Markdown empleado para la presentación. La numeración editorial no implica paginación física fija.

## Límites conocidos

El ejecutable no está firmado digitalmente y requiere .NET Desktop Runtime 8 x64. No se implementan autenticación de operador, auditoría ni migración de datos previos. El repositorio SQL lee un estado completo y serializa operaciones de esta aplicación: está orientado a bajo volumen académico. Las escrituras externas que no usen el mismo bloqueo no están cubiertas por la regla agregada de disponibilidad. Las ediciones no usan rowversion. La eliminación confirmada de un préstamo borra su historial. Estas limitaciones no impiden las funciones comprobadas, pero acotan su uso institucional.
