using System.Text.Json;
using Biblioteca.Entidades;
namespace Biblioteca.Datos;

// Implementacion de demostracion. La interfaz permite intercambiar JSON y SQL Server.
public sealed class AlmacenJson : IAlmacen
{
    private readonly string ruta;
    private readonly string bloqueo;
    public string Descripcion => "Demostración local (JSON)";
    public AlmacenJson(string rutaArchivo)
    {
        ruta = Path.GetFullPath(rutaArchivo);
        Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
        bloqueo = ruta + ".lock";
    }
    private FileStream Bloquear()
    {
        for (int i = 0; ; i++)
        {
            try { return new FileStream(bloqueo, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); }
            catch (IOException) when (i < 30) { Thread.Sleep(100); }
        }
    }
    private EstadoBiblioteca Cargar() => File.Exists(ruta)
        ? JsonSerializer.Deserialize<EstadoBiblioteca>(File.ReadAllText(ruta))
          ?? throw new InvalidDataException("El archivo de demostración está vacío o es inválido.")
        : DatosEjemplo.Crear();
    public EstadoBiblioteca Leer() { using var l = Bloquear(); return Cargar(); }
    public void Ejecutar(Action<EstadoBiblioteca> operacion)
    {
        using var l = Bloquear();
        var estado = Cargar();
        operacion(estado); // Ante una excepcion, no se cambia el archivo.
        string temporal = ruta + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temporal, JsonSerializer.Serialize(estado, new JsonSerializerOptions { WriteIndented = true }));
            File.Move(temporal, ruta, true);
        }
        finally { if (File.Exists(temporal)) File.Delete(temporal); }
    }
}
public static class DatosEjemplo
{
    public static EstadoBiblioteca Crear() => new()
    {
        Autores = [new Autor("Gabriel", "García Márquez") { Id=1 }, new Autor("George", "Orwell") { Id=2 }, new Autor("Antoine", "de Saint-Exupéry") { Id=3 }],
        Editoriales = [new Editorial("Editorial de práctica A") { Id=1 }, new Editorial("Editorial de práctica B") { Id=2 }],
        Libros = [
            new Libro { Id=1, Codigo="LIB-001", ISBN="9780307474728", Titulo="Cien años de soledad", IdAutor=1, IdEditorial=1, Categoria="Novela", Anio=1967, Existencias=5 },
            new Libro { Id=2, Codigo="LIB-002", ISBN="9780451524935", Titulo="1984", IdAutor=2, IdEditorial=2, Categoria="Distopía", Anio=1949, Existencias=3 },
            new Libro { Id=3, Codigo="LIB-003", ISBN="9780156012195", Titulo="El principito", IdAutor=3, IdEditorial=1, Categoria="Literatura", Anio=1943, Existencias=4 }
        ],
        Usuarios = [new Usuario { Id=1, Nombre="Ana", Apellido="Ejemplo", Documento="DEMO-001", Correo="ana@example.com", Telefono="3000000000" }]
    };
}
