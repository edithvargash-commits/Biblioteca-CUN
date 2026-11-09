using System.Data;
using Biblioteca.Entidades;
using Microsoft.Data.SqlClient;
namespace Biblioteca.Datos;

public sealed class AlmacenSql(string cadena) : IAlmacen
{
    public string Descripcion => "SQL Server · BibliotecaCUN";
    private SqlConnection Abrir() { var c = new SqlConnection(cadena); c.Open(); return c; }
    public EstadoBiblioteca Leer()
    {
        using var c = Abrir();
        using var t = c.BeginTransaction(IsolationLevel.Serializable);
        Bloquear(c, t, "Shared");
        var e = Cargar(c, t);
        t.Commit(); return e;
    }
    public void Ejecutar(Action<EstadoBiblioteca> operacion)
    {
        using var c = Abrir();
        using var t = c.BeginTransaction(IsolationLevel.Serializable);
        Bloquear(c, t, "Exclusive");
        var antes = Cargar(c, t);
        // Copia independiente: conserva el estado previo para sincronizar diferencias.
        var despues = System.Text.Json.JsonSerializer.Deserialize<EstadoBiblioteca>(System.Text.Json.JsonSerializer.Serialize(antes))!;
        operacion(despues);
        Sincronizar(c, t, antes, despues);
        t.Commit(); // Dispose revierte la transaccion si ocurre un error antes del commit.
    }
    private static void Bloquear(SqlConnection c, SqlTransaction t, string modo)
    {
        using var q = new SqlCommand("DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource=N'BibliotecaCUN.Escritura',@LockMode=@modo,@LockOwner='Transaction',@LockTimeout=10000; IF @r<0 THROW 51000,N'La biblioteca está ocupada. Intente nuevamente.',1;", c, t);
        q.Parameters.Add("@modo",SqlDbType.NVarChar,32).Value=modo; q.ExecuteNonQuery();
    }
    private static EstadoBiblioteca Cargar(SqlConnection c, SqlTransaction t)
    {
        var e = new EstadoBiblioteca();
        using var q = new SqlCommand("SELECT Id,Nombre,Apellido FROM dbo.Autores ORDER BY Id; SELECT Id,Nombre FROM dbo.Editoriales ORDER BY Id; SELECT Id,Codigo,ISBN,Titulo,IdAutor,IdEditorial,Categoria,Anio,Existencias FROM dbo.Libros ORDER BY Id; SELECT Id,Nombre,Apellido,Documento,Telefono,Correo FROM dbo.Usuarios ORDER BY Id; SELECT Id,IdUsuario,IdLibro,FechaPrestamo,FechaLimite,FechaDevolucion FROM dbo.Prestamos ORDER BY Id;", c, t);
        using var r = q.ExecuteReader();
        while(r.Read()) e.Autores.Add(new Autor(r.GetString(1),r.GetString(2)){Id=r.GetInt32(0)});
        r.NextResult(); while(r.Read()) e.Editoriales.Add(new Editorial(r.GetString(1)){Id=r.GetInt32(0)});
        r.NextResult(); while(r.Read()) e.Libros.Add(new Libro {Id=r.GetInt32(0),Codigo=r.GetString(1),ISBN=r.GetString(2),Titulo=r.GetString(3),IdAutor=r.GetInt32(4),IdEditorial=r.GetInt32(5),Categoria=r.GetString(6),Anio=r.GetInt32(7),Existencias=r.GetInt32(8)});
        r.NextResult(); while(r.Read()) e.Usuarios.Add(new Usuario {Id=r.GetInt32(0),Nombre=r.GetString(1),Apellido=r.GetString(2),Documento=r.GetString(3),Telefono=r.GetString(4),Correo=r.GetString(5)});
        r.NextResult(); while(r.Read()) e.Prestamos.Add(new Prestamo {Id=r.GetInt32(0),IdUsuario=r.GetInt32(1),IdLibro=r.GetInt32(2),FechaPrestamo=r.GetDateTime(3),FechaLimite=r.GetDateTime(4),FechaDevolucion=r.IsDBNull(5)?null:r.GetDateTime(5)});
        return e;
    }
    private static void Borrar<T>(SqlConnection c, SqlTransaction t, string tabla, List<T> antes, List<T> despues) where T:Registro
    {
        // tabla proviene exclusivamente de nombres constantes internos.
        foreach(var x in antes.Where(x=>!despues.Any(y=>y.Id==x.Id)))
        {
            using var q=new SqlCommand($"DELETE FROM dbo.{tabla} WHERE Id=@Id",c,t);
            q.Parameters.Add("@Id",SqlDbType.Int).Value=x.Id;q.ExecuteNonQuery();
        }
    }
    private static void Guardar<T>(SqlConnection c, SqlTransaction t, string tabla, List<T> antes, List<T> despues, params (string Campo, SqlDbType Tipo, int Tamano)[] columnas) where T:Registro
    {
        foreach(var x in despues)
        {
            var previo=antes.SingleOrDefault(y=>y.Id==x.Id);
            if(previo is not null && System.Text.Json.JsonSerializer.Serialize(previo)==System.Text.Json.JsonSerializer.Serialize(x)) continue;
            string sql=previo is null
                ? $"INSERT INTO dbo.{tabla}(Id,{string.Join(',',columnas.Select(a=>a.Campo))}) VALUES(@Id,{string.Join(',',columnas.Select(a=>'@'+a.Campo))})"
                : $"UPDATE dbo.{tabla} SET {string.Join(',',columnas.Select(a=>a.Campo+'='+'@'+a.Campo))} WHERE Id=@Id";
            using var q=new SqlCommand(sql,c,t);
            q.Parameters.Add("@Id",SqlDbType.Int).Value=x.Id;
            foreach(var a in columnas)
            {
                var p=q.Parameters.Add('@'+a.Campo,a.Tipo);
                if(a.Tamano>0)p.Size=a.Tamano;
                p.Value=typeof(T).GetProperty(a.Campo)!.GetValue(x)??DBNull.Value;
            }
            q.ExecuteNonQuery();
        }
    }
    private static void Sincronizar(SqlConnection c, SqlTransaction t, EstadoBiblioteca a, EstadoBiblioteca d)
    {
        Borrar(c,t,"Prestamos",a.Prestamos,d.Prestamos); Borrar(c,t,"Libros",a.Libros,d.Libros);
        Borrar(c,t,"Usuarios",a.Usuarios,d.Usuarios); Borrar(c,t,"Autores",a.Autores,d.Autores); Borrar(c,t,"Editoriales",a.Editoriales,d.Editoriales);
        Guardar(c,t,"Autores",a.Autores,d.Autores,("Nombre",SqlDbType.NVarChar,100),("Apellido",SqlDbType.NVarChar,100));
        Guardar(c,t,"Editoriales",a.Editoriales,d.Editoriales,("Nombre",SqlDbType.NVarChar,150));
        Guardar(c,t,"Usuarios",a.Usuarios,d.Usuarios,("Nombre",SqlDbType.NVarChar,100),("Apellido",SqlDbType.NVarChar,100),("Documento",SqlDbType.NVarChar,30),("Telefono",SqlDbType.NVarChar,30),("Correo",SqlDbType.NVarChar,150));
        Guardar(c,t,"Libros",a.Libros,d.Libros,("Codigo",SqlDbType.NVarChar,30),("ISBN",SqlDbType.VarChar,13),("Titulo",SqlDbType.NVarChar,200),("IdAutor",SqlDbType.Int,0),("IdEditorial",SqlDbType.Int,0),("Categoria",SqlDbType.NVarChar,100),("Anio",SqlDbType.Int,0),("Existencias",SqlDbType.Int,0));
        Guardar(c,t,"Prestamos",a.Prestamos,d.Prestamos,("IdUsuario",SqlDbType.Int,0),("IdLibro",SqlDbType.Int,0),("FechaPrestamo",SqlDbType.Date,0),("FechaLimite",SqlDbType.Date,0),("FechaDevolucion",SqlDbType.Date,0));
    }
}
