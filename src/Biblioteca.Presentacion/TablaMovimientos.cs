namespace Biblioteca.Presentacion;

internal static class TablaMovimientos
{
    // Las columnas existen ANTES de enlazar datos o incorporar la tabla a un Form.
    // No depende del orden de creación de BindingContext/Handle de Windows Forms.
    internal static DataGridView Crear()
    {
        var tabla=Estilo.Grilla();
        tabla.AutoGenerateColumns=false;
        foreach(var nombre in new[]{"Lector","Libro","Fecha","Estado"})
        {
            var columna=new DataGridViewTextBoxColumn
            {
                Name=nombre,HeaderText=nombre,DataPropertyName=nombre,
                ValueType=nombre=="Fecha"?typeof(DateTime):typeof(string),
                SortMode=DataGridViewColumnSortMode.Automatic
            };
            if(nombre=="Fecha")columna.DefaultCellStyle.Format="dd/MM/yyyy";
            tabla.Columns.Add(columna);
        }
        return tabla;
    }
}
