using Biblioteca.Entidades;
namespace Biblioteca.Presentacion;
public partial class FrmLibros:FrmCrud
{
    public FrmLibros():base("Libros"){InitializeComponent();buscar.PlaceholderText="Código, ISBN, título o autor…";}
    protected override void CargarOpciones()
    {
        Opciones("Autor",datos.Autores.Select(x=>new Opcion(x.Id,x.NombreCompleto)));
        Opciones("Editorial",datos.Editoriales.Select(x=>new Opcion(x.Id,x.Nombre)));
    }
    protected override void Limpiar(){base.Limpiar();P("Anio",DateTime.Today.Year);}
    protected override void Listar()
    {
        var t=Estilo.Tabla([("Id",typeof(int)),("Código",typeof(string)),("ISBN",typeof(string)),("Título",typeof(string)),("Autor",typeof(string)),("Editorial",typeof(string)),("Categoría",typeof(string)),("Año",typeof(int)),("Totales",typeof(int)),("Disponibles",typeof(int))]);
        foreach(var l in datos.Libros)
        {
            string autor=datos.Autores.Single(x=>x.Id==l.IdAutor).NombreCompleto;
            if(Coincide(l.Codigo,l.ISBN,l.Titulo,autor))t.Rows.Add(l.Id,l.Codigo,l.ISBN,l.Titulo,autor,datos.Editoriales.Single(x=>x.Id==l.IdEditorial).Nombre,l.Categoria,l.Anio,l.Existencias,datos.Disponibles(l.Id));
        }Mostrar(t);
    }
    protected override void CargarSeleccion()
    {
        var l=datos.Libros.Single(x=>x.Id==seleccionado);
        P("Codigo",l.Codigo);P("ISBN",l.ISBN);P("Titulo",l.Titulo);P("Autor",l.IdAutor);P("Editorial",l.IdEditorial);P("Categoria",l.Categoria);P("Anio",l.Anio);P("Existencias",l.Existencias);
    }
    protected override void GuardarRegistro()=>Contexto.Servicio.Guardar(new Libro{Id=seleccionado,Codigo=T("Codigo"),ISBN=T("ISBN"),Titulo=T("Titulo"),IdAutor=C("Autor"),IdEditorial=C("Editorial"),Categoria=T("Categoria"),Anio=N("Anio"),Existencias=N("Existencias")});
    protected override void EliminarRegistro()=>Contexto.Servicio.Eliminar<Libro>(seleccionado);
}
