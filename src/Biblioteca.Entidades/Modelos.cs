namespace Biblioteca.Entidades;

public abstract class Registro
{
    public int Id { get; set; }
    protected Registro() { }
    protected Registro(int id) => Id = id;
}
public sealed class Autor : Registro
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    public Autor() { }
    public Autor(string nombre, string apellido) { Nombre = nombre; Apellido = apellido; }
}
public sealed class Editorial : Registro
{
    public string Nombre { get; set; } = "";
    public Editorial() { }
    public Editorial(string nombre) => Nombre = nombre;
}
public sealed class Libro : Registro
{
    public string Codigo { get; set; } = "";
    public string ISBN { get; set; } = "";
    public string Titulo { get; set; } = "";
    public int IdAutor { get; set; }
    public int IdEditorial { get; set; }
    public string Categoria { get; set; } = "";
    public int Anio { get; set; } = DateTime.Today.Year;
    public int Existencias { get; set; }
}
public sealed class Usuario : Registro
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Correo { get; set; } = "";
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
}
public sealed class Prestamo : Registro
{
    public int IdUsuario { get; set; }
    public int IdLibro { get; set; }
    public DateTime FechaPrestamo { get; set; } = DateTime.Today;
    public DateTime FechaLimite { get; set; } = DateTime.Today.AddDays(14);
    public DateTime? FechaDevolucion { get; set; }
    public string Estado => FechaDevolucion.HasValue ? "Devuelto" : "Prestado";
    public bool Vencido => !FechaDevolucion.HasValue && FechaLimite.Date < DateTime.Today;
}
public sealed class EstadoBiblioteca
{
    public List<Autor> Autores { get; set; } = [];
    public List<Editorial> Editoriales { get; set; } = [];
    public List<Libro> Libros { get; set; } = [];
    public List<Usuario> Usuarios { get; set; } = [];
    public List<Prestamo> Prestamos { get; set; } = [];
    public int Disponibles(int idLibro) => Libros.Single(x => x.Id == idLibro).Existencias
        - Prestamos.Count(x => x.IdLibro == idLibro && !x.FechaDevolucion.HasValue);
}
public sealed class ReglaException(string mensaje) : Exception(mensaje);
