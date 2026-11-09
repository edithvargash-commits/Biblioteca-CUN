namespace Biblioteca.Presentacion;
public partial class FrmPrincipal
{
    private void InitializeComponent()
    {
        Estilo.Formato(this);Text="Biblioteca CUN | Sistema de gestión";WindowState=FormWindowState.Maximized;
        var menu=new FlowLayoutPanel{Dock=DockStyle.Left,Width=264,AutoSize=false,BackColor=Color.FromArgb(247,235,230),FlowDirection=FlowDirection.TopDown,WrapContents=false,Padding=new Padding(16,20,16,12),AutoScroll=true};
        var marca=new MarcaBiblioteca();
        menu.Controls.Add(marca);
        menu.Controls.Add(new Label{Text="ESPACIO DE TRABAJO",AutoSize=true,Font=new Font("Segoe UI",8,FontStyle.Bold),ForeColor=Estilo.Suave,Margin=new Padding(10,0,0,12)});
        foreach(var (texto,accion) in new (string,Action)[]{("Inicio",Inicio),("Libros",()=>Abrir(new FrmLibros())),("Usuarios",()=>Abrir(new FrmUsuarios())),("Autores",()=>Abrir(new FrmAutores())),("Editoriales",()=>Abrir(new FrmEditoriales())),("Préstamos",()=>Abrir(new FrmPrestamos())),("Reportes",()=>Abrir(new FrmReportes())),("Conexión",()=>{using var f=new FrmConexion();f.ShowDialog(this);Inicio();}),("Salir",Close)})
        {
            var b=new BotonRosa{Text=texto,Icono=texto,Navegacion=true,Width=224,Height=43,Font=new Font("Segoe UI",10),Cursor=Cursors.Hand,Margin=new Padding(0,1,0,3)};
            b.Click+=(_,_)=>Estilo.Ejecutar(accion);navegacion.Add(b);menu.Controls.Add(b);
        }
        Controls.Add(contenido);Controls.Add(estado);Controls.Add(menu);
    }
}
