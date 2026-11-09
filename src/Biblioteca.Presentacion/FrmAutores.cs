using Biblioteca.Entidades;
namespace Biblioteca.Presentacion;
public partial class FrmAutores:FrmCrud
{
 public FrmAutores():base("Autores"){InitializeComponent();}
 protected override void Listar(){var t=Estilo.Tabla([("Id",typeof(int)),("Nombre",typeof(string)),("Apellido",typeof(string))]);foreach(var fila in datos.Autores.Where(x=>Coincide(x.NombreCompleto)).Select(x=>new object[]{x.Id,x.Nombre,x.Apellido}))t.Rows.Add(fila);Mostrar(t);}
 protected override void CargarSeleccion(){var x=datos.Autores.Single(x=>x.Id==seleccionado);P("Nombre",x.Nombre);P("Apellido",x.Apellido);}
 protected override void GuardarRegistro()=>Contexto.Servicio.Guardar(new Autor(T("Nombre"),T("Apellido")){Id=seleccionado});
 protected override void EliminarRegistro()=>Contexto.Servicio.Eliminar<Autor>(seleccionado);
}
