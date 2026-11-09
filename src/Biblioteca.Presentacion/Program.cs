using System.Text.Json;
using Biblioteca.Datos;
using Biblioteca.Negocio;
namespace Biblioteca.Presentacion;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_,e)=>MessageBox.Show(e.Exception.Message,"Biblioteca CUN",MessageBoxButtons.OK,MessageBoxIcon.Error);
        try { Application.Run(new FrmPrincipal()); }
        catch(Exception e) { MessageBox.Show(e.Message,"No se pudo iniciar Biblioteca CUN",MessageBoxButtons.OK,MessageBoxIcon.Error); }
    }
}
internal sealed class Configuracion
{
    public string Modo { get; set; }="Demo";
    public string Cadena { get; set; }=@"Server=.\SQLEXPRESS;Database=BibliotecaCUN;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5;";
}
internal static class Contexto
{
    public static string Carpeta => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"BibliotecaCUN");
    public static Configuracion Config { get; private set; }=Cargar();
    public static BibliotecaServicio Servicio { get; private set; }=Crear(Config);
    private static Configuracion Cargar()
    {
        Directory.CreateDirectory(Carpeta);
        string p=Path.Combine(Carpeta,"configuracion.json");
        return File.Exists(p)?JsonSerializer.Deserialize<Configuracion>(File.ReadAllText(p))??new():new();
    }
    private static BibliotecaServicio Crear(Configuracion c)=>new(c.Modo=="SQL"?new AlmacenSql(c.Cadena):new AlmacenJson(Path.Combine(Carpeta,"demostracion.json")));
    public static void Cambiar(Configuracion c)
    {
        var s=Crear(c);s.Consultar(); // Probar antes de reemplazar la configuracion activa.
        File.WriteAllText(Path.Combine(Carpeta,"configuracion.json"),JsonSerializer.Serialize(c,new JsonSerializerOptions{WriteIndented=true}));
        Config=c;Servicio=s;
    }
}
