using System.Data;
using Biblioteca.Presentacion;

internal static class Prueba
{
    [STAThread]
    private static int Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        using var tabla=TablaMovimientos.Crear();
        if(tabla.Parent is not null || tabla.IsHandleCreated)throw new Exception("La prueba requiere tabla sin formulario ni handle.");
        Comprobar(tabla);
        var datos=new DataTable();
        datos.Columns.Add("Lector");datos.Columns.Add("Libro");datos.Columns.Add("Fecha",typeof(DateTime));datos.Columns.Add("Estado");
        tabla.DataSource=datos;Comprobar(tabla);
        datos.Rows.Add("Usuario de prueba","Libro de prueba",new DateTime(2026,11,9),"Devuelto");
        Comprobar(tabla);
        using var ventana=new Form();ventana.Controls.Add(tabla);
        ventana.CreateControl();tabla.CreateControl();
        tabla.BindingContext=new BindingContext();tabla.DataSource=datos;Comprobar(tabla);
        if(tabla.Rows.Count!=1)throw new Exception("Debe enlazar una fila al crear el contexto.");
        Console.WriteLine("OK: columnas antes del handle; datos vacíos; fila; enlace al formulario; formato de fecha.");
        return 0;
    }
    private static void Comprobar(DataGridView tabla)
    {
        if(tabla.AutoGenerateColumns||tabla.Columns.Count!=4)throw new Exception("Se requieren cuatro columnas explícitas.");
        foreach(var nombre in new[]{"Lector","Libro","Fecha","Estado"})
        {
            var c=tabla.Columns[nombre]??throw new Exception("Falta la columna "+nombre);
            if(c.DataPropertyName!=nombre)throw new Exception("Enlace incorrecto de "+nombre);
        }
        if(tabla.Columns["Fecha"].DefaultCellStyle.Format!="dd/MM/yyyy")throw new Exception("Formato de fecha incorrecto.");
    }
}
