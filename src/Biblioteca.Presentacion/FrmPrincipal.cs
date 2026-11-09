namespace Biblioteca.Presentacion;
public partial class FrmPrincipal : Form
{
    private readonly Panel contenido=new(){Dock=DockStyle.Fill,Padding=new Padding(26,22,26,18)};
    private readonly Label estado=new(){Dock=DockStyle.Bottom,Height=32,TextAlign=ContentAlignment.MiddleLeft,Padding=new Padding(18,0,0,0),BackColor=Color.White,ForeColor=Estilo.Suave,Font=new Font("Segoe UI",8.5f)};
    private readonly List<BotonRosa> navegacion=[];
    public FrmPrincipal(){InitializeComponent();Shown+=(_,_)=>Inicio();}
    private void Vaciar(){foreach(Control c in contenido.Controls.Cast<Control>().ToArray()){contenido.Controls.Remove(c);c.Dispose();}}
    private void Marcar(string texto){foreach(var b in navegacion){b.Activo=b.Text==texto;b.Invalidate();}}
    private void Abrir(Form f)
    {
        Vaciar();f.TopLevel=false;f.FormBorderStyle=FormBorderStyle.None;f.Dock=DockStyle.Fill;f.MinimumSize=Size.Empty;
        contenido.Controls.Add(f);f.Show();Marcar(f is FrmPrestamos?"Préstamos":f is FrmReportes?"Reportes":f.Text);ActualizarEstado();
    }
    private void ActualizarEstado()=>estado.Text="●  "+Contexto.Servicio.Descripcion+"     |     "+DateTime.Today.ToString("dd/MM/yyyy");
    private void Inicio()
    {
        Vaciar();Marcar("Inicio");
        var p=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true};
        var hero=new PanelRosa{Height=174,Width=850,ColorFondo=Estilo.Rosa,Margin=new Padding(0,0,0,20),Padding=new Padding(26)};
        hero.Controls.Add(new Label{Text="UN ESPACIO PARA EL CONOCIMIENTO",Font=new Font("Segoe UI",8.5f,FontStyle.Bold),ForeColor=Estilo.Verde,AutoSize=true,Location=new Point(26,22),BackColor=Estilo.Rosa});
        var frase=new Label{Text="Cada libro, una nueva historia.",Font=Estilo.Editorial(27),ForeColor=Estilo.Marino,AutoSize=true,Location=new Point(22,52),BackColor=Estilo.Rosa};hero.Controls.Add(frase);
        var descripcion=new Label{Text="Tu catálogo, tus lectores y tus préstamos, en un mismo lugar.",AutoSize=true,Font=new Font("Segoe UI",10),ForeColor=Estilo.Suave,Location=new Point(26,112),BackColor=Estilo.Rosa};hero.Controls.Add(descripcion);
        var titulo=new Label{Text="Tu biblioteca de un vistazo",AutoSize=true,Font=Estilo.Editorial(20),Margin=new Padding(0,0,0,14)};
        var metricas=new FlowLayoutPanel{Width=850,AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,WrapContents=true,Margin=new Padding(0,0,0,20)};
        p.Controls.Add(hero);p.Controls.Add(titulo);p.Controls.Add(metricas);
        var acceso=new FlowLayoutPanel{Width=850,AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,WrapContents=true,Margin=new Padding(0,0,0,20)};
        acceso.Controls.Add(Estilo.Boton("Ver catálogo",()=>Estilo.Ejecutar(()=>Abrir(new FrmLibros())),true));
        acceso.Controls.Add(Estilo.Boton("Nuevo préstamo",()=>Estilo.Ejecutar(()=>Abrir(new FrmPrestamos()))));
        acceso.Controls.Add(Estilo.Boton("Ver reportes",()=>Estilo.Ejecutar(()=>Abrir(new FrmReportes()))));
        p.Controls.Add(acceso);
        p.Controls.Add(new Label{Text="Movimientos recientes",AutoSize=true,Font=Estilo.Editorial(20),Margin=new Padding(0,0,0,14)});
        var recientes=TablaMovimientos.Crear();recientes.Dock=DockStyle.Fill;
        recientes.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
        recientes.Columns["Lector"].FillWeight=140;recientes.Columns["Libro"].FillWeight=200;
        recientes.Columns["Fecha"].FillWeight=100;recientes.Columns["Estado"].FillWeight=100;
        recientes.DataBindingComplete+=(_,_)=>recientes.ClearSelection();
        var caja=Estilo.Superficie(recientes);caja.Dock=DockStyle.None;caja.Width=850;caja.Height=238;p.Controls.Add(caja);
        Estilo.Ejecutar(()=>
        {
            var e=Contexto.Servicio.Consultar();
            foreach(var item in new[]{("Títulos registrados",e.Libros.Count),("Ejemplares disponibles",e.Libros.Sum(x=>e.Disponibles(x.Id))),("Préstamos activos",e.Prestamos.Count(x=>!x.FechaDevolucion.HasValue)),("Préstamos vencidos",e.Prestamos.Count(x=>x.Vencido)),("Lectores registrados",e.Usuarios.Count)})
            {
                var card=new PanelRosa{AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,MinimumSize=new Size(195,124),ColorFondo=Color.White,Margin=new Padding(0,0,12,12),Padding=new Padding(18,14,18,18)};
                var cuerpo=new FlowLayoutPanel{AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,FlowDirection=FlowDirection.TopDown,WrapContents=false,BackColor=Color.White,Location=new Point(18,14),Margin=new Padding(0)};
                cuerpo.Controls.Add(new Label{Text=item.Item2.ToString("N0"),Font=Estilo.Editorial(29),ForeColor=Estilo.Verde,AutoSize=true,MaximumSize=Size.Empty,BackColor=Color.White,Margin=new Padding(0,0,0,8)});
                cuerpo.Controls.Add(new Label{Text=item.Item1,Font=new Font("Segoe UI",9),ForeColor=Estilo.Suave,AutoSize=true,MaximumSize=Size.Empty,BackColor=Color.White,Margin=new Padding(0)});
                card.Controls.Add(cuerpo);metricas.Controls.Add(card);
            }
            var t=Estilo.Tabla([("Lector",typeof(string)),("Libro",typeof(string)),("Fecha",typeof(DateTime)),("Estado",typeof(string))]);
            foreach(var x in e.Prestamos.OrderByDescending(x=>x.Id).Take(5))t.Rows.Add(e.Usuarios.Single(u=>u.Id==x.IdUsuario).NombreCompleto,e.Libros.Single(l=>l.Id==x.IdLibro).Titulo,x.FechaPrestamo,x.Estado);
            recientes.DataSource=t;recientes.ClearSelection();
            if(t.Rows.Count==0){var vacio=new Label{Text="Aún no hay movimientos. Registra tu primer préstamo para comenzar.",Dock=DockStyle.Bottom,Height=48,ForeColor=Estilo.Suave,BackColor=Color.White,TextAlign=ContentAlignment.MiddleCenter};caja.Controls.Add(vacio);}
        });
        void Ajustar()
        {
            int ancho=Math.Max(560,p.ClientSize.Width-SystemInformation.VerticalScrollBarWidth-8);
            hero.Width=metricas.Width=acceso.Width=caja.Width=ancho;
            metricas.MaximumSize=new Size(ancho,0);acceso.MaximumSize=new Size(ancho,0);
            frase.MaximumSize=new Size(Math.Max(100,hero.ClientSize.Width-52),0);
            descripcion.MaximumSize=new Size(Math.Max(100,hero.ClientSize.Width-52),0);
            descripcion.Top=frase.Bottom+14;hero.Height=Math.Max(174,descripcion.Bottom+24);
        }
        p.ClientSizeChanged+=(_,_)=>Ajustar();contenido.Controls.Add(p);Ajustar();ActualizarEstado();
    }
}
