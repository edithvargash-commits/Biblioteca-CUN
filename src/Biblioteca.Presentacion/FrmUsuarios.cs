using Biblioteca.Entidades;
namespace Biblioteca.Presentacion;
public partial class FrmUsuarios:FrmCrud
{
 public FrmUsuarios():base("Usuarios"){InitializeComponent();}
 protected override void Listar(){var t=Estilo.Tabla([("Id",typeof(int)),("Nombre",typeof(string)),("Apellido",typeof(string)),("Documento",typeof(string)),("Teléfono",typeof(string)),("Correo electrónico",typeof(string))]);foreach(var fila in datos.Usuarios.Where(x=>Coincide(x.NombreCompleto,x.Documento,x.Correo)).Select(x=>new object[]{x.Id,x.Nombre,x.Apellido,x.Documento,x.Telefono,x.Correo}))t.Rows.Add(fila);Mostrar(t);}
 protected override void CargarSeleccion(){var x=datos.Usuarios.Single(x=>x.Id==seleccionado);P("Nombre",x.Nombre);P("Apellido",x.Apellido);P("Documento",x.Documento);P("Telefono",x.Telefono);P("Correo",x.Correo);}
 protected override void GuardarRegistro()=>Contexto.Servicio.Guardar(new Usuario{Id=seleccionado,Nombre=T("Nombre"),Apellido=T("Apellido"),Documento=T("Documento"),Telefono=T("Telefono"),Correo=T("Correo")});
 protected override void EliminarRegistro()=>Contexto.Servicio.Eliminar<Usuario>(seleccionado);
}
