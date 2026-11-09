using System.Drawing.Drawing2D;
using System.Reflection;
namespace Biblioteca.Presentacion;

internal static class Geometria
{
    public static GraphicsPath Redondo(RectangleF r,float radio)
    {
        float d=Math.Min(radio*2,Math.Min(r.Width,r.Height));var p=new GraphicsPath();
        p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;
    }
}
internal sealed class PanelRosa:Panel
{
    public Color ColorFondo {get;set;}=Color.White;
    public Color ColorBorde {get;set;}=Estilo.Borde;
    public PanelRosa(){DoubleBuffered=true;BackColor=Estilo.Fondo;ResizeRedraw=true;}
    protected override void OnPaint(PaintEventArgs e)
    {
        if(ClientSize.Width<3||ClientSize.Height<3)return;
        e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
        using var p=Geometria.Redondo(new RectangleF(.5f,.5f,ClientSize.Width-1,ClientSize.Height-1),12*DeviceDpi/96f);
        using var b=new SolidBrush(ColorFondo);using var pen=new Pen(ColorBorde);
        e.Graphics.FillPath(b,p);e.Graphics.DrawPath(pen,p);base.OnPaint(e);
    }
}
internal sealed class BotonRosa:Button
{
    public bool Principal {get;set;}
    public bool Navegacion {get;set;}
    public bool Activo {get;set;}
    public string Icono {get;set;}="";
    private bool sobre;
    public BotonRosa(){FlatStyle=FlatStyle.Flat;FlatAppearance.BorderSize=0;BackColor=Estilo.Fondo;UseVisualStyleBackColor=false;DoubleBuffered=true;}
    public override Size GetPreferredSize(Size proposedSize)
    {
        Size texto;
        var flags=TextFormatFlags.SingleLine|TextFormatFlags.NoPadding;
        if(IsHandleCreated){using var g=CreateGraphics();texto=TextRenderer.MeasureText(g,Text,Font,Size.Empty,flags);}
        else texto=TextRenderer.MeasureText(Text,Font,Size.Empty,flags);
        int escalaPadding=(int)Math.Ceiling((Navegacion?64:36)*DeviceDpi/96f);
        return new Size(Math.Max(Navegacion?200:140,texto.Width+escalaPadding),Math.Max(40,texto.Height+(int)Math.Ceiling(18*DeviceDpi/96f)));
    }
    protected override void OnHandleCreated(EventArgs e){base.OnHandleCreated(e);if(!Navegacion)Size=GetPreferredSize(Size.Empty);}
    protected override void OnDpiChangedAfterParent(EventArgs e){base.OnDpiChangedAfterParent(e);if(!Navegacion)Size=GetPreferredSize(Size.Empty);}
    protected override void OnMouseEnter(EventArgs e){sobre=true;Invalidate();base.OnMouseEnter(e);}
    protected override void OnMouseLeave(EventArgs e){sobre=false;Invalidate();base.OnMouseLeave(e);}
    protected override void OnPaint(PaintEventArgs e)
    {
        var g=e.Graphics;g.Clear(Parent?.BackColor??Estilo.Fondo);g.SmoothingMode=SmoothingMode.AntiAlias;
        bool lleno=Principal||Activo;
        Color fondo=lleno?(sobre?Color.FromArgb(112,59,78):Estilo.Verde):sobre?Estilo.Rosa:Navegacion?Color.FromArgb(247,235,230):Color.White;
        Color texto=lleno?Color.White:Estilo.Marino;
        using var p=Geometria.Redondo(new RectangleF(1,1,Width-2,Height-2),8*DeviceDpi/96f);
        using var b=new SolidBrush(fondo);g.FillPath(b,p);
        if(!Navegacion&&!lleno){using var pen=new Pen(Estilo.Borde);g.DrawPath(pen,p);}
        int margen=(int)(14*DeviceDpi/96f),icono=(int)(20*DeviceDpi/96f);
        if(Navegacion)
        {
            Iconos.Dibujar(g,Icono,new Rectangle(margen,(Height-icono)/2,icono,icono),texto);
            TextRenderer.DrawText(g,Text,Font,new Rectangle(margen+icono+12,0,Width-margen-icono-14,Height),texto,TextFormatFlags.Left|TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis);
        }
        else TextRenderer.DrawText(g,Text,Font,ClientRectangle,texto,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis);
        if(Focused)ControlPaint.DrawFocusRectangle(g,new Rectangle(5,5,Width-10,Height-10),texto,fondo);
    }
}
internal static class Iconos
{
    public static void Dibujar(Graphics g,string tipo,Rectangle r,Color color)
    {
        var estado=g.Save();g.TranslateTransform(r.X,r.Y);g.ScaleTransform(r.Width/24f,r.Height/24f);
        using var p=new Pen(color,1.6f){StartCap=LineCap.Round,EndCap=LineCap.Round,LineJoin=LineJoin.Round};
        switch(tipo)
        {
            case "Inicio":g.DrawLines(p,[new PointF(3,11),new PointF(12,3),new PointF(21,11)]);g.DrawLines(p,[new PointF(6,9),new PointF(6,21),new PointF(18,21),new PointF(18,9)]);break;
            case "Usuarios":case "Autores":g.DrawEllipse(p,8,3,8,8);g.DrawArc(p,4,13,16,14,180,180);break;
            case "Editoriales":g.DrawRectangle(p,5,4,14,17);g.DrawLine(p,9,8,15,8);g.DrawLine(p,9,12,15,12);g.DrawLine(p,9,16,15,16);break;
            case "Préstamos":g.DrawLine(p,4,8,20,8);g.DrawLines(p,[new PointF(16,4),new PointF(20,8),new PointF(16,12)]);g.DrawLine(p,20,17,4,17);g.DrawLines(p,[new PointF(8,13),new PointF(4,17),new PointF(8,21)]);break;
            case "Reportes":g.DrawLine(p,4,21,21,21);g.DrawRectangle(p,5,12,3,7);g.DrawRectangle(p,11,8,3,11);g.DrawRectangle(p,17,3,3,16);break;
            case "Conexión":g.DrawEllipse(p,3,3,6,6);g.DrawEllipse(p,15,15,6,6);g.DrawLine(p,8,8,16,16);break;
            case "Salir":g.DrawLines(p,[new PointF(11,3),new PointF(4,3),new PointF(4,21),new PointF(11,21)]);g.DrawLine(p,10,12,21,12);g.DrawLines(p,[new PointF(17,8),new PointF(21,12),new PointF(17,16)]);break;
            default:g.DrawLines(p,[new PointF(12,6),new PointF(3,4),new PointF(3,19),new PointF(12,21),new PointF(21,19),new PointF(21,4),new PointF(12,6),new PointF(12,21)]);break;
        }g.Restore(estado);
    }
}
internal sealed class ImagenLibro:Control
{
    private static readonly Lazy<Image?> imagen=new(()=>
    {
        using var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("Biblioteca.Presentacion.Assets.libro_rosa.png");
        if(stream is null)return null;
        using var original=Image.FromStream(stream);return new Bitmap(original);
    });
    public ImagenLibro(){DoubleBuffered=true;BackColor=Color.FromArgb(247,235,230);}
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;
        if(imagen.Value is Image im)
        {
            g.InterpolationMode=InterpolationMode.HighQualityBicubic;float escala=Math.Min(Width/(float)im.Width,Height/(float)im.Height);
            var size=new SizeF(im.Width*escala,im.Height*escala);g.DrawImage(im,(Width-size.Width)/2,(Height-size.Height)/2,size.Width,size.Height);
        }
        else Iconos.Dibujar(g,"Libros",new Rectangle(8,8,Math.Max(1,Width-16),Math.Max(1,Height-16)),Estilo.Verde);
    }
}
