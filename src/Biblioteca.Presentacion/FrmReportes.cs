using System.Data;
namespace Biblioteca.Presentacion;
public sealed class FrmReportes:Form
{
    private readonly DataGridView grilla=Estilo.Grilla();
    private readonly ComboBox tipo=new(){Width=260,DropDownStyle=ComboBoxStyle.DropDownList};
    private readonly DateTimePicker desde=new(){Width=150,Format=DateTimePickerFormat.Short,Value=DateTime.Today.AddMonths(-1)};
    private readonly DateTimePicker hasta=new(){Width=150,Format=DateTimePickerFormat.Short};
    private readonly CheckBox filtrar=new(){Text="Filtrar fechas de préstamo",AutoSize=true};
    public FrmReportes()
    {
        Estilo.Formato(this);Text="Reportes";
        tipo.Items.AddRange(["Inventario de libros","Autores","Editoriales","Usuarios","Todos los préstamos","Préstamos activos","Préstamos devueltos","Préstamos vencidos"]);tipo.SelectedIndex=0;
        var p=new TableLayoutPanel{Dock=DockStyle.Fill,RowCount=3,ColumnCount=1,Padding=new Padding(0)};
        p.RowStyles.Add(new RowStyle(SizeType.Absolute,86));p.RowStyles.Add(new RowStyle(SizeType.Absolute,116));p.RowStyles.Add(new RowStyle(SizeType.Percent,100));
        p.Controls.Add(Estilo.Encabezado("Consultas y reportes","Explora tu inventario y el historial de circulación."),0,0);
        Estilo.Entrada(tipo);Estilo.Entrada(desde);Estilo.Entrada(hasta);
        var bar=new FlowLayoutPanel{Dock=DockStyle.Fill,AutoScroll=true};bar.Controls.AddRange([tipo,filtrar,desde,hasta,Estilo.Boton("Consultar",()=>Estilo.Ejecutar(Consultar),true),Estilo.Boton("Exportar CSV",()=>Estilo.Ejecutar(()=>Estilo.Exportar(grilla)))]);
        p.Controls.Add(bar,0,1);p.Controls.Add(Estilo.Superficie(grilla),0,2);Controls.Add(p);Shown+=(_,_)=>Estilo.Ejecutar(Consultar);
    }
    private void Consultar()
    {
        var e=Contexto.Servicio.Consultar();DataTable t;
        if(filtrar.Checked&&desde.Value.Date>hasta.Value.Date)throw new Biblioteca.Entidades.ReglaException("La fecha inicial no puede ser posterior a la final.");
        switch(tipo.SelectedIndex)
        {
            case 0:
                t=Estilo.Tabla([("Código",typeof(string)),("ISBN",typeof(string)),("Título",typeof(string)),("Autor",typeof(string)),("Editorial",typeof(string)),("Totales",typeof(int)),("Activos",typeof(int)),("Disponibles",typeof(int))]);
                foreach(var l in e.Libros)t.Rows.Add(l.Codigo,l.ISBN,l.Titulo,e.Autores.Single(x=>x.Id==l.IdAutor).NombreCompleto,e.Editoriales.Single(x=>x.Id==l.IdEditorial).Nombre,l.Existencias,l.Existencias-e.Disponibles(l.Id),e.Disponibles(l.Id));break;
            case 1:
                t=Estilo.Tabla([("Id",typeof(int)),("Nombre",typeof(string)),("Apellido",typeof(string))]);foreach(var a in e.Autores)t.Rows.Add(a.Id,a.Nombre,a.Apellido);break;
            case 2:
                t=Estilo.Tabla([("Id",typeof(int)),("Editorial",typeof(string))]);foreach(var a in e.Editoriales)t.Rows.Add(a.Id,a.Nombre);break;
            case 3:
                t=Estilo.Tabla([("Documento",typeof(string)),("Nombre",typeof(string)),("Teléfono",typeof(string)),("Correo",typeof(string))]);foreach(var a in e.Usuarios)t.Rows.Add(a.Documento,a.NombreCompleto,a.Telefono,a.Correo);break;
            default:
                t=Estilo.Tabla([("N.º",typeof(int)),("Documento",typeof(string)),("Usuario",typeof(string)),("Libro",typeof(string)),("Préstamo",typeof(DateTime)),("Límite",typeof(DateTime)),("Devolución",typeof(DateTime)),("Estado",typeof(string)),("Vencido",typeof(string))]);
                var ps=e.Prestamos.Where(x=>tipo.SelectedIndex switch{5=>!x.FechaDevolucion.HasValue,6=>x.FechaDevolucion.HasValue,7=>x.Vencido,_=>true});
                if(filtrar.Checked)ps=ps.Where(x=>x.FechaPrestamo.Date>=desde.Value.Date&&x.FechaPrestamo.Date<=hasta.Value.Date);
                foreach(var x in ps){var u=e.Usuarios.Single(a=>a.Id==x.IdUsuario);t.Rows.Add(x.Id,u.Documento,u.NombreCompleto,e.Libros.Single(a=>a.Id==x.IdLibro).Titulo,x.FechaPrestamo,x.FechaLimite,(object?)x.FechaDevolucion??DBNull.Value,x.Estado,x.Vencido?"Sí":"No");}break;
        }
        grilla.DataSource=t;foreach(DataGridViewColumn c in grilla.Columns)if(c.ValueType==typeof(DateTime))c.DefaultCellStyle.Format="dd/MM/yyyy";
        filtrar.Enabled=desde.Enabled=hasta.Enabled=tipo.SelectedIndex>=4;
    }
}
