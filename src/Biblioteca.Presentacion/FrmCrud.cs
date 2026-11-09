using System.Data;
using Biblioteca.Entidades;
namespace Biblioteca.Presentacion;
public abstract class FrmCrud : Form
{
    protected readonly DataGridView grilla=Estilo.Grilla();
    protected readonly TextBox buscar=new(){Width=280,PlaceholderText="Escribe para buscar…"};
    protected readonly FlowLayoutPanel campos=new(){Dock=DockStyle.Fill,AutoScroll=true,WrapContents=true,Padding=new Padding(0,8,0,0)};
    protected readonly Dictionary<string,Control> entradas=[];
    protected EstadoBiblioteca datos=new();
    protected int seleccionado;
    private readonly string titulo;
    protected FrmCrud(string tituloFormulario)
    {
        titulo=tituloFormulario;Estilo.Formato(this);Text=titulo;
        var layout=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=1,RowCount=5,Padding=new Padding(0)};
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute,86));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,56));layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,224));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,64));
        layout.Controls.Add(Estilo.Encabezado(titulo,"Consulta y organiza la información de tu biblioteca."),0,0);
        var herramientas=new FlowLayoutPanel{Dock=DockStyle.Fill,WrapContents=false,AutoScroll=true};Estilo.Entrada(buscar);buscar.Margin=new Padding(0,8,12,6);herramientas.Controls.Add(buscar);herramientas.Controls.Add(Estilo.Boton("Actualizar",()=>Estilo.Ejecutar(Recargar)));herramientas.Controls.Add(Estilo.Boton("Exportar CSV",()=>Estilo.Ejecutar(()=>Estilo.Exportar(grilla))));
        layout.Controls.Add(herramientas,0,1);layout.Controls.Add(Estilo.Superficie(grilla),0,2);
        var editor=Estilo.Superficie(campos);
        editor.Controls.Add(new Label{Text="DATOS DEL REGISTRO",Dock=DockStyle.Top,Height=28,Font=new Font("Segoe UI",8.5f,FontStyle.Bold),ForeColor=Estilo.Verde,BackColor=Color.White});
        campos.BackColor=Color.White;layout.Controls.Add(editor,0,3);
        var acciones=new FlowLayoutPanel{Dock=DockStyle.Fill,WrapContents=false,AutoScroll=true,Padding=new Padding(0,8,0,0)};
        acciones.Controls.Add(Estilo.Boton("Nuevo / limpiar",()=>Estilo.Ejecutar(Limpiar)));
        acciones.Controls.Add(Estilo.Boton("Guardar",()=>Estilo.Ejecutar(()=>{GuardarRegistro();Recargar();MessageBox.Show("Registro guardado.",titulo);}),true));
        acciones.Controls.Add(Estilo.Boton("Eliminar",()=>Estilo.Ejecutar(()=>{
            if(seleccionado==0)throw new ReglaException("Seleccione una fila primero.");
            if(MessageBox.Show("¿Eliminar el registro seleccionado? Esta operación no se puede deshacer.",titulo,MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes){EliminarRegistro();Recargar();}
        })));
        AgregarAcciones(acciones);layout.Controls.Add(acciones,0,4);Controls.Add(layout);
        buscar.TextChanged+=(_,_)=>Estilo.Ejecutar(Listar);
        grilla.SelectionChanged+=(_,_)=>Estilo.Ejecutar(()=>{if(grilla.SelectedRows.Count>0 && grilla.CurrentRow?.Cells["Id"].Value is int id){seleccionado=id;CargarSeleccion();}});
        Shown+=(_,_)=>Estilo.Ejecutar(Recargar);
    }
    protected virtual void AgregarAcciones(FlowLayoutPanel p) { }
    protected TextBox Texto(string clave,string etiqueta,int max=100)
    {var x=new TextBox{Width=220,MaxLength=max};Agregar(clave,etiqueta,x);return x;}
    protected NumericUpDown Numero(string clave,string etiqueta,int max=1000000)
    {var x=new NumericUpDown{Width=220,Minimum=0,Maximum=max};Agregar(clave,etiqueta,x);return x;}
    protected ComboBox Combo(string clave,string etiqueta)
    {var x=new ComboBox{Width=220,DropDownStyle=ComboBoxStyle.DropDownList,DisplayMember="Texto",ValueMember="Id"};Agregar(clave,etiqueta,x);return x;}
    protected DateTimePicker Fecha(string clave,string etiqueta)
    {var x=new DateTimePicker{Width=220,Format=DateTimePickerFormat.Short};Agregar(clave,etiqueta,x);return x;}

    private void Agregar(string clave,string etiqueta,Control c)
    {
        entradas.Add(clave,c);Estilo.Entrada(c);
        var p=new FlowLayoutPanel{FlowDirection=FlowDirection.TopDown,Width=240,MinimumSize=new Size(240,82),AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,WrapContents=false,BackColor=Color.White,Margin=new Padding(4,0,8,4)};
        p.Controls.Add(new Label{Text=etiqueta,AutoSize=true,Font=new Font("Segoe UI",9),ForeColor=Estilo.Suave,Margin=new Padding(0,0,0,0)});p.Controls.Add(c);campos.Controls.Add(p);
    }
    protected string T(string clave)=>entradas[clave].Text.Trim();
    protected int N(string clave)=>(int)((NumericUpDown)entradas[clave]).Value;
    protected int C(string clave)=>((ComboBox)entradas[clave]).SelectedValue is int i?i:0;
    protected DateTime F(string clave)=>((DateTimePicker)entradas[clave]).Value.Date;
    protected void P(string clave,object? valor)
    {
        var c=entradas[clave];switch(c)
        {case NumericUpDown n:n.Value=Math.Clamp(Convert.ToDecimal(valor),n.Minimum,n.Maximum);break;
         case ComboBox combo:combo.SelectedValue=valor??0;break;
         case DateTimePicker f:f.Value=valor is DateTime d?d:DateTime.Today;break;
         default:c.Text=Convert.ToString(valor)??"";break;}
    }
    protected void Opciones(string clave,IEnumerable<Opcion> opciones)
    {var c=(ComboBox)entradas[clave];c.DataSource=opciones.ToList();c.SelectedIndex=-1;}
    protected void Recargar(){datos=Contexto.Servicio.Consultar();CargarOpciones();Listar();Limpiar();}
    protected virtual void CargarOpciones() { }
    protected virtual void Limpiar()
    {
        grilla.ClearSelection();grilla.CurrentCell=null;seleccionado=0;
        foreach(var c in entradas.Values){if(c is TextBox)t(c);else if(c is NumericUpDown n)n.Value=0;else if(c is ComboBox b)b.SelectedIndex=-1;else if(c is DateTimePicker f)f.Value=DateTime.Today;}
        static void t(Control c)=>c.Text="";
    }
    protected void Mostrar(DataTable t)
    {
        grilla.DataSource=t;if(grilla.Columns.Contains("Id"))grilla.Columns["Id"].Visible=false;
    }
    protected bool Coincide(params string[] valores)=>valores.Any(x=>x.Contains(buscar.Text.Trim(),StringComparison.CurrentCultureIgnoreCase));
    protected abstract void Listar();
    protected abstract void CargarSeleccion();
    protected abstract void GuardarRegistro();
    protected abstract void EliminarRegistro();
}
