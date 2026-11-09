using System.Net.Mail;
using System.Text.Json;
using Biblioteca.Datos;
using Biblioteca.Entidades;
namespace Biblioteca.Negocio;

public sealed class BibliotecaServicio(IAlmacen almacen)
{
    public string Descripcion => almacen.Descripcion;
    public EstadoBiblioteca Consultar() => almacen.Leer();
    private static T Copiar<T>(T valor) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(valor))!;
    private static List<T> Lista<T>(EstadoBiblioteca e) where T:Registro => typeof(T).Name switch
    {
        nameof(Autor)=>(List<T>)(object)e.Autores,
        nameof(Editorial)=>(List<T>)(object)e.Editoriales,
        nameof(Libro)=>(List<T>)(object)e.Libros,
        nameof(Usuario)=>(List<T>)(object)e.Usuarios,
        nameof(Prestamo)=>(List<T>)(object)e.Prestamos,
        _=>throw new NotSupportedException()
    };
    public int Guardar<T>(T registro) where T:Registro
    {
        var nuevo=Copiar(registro);int id=0;
        almacen.Ejecutar(e=>
        {
            var lista=Lista<T>(e);
            if(nuevo.Id==0) { nuevo.Id=lista.Select(x=>x.Id).DefaultIfEmpty().Max()+1;lista.Add(nuevo); }
            else
            {
                int i=lista.FindIndex(x=>x.Id==nuevo.Id);
                if(i<0)throw new ReglaException("El registro ya no existe. Actualice la lista.");
                if(lista[i] is Prestamo previo && previo.FechaDevolucion.HasValue)
                    throw new ReglaException("Un préstamo devuelto no se puede editar.");
                lista[i]=nuevo;
            }
            Validar(e); id=nuevo.Id;
        });return id;
    }
    public void Eliminar<T>(int id) where T:Registro => almacen.Ejecutar(e=>
    {
        if(typeof(T)==typeof(Autor)&&e.Libros.Any(x=>x.IdAutor==id))throw new ReglaException("El autor está relacionado con libros. No puede eliminarse.");
        if(typeof(T)==typeof(Editorial)&&e.Libros.Any(x=>x.IdEditorial==id))throw new ReglaException("La editorial está relacionada con libros. No puede eliminarse.");
        if(typeof(T)==typeof(Libro)&&e.Prestamos.Any(x=>x.IdLibro==id))throw new ReglaException("El libro tiene historial de préstamos. No puede eliminarse.");
        if(typeof(T)==typeof(Usuario)&&e.Prestamos.Any(x=>x.IdUsuario==id))throw new ReglaException("El usuario tiene historial de préstamos. No puede eliminarse.");
        var lista=Lista<T>(e);if(lista.RemoveAll(x=>x.Id==id)==0)throw new ReglaException("El registro ya no existe.");
        Validar(e);
    });
    public void Devolver(int id,DateTime fecha) => almacen.Ejecutar(e=>
    {
        var p=e.Prestamos.SingleOrDefault(x=>x.Id==id)??throw new ReglaException("No se encontró el préstamo.");
        if(p.FechaDevolucion.HasValue)throw new ReglaException("La devolución ya fue registrada.");
        p.FechaDevolucion=fecha.Date;Validar(e);
    });
    public static string NormalizarIsbn(string valor)=>valor.Replace("-","").Replace(" ","").ToUpperInvariant();
    public static bool IsbnValido(string valor)
    {
        var s=NormalizarIsbn(valor);
        if(s.Length==13 && s.All(char.IsAsciiDigit))
            return s.Select((c,i)=>(c-'0')*(i%2==0?1:3)).Sum()%10==0;
        if(s.Length==10 && s[..9].All(char.IsAsciiDigit) && (char.IsAsciiDigit(s[9])||s[9]=='X'))
            return s.Select((c,i)=>(c=='X'?10:c-'0')*(10-i)).Sum()%11==0;
        return false;
    }
    private static string Texto(string? s,string campo,int max,bool obligatorio=true)
    {
        s=(s??"").Trim();
        if(obligatorio&&s.Length==0)throw new ReglaException($"El campo {campo} es obligatorio.");
        if(s.Length>max)throw new ReglaException($"{campo} admite máximo {max} caracteres.");
        return s;
    }
    private static void Unicos(IEnumerable<string> valores,string campo)
    {
        if(valores.GroupBy(x=>x,StringComparer.OrdinalIgnoreCase).Any(g=>g.Count()>1))
            throw new ReglaException($"Ya existe un registro con el mismo {campo}.");
    }
    public static void Validar(EstadoBiblioteca e)
    {
        foreach(var a in e.Autores){a.Nombre=Texto(a.Nombre,"nombre del autor",100);a.Apellido=Texto(a.Apellido,"apellido del autor",100);}
        foreach(var x in e.Editoriales)x.Nombre=Texto(x.Nombre,"editorial",150);
        foreach(var u in e.Usuarios)
        {
            u.Nombre=Texto(u.Nombre,"nombre",100);u.Apellido=Texto(u.Apellido,"apellido",100);u.Documento=Texto(u.Documento,"documento",30);
            u.Telefono=Texto(u.Telefono,"teléfono",30,false);u.Correo=Texto(u.Correo,"correo",150,false);
            if(u.Correo.Length>0&&(!MailAddress.TryCreate(u.Correo,out var m)||m.Address!=u.Correo||!m.Host.Contains('.')))
                throw new ReglaException("El correo electrónico no es válido.");
        }
        foreach(var l in e.Libros)
        {
            l.Codigo=Texto(l.Codigo,"código",30);l.Titulo=Texto(l.Titulo,"título",200);l.Categoria=Texto(l.Categoria,"categoría",100,false);
            l.ISBN=NormalizarIsbn(Texto(l.ISBN,"ISBN",25));
            if(!IsbnValido(l.ISBN))throw new ReglaException("Ingrese un ISBN de 10 o 13 caracteres con dígito de control válido.");
            if(!e.Autores.Any(x=>x.Id==l.IdAutor)||!e.Editoriales.Any(x=>x.Id==l.IdEditorial))throw new ReglaException("Seleccione un autor y una editorial existentes.");
            if(l.Anio<1||l.Anio>DateTime.Today.Year+1)throw new ReglaException("El año debe estar entre 1 y el próximo año.");
            if(l.Existencias<0||l.Existencias>1000000)throw new ReglaException("Las existencias deben estar entre 0 y 1.000.000.");
        }
        Unicos(e.Libros.Select(x=>x.Codigo),"código");Unicos(e.Libros.Select(x=>x.ISBN),"ISBN");Unicos(e.Usuarios.Select(x=>x.Documento),"documento");Unicos(e.Editoriales.Select(x=>x.Nombre),"nombre de editorial");
        foreach(var p in e.Prestamos)
        {
            if(!e.Usuarios.Any(x=>x.Id==p.IdUsuario)||!e.Libros.Any(x=>x.Id==p.IdLibro))throw new ReglaException("Seleccione un usuario y un libro existentes.");
            if(p.FechaPrestamo.Date>DateTime.Today)throw new ReglaException("La fecha del préstamo no puede ser futura.");
            if(p.FechaLimite.Date<p.FechaPrestamo.Date)throw new ReglaException("La fecha límite debe ser igual o posterior al préstamo.");
            if(p.FechaDevolucion.HasValue&&(p.FechaDevolucion.Value.Date<p.FechaPrestamo.Date||p.FechaDevolucion.Value.Date>DateTime.Today))throw new ReglaException("La devolución debe estar entre la fecha de préstamo y hoy.");
        }
        foreach(var l in e.Libros)if(e.Disponibles(l.Id)<0)throw new ReglaException($"No hay ejemplares disponibles de «{l.Titulo}». Las existencias tampoco pueden ser menores que los préstamos activos.");
        if(e.Prestamos.Where(x=>!x.FechaDevolucion.HasValue).GroupBy(x=>(x.IdUsuario,x.IdLibro)).Any(g=>g.Count()>1))throw new ReglaException("El usuario ya tiene un préstamo activo de este libro.");
    }
}
