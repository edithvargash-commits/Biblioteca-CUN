namespace Biblioteca.Presentacion;
public partial class FrmLibros
{
    private void InitializeComponent()
    {
        Texto("Codigo","Código *",30);Texto("ISBN","ISBN *",25);Texto("Titulo","Título *",200);
        Combo("Autor","Autor *");Combo("Editorial","Editorial *");Texto("Categoria","Categoría",100);
        Numero("Anio","Año *",DateTime.Today.Year+1);Numero("Existencias","Ejemplares totales *");
    }
}
