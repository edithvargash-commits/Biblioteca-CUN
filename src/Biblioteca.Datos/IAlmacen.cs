using Biblioteca.Entidades;
namespace Biblioteca.Datos;
// La operacion lee, valida y escribe dentro de una sola transaccion.
public interface IAlmacen
{
    string Descripcion { get; }
    EstadoBiblioteca Leer();
    void Ejecutar(Action<EstadoBiblioteca> operacion);
}
