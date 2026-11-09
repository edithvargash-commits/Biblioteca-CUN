using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Biblioteca.Presentacion;

// Marca compacta: dimensiones acotadas y dibujo sin medir ni redimensionar el menu.
internal sealed class MarcaBiblioteca : Panel
{
    private readonly ImagenLibro libro = new();
    private readonly Font fuenteTitulo = new("Georgia", 32, FontStyle.Bold, GraphicsUnit.Pixel);
    private readonly Font fuenteDetalle = new("Segoe UI", 12, FontStyle.Regular, GraphicsUnit.Pixel);

    public MarcaBiblioteca()
    {
        DoubleBuffered=true;ResizeRedraw=true;AutoSize=false;
        Size=new Size(224,154);
        BackColor=Color.FromArgb(247,235,230);
        Margin=new Padding(0,0,0,20);
        AccessibleName="Biblioteca CUN";
        libro.Bounds=new Rectangle(4,0,66,58);Controls.Add(libro);
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g=e.Graphics;
        var estado=g.Save();
        try
        {
            g.PageUnit=GraphicsUnit.Pixel;
            g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
            using var principal=new SolidBrush(Estilo.Marino);
            using var secundario=new SolidBrush(Estilo.Suave);
            using var formato=new StringFormat(StringFormat.GenericTypographic)
            {
                FormatFlags=StringFormatFlags.NoWrap,
                Trimming=StringTrimming.None,
                Alignment=StringAlignment.Near,
                LineAlignment=StringAlignment.Near
            };
            // Texto breve y completo en una sola linea, a escala visual de la referencia.
            // Mantiene la proporción aprobada sin ensanchar el menú ni dividir la palabra.
            var medida=g.MeasureString("BIBLIOTECA",fuenteTitulo,PointF.Empty,formato);
            float factor=Math.Min(1f,216f/Math.Max(1f,medida.Width));
            var marca=g.Save();
            g.TranslateTransform(4,76);g.ScaleTransform(factor,1f);
            g.DrawString("BIBLIOTECA",fuenteTitulo,principal,PointF.Empty,formato);
            g.Restore(marca);
            g.DrawString("CUN / Gestión de biblioteca",fuenteDetalle,secundario,new RectangleF(4,120,216,24),formato);
        }
        finally { g.Restore(estado); }
    }
    protected override void Dispose(bool disposing)
    {
        if(disposing){fuenteTitulo.Dispose();fuenteDetalle.Dispose();}
        base.Dispose(disposing);
    }
}
