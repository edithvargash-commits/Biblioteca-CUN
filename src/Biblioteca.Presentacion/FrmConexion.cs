namespace Biblioteca.Presentacion;
public sealed class FrmConexion:Form
{
    public FrmConexion()
    {
        Estilo.Formato(this);Text="Conexión y modo de trabajo";MinimumSize=new Size(700,420);Size=new Size(760,460);
        var p=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,Padding=new Padding(22),AutoScroll=true};
        var modo=new ComboBox{Width=300,DropDownStyle=ComboBoxStyle.DropDownList};modo.Items.AddRange(["Demostración local","SQL Server"]);modo.SelectedIndex=Contexto.Config.Modo=="SQL"?1:0;
        var cadena=new TextBox{Multiline=true,Width=660,Height=85,Text=Contexto.Config.Cadena};
        p.Controls.Add(new Label{Text="Selecciona dónde guardar los datos",Font=Estilo.Editorial(20),AutoSize=true});p.Controls.Add(modo);
        p.Controls.Add(new Label{Text="Cadena de conexión SQL Server (autenticación de Windows)",AutoSize=true,Margin=new Padding(0,15,0,5)});p.Controls.Add(cadena);
        p.Controls.Add(new Label{Text="Antes de usar SQL Server, ejecuta SQL/01_Crear_Biblioteca.sql en SSMS.\nLos datos de demostración no se copian a SQL Server.\nUsa autenticación de Windows; evita guardar contraseñas en esta configuración.",AutoSize=true});
        p.Controls.Add(Estilo.Boton("Probar y guardar",()=>Estilo.Ejecutar(()=>{Contexto.Cambiar(new Configuracion{Modo=modo.SelectedIndex==1?"SQL":"Demo",Cadena=cadena.Text.Trim()});MessageBox.Show("Conexión comprobada y configuración guardada.");DialogResult=DialogResult.OK;Close();}),true));Estilo.Entrada(modo);Estilo.Entrada(cadena);Controls.Add(p);
    }
}
