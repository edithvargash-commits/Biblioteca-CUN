using Biblioteca.Entidades;
namespace Biblioteca.Presentacion;
public sealed class FrmPrestamos:FrmCrud
{
    public FrmPrestamos():base("Préstamos y devoluciones")
    {
        Combo("Usuario","Usuario *");Combo("Libro","Libro *");Fecha("Prestamo","Fecha de préstamo *");Fecha("Limite","Fecha límite *");Fecha("Devolucion","Fecha para registrar devolución");
        buscar.PlaceholderText="Usuario, libro o estado…";
    }
    protected override void AgregarAcciones(FlowLayoutPanel p)=>p.Controls.Add(Estilo.Boton("Registrar devolución",()=>Estilo.Ejecutar(()=>
    {
        if(seleccionado==0)throw new ReglaException("Seleccione el préstamo que desea devolver.");
        if(MessageBox.Show("¿Registrar la devolución con fecha "+F("Devolucion").ToString("dd/MM/yyyy")+"?","Devolución",MessageBoxButtons.YesNo)==DialogResult.Yes)
        {Contexto.Servicio.Devolver(seleccionado,F("Devolucion"));Recargar();MessageBox.Show("Devolución registrada.");}
    }),true));
    protected override void CargarOpciones()
    {
        Opciones("Usuario",datos.Usuarios.Select(x=>new Opcion(x.Id,x.NombreCompleto+" · "+x.Documento)));
        Opciones("Libro",datos.Libros.Select(x=>new Opcion(x.Id,x.Titulo+" · disponibles: "+datos.Disponibles(x.Id))));
    }
    protected override void Limpiar(){base.Limpiar();P("Limite",DateTime.Today.AddDays(14));}
    protected override void Listar()
    {
        var t=Estilo.Tabla([("Id",typeof(int)),("N.º",typeof(int)),("Usuario",typeof(string)),("Libro",typeof(string)),("Préstamo",typeof(DateTime)),("Límite",typeof(DateTime)),("Devolución",typeof(DateTime)),("Estado",typeof(string)),("Vencido",typeof(string))]);
        foreach(var x in datos.Prestamos)
        {
            string u=datos.Usuarios.Single(a=>a.Id==x.IdUsuario).NombreCompleto,l=datos.Libros.Single(a=>a.Id==x.IdLibro).Titulo;
            if(Coincide(u,l,x.Estado,x.Id.ToString()))t.Rows.Add(x.Id,x.Id,u,l,x.FechaPrestamo,x.FechaLimite,(object?)x.FechaDevolucion??DBNull.Value,x.Estado,x.Vencido?"Sí":"No");
        }Mostrar(t);
        foreach(DataGridViewColumn c in grilla.Columns)if(c.ValueType==typeof(DateTime))c.DefaultCellStyle.Format="dd/MM/yyyy";
    }
    protected override void CargarSeleccion()
    {var x=datos.Prestamos.Single(x=>x.Id==seleccionado);P("Usuario",x.IdUsuario);P("Libro",x.IdLibro);P("Prestamo",x.FechaPrestamo);P("Limite",x.FechaLimite);P("Devolucion",x.FechaDevolucion??DateTime.Today);}
    protected override void GuardarRegistro()=>Contexto.Servicio.Guardar(new Prestamo{Id=seleccionado,IdUsuario=C("Usuario"),IdLibro=C("Libro"),FechaPrestamo=F("Prestamo"),FechaLimite=F("Limite")});
    protected override void EliminarRegistro()=>Contexto.Servicio.Eliminar<Prestamo>(seleccionado);
}
