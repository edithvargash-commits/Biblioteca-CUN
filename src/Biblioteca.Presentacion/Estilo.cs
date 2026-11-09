using System.Data;
using System.Globalization;
using System.Text;
using Biblioteca.Entidades;
namespace Biblioteca.Presentacion;
internal static class Estilo
{

    public static readonly Color Marino=Color.FromArgb(57,42,47), Verde=Color.FromArgb(132,73,94), Fondo=Color.FromArgb(251,247,244);
    public static readonly Color Rosa=Color.FromArgb(246,231,231), Nude=Color.FromArgb(238,216,201), Borde=Color.FromArgb(229,214,210), Suave=Color.FromArgb(119,95,102);
    public static Font Editorial(float size=24)=>new("Georgia",size,FontStyle.Regular);
    public static void Formato(Form f)
    {
        f.Font=new Font("Segoe UI",10);f.ForeColor=Marino;f.BackColor=Fondo;f.StartPosition=FormStartPosition.CenterParent;
        f.AutoScaleMode=AutoScaleMode.Dpi;f.MinimumSize=new Size(980,700);f.Size=new Size(1200,800);
    }
    public static Button Boton(string texto,Action accion,bool principal=false)
    {
        var b=new BotonRosa{Text=texto,Principal=principal,AutoSize=true,AutoSizeMode=AutoSizeMode.GrowAndShrink,MinimumSize=new Size(140,40),Size=new Size(texto=="Registrar devolución"?182:texto.Length>16?166:140,40),Margin=new Padding(0,4,10,4),Font=new Font("Segoe UI",10,FontStyle.Bold),Cursor=Cursors.Hand};
        b.Click+=(_,_)=>accion();return b;
    }
    public static Control Encabezado(string titulo,string descripcion)
    {
        var p=new Panel{Dock=DockStyle.Fill,BackColor=Fondo};
        var l=new Label{Text=titulo,AutoSize=true,Font=Editorial(25),ForeColor=Marino,Location=new Point(0,0)};
        var d=new Label{Text=descripcion,AutoSize=true,Font=new Font("Segoe UI",9.5f),ForeColor=Suave,Location=new Point(2,47)};
        p.Controls.AddRange([l,d]);return p;
    }
    public static Panel Superficie(Control contenido,int espacio=12)
    {
        var p=new PanelRosa{Dock=DockStyle.Fill,Padding=new Padding(espacio),Margin=new Padding(0,6,0,10),ColorFondo=Color.White};
        contenido.Dock=DockStyle.Fill;p.Controls.Add(contenido);return p;
    }
    public static void Entrada(Control c)
    {
        c.BackColor=Color.White;c.ForeColor=Marino;c.Margin=new Padding(0,6,12,8);
        if(c is TextBox t){t.BorderStyle=BorderStyle.FixedSingle;}
        if(c is ComboBox b){b.FlatStyle=FlatStyle.Flat;b.DropDownWidth=Math.Max(340,b.Width);}
    }
    public static void Ejecutar(Action accion)
    {
        try { accion(); }
        catch(ReglaException ex){MessageBox.Show(ex.Message,"Revise los datos",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        catch(Exception ex){MessageBox.Show("No se pudo completar la operación.\n\n"+ex.Message+"\n\nSi usa SQL Server, revise Conexión y ejecute SQL/01_Crear_Biblioteca.sql.","Biblioteca CUN",MessageBoxButtons.OK,MessageBoxIcon.Error);}
    }

    public static DataGridView Grilla()
    {
        var g=new DataGridView{Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,AllowUserToDeleteRows=false,MultiSelect=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.DisplayedCells,BackgroundColor=Color.White,BorderStyle=BorderStyle.None,RowHeadersVisible=false,AutoGenerateColumns=true,EnableHeadersVisualStyles=false,GridColor=Borde,CellBorderStyle=DataGridViewCellBorderStyle.SingleHorizontal,ColumnHeadersBorderStyle=DataGridViewHeaderBorderStyle.None,ColumnHeadersHeightSizeMode=DataGridViewColumnHeadersHeightSizeMode.DisableResizing,ColumnHeadersHeight=46};
        g.ColumnHeadersDefaultCellStyle=new DataGridViewCellStyle{BackColor=Rosa,ForeColor=Marino,Font=new Font("Segoe UI",9.5f,FontStyle.Bold),Padding=new Padding(12,4,12,4),SelectionBackColor=Rosa,SelectionForeColor=Marino};
        g.DefaultCellStyle=new DataGridViewCellStyle{BackColor=Color.White,ForeColor=Marino,Font=new Font("Segoe UI",10),SelectionBackColor=Color.FromArgb(237,211,219),SelectionForeColor=Marino,Padding=new Padding(12,5,12,5)};
        g.AlternatingRowsDefaultCellStyle.BackColor=Color.FromArgb(253,249,247);g.RowTemplate.Height=40;
        return g;
    }
    public static DataTable Tabla(IEnumerable<(string Columna,Type Tipo)> columnas)
    {var t=new DataTable();foreach(var c in columnas)t.Columns.Add(c.Columna,c.Tipo);return t;}
    public static void Exportar(DataGridView g)
    {
        if(g.Rows.Count==0)throw new ReglaException("No hay filas para exportar.");
        using var d=new SaveFileDialog{Filter="CSV para Excel (*.csv)|*.csv",FileName="Biblioteca_"+DateTime.Now.ToString("yyyyMMdd_HHmm")+".csv"};
        if(d.ShowDialog()!=DialogResult.OK)return;
        static string Csv(object? value)
        {
            string s=Convert.ToString(value,CultureInfo.CurrentCulture)??"";
            if(s.TrimStart().StartsWith('=')||s.TrimStart().StartsWith('+')||s.TrimStart().StartsWith('-')||s.TrimStart().StartsWith('@'))s="'"+s;
            return "\""+s.Replace("\"","\"\"")+"\"";
        }
        var cols=g.Columns.Cast<DataGridViewColumn>().Where(c=>c.Visible).OrderBy(c=>c.DisplayIndex).ToArray();
        using var w=new StreamWriter(d.FileName,false,new UTF8Encoding(true));
        w.WriteLine(string.Join(';',cols.Select(c=>Csv(c.HeaderText))));
        foreach(DataGridViewRow row in g.Rows)w.WriteLine(string.Join(';',cols.Select(c=>Csv(row.Cells[c.Index].Value))));
        MessageBox.Show("Reporte guardado.","Biblioteca CUN");
    }
}
public sealed record Opcion(int Id,string Texto);
