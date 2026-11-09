using Biblioteca.Entidades;
namespace Biblioteca.Presentacion;
public partial class FrmEditoriales:FrmCrud
{
 public FrmEditoriales():base("Editoriales"){InitializeComponent();}
 protected override void Listar(){var t=Estilo.Tabla([("Id",typeof(int)),("Nombre de la editorial",typeof(string))]);foreach(var fila in datos.Editoriales.Where(x=>Coincide(x.Nombre)).Select(x=>new object[]{x.Id,x.Nombre}))t.Rows.Add(fila);Mostrar(t);}
 protected override void CargarSeleccion(){var x=datos.Editoriales.Single(x=>x.Id==seleccionado);P("Nombre",x.Nombre);}
 protected override void GuardarRegistro()=>Contexto.Servicio.Guardar(new Editorial(T("Nombre")){Id=seleccionado});
 protected override void EliminarRegistro()=>Contexto.Servicio.Eliminar<Editorial>(seleccionado);
}
