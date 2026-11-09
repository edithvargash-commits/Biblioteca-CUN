using Biblioteca.Datos;
using Biblioteca.Entidades;
using Biblioteca.Negocio;

int exitos=0,fallos=0;
string dir=Path.Combine(Path.GetTempPath(),"BibliotecaCUN-Pruebas-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(dir);
BibliotecaServicio Nuevo(string nombre)=>new(new AlmacenJson(Path.Combine(dir,nombre+".json")));
void Igual<T>(T esperado,T real){if(!EqualityComparer<T>.Default.Equals(esperado,real))throw new Exception($"Esperado {esperado}; recibido {real}");}
void Rechaza(Action a){try{a();}catch(ReglaException){return;}throw new Exception("Debió rechazar la operación.");}
void Caso(string nombre,Action prueba)
{try{prueba();exitos++;Console.WriteLine("OK  "+nombre);}catch(Exception ex){fallos++;Console.WriteLine("ERROR  "+nombre+": "+ex.Message);}}
Prestamo P(int libro=1,int usuario=1)=>new(){IdLibro=libro,IdUsuario=usuario,FechaPrestamo=DateTime.Today,FechaLimite=DateTime.Today.AddDays(14)};
if(args.Length>0 && args[0]=="--sql")
{
 if(args.Length!=2){Console.WriteLine("Uso: --sql <cadena de conexión de base de práctica>");return 2;}
 try { PruebaSql(args[1]); Console.WriteLine("SQL: prueba de integración aprobada."); return 0; }
 catch(Exception ex){Console.WriteLine("SQL: ERROR: "+ex.Message);return 1;}
 finally { try{Directory.Delete(dir,true);}catch(IOException){} }
}
Caso("ISBN-10 / ISBN-13 y digito incorrecto",()=>{
 Igual(true,BibliotecaServicio.IsbnValido("978-0-307-47472-8"));Igual(true,BibliotecaServicio.IsbnValido("0-306-40615-2"));Igual(false,BibliotecaServicio.IsbnValido("9780307474729"));Igual(false,BibliotecaServicio.IsbnValido("ABCDEFGHIJKLM"));});
Caso("Datos iniciales y persistencia",()=>{var s=Nuevo("persistencia");int id=s.Guardar(new Autor("Nombre","Apellido"));Igual(4,id);Igual(4,Nuevo("persistencia").Consultar().Autores.Count);});
Caso("CRUD autor",()=>{var s=Nuevo("autor");int id=s.Guardar(new Autor("Nombre","Apellido"));s.Guardar(new Autor("Modificado","Apellido"){Id=id});Igual("Modificado",s.Consultar().Autores.Single(x=>x.Id==id).Nombre);s.Eliminar<Autor>(id);Igual(3,s.Consultar().Autores.Count);});
Caso("CRUD editorial",()=>{var s=Nuevo("editorial");int id=s.Guardar(new Editorial("Nueva"));s.Guardar(new Editorial("Editada"){Id=id});Igual("Editada",s.Consultar().Editoriales.Single(x=>x.Id==id).Nombre);s.Eliminar<Editorial>(id);Igual(2,s.Consultar().Editoriales.Count);});
Caso("CRUD usuario",()=>{var s=Nuevo("usuario");int id=s.Guardar(new Usuario{Nombre="Luis",Apellido="Ejemplo",Documento="D2"});var u=s.Consultar().Usuarios.Single(x=>x.Id==id);u.Telefono="123";s.Guardar(u);Igual("123",s.Consultar().Usuarios.Single(x=>x.Id==id).Telefono);s.Eliminar<Usuario>(id);Igual(1,s.Consultar().Usuarios.Count);});
Caso("CRUD libro y normalizacion ISBN",()=>{var s=Nuevo("libro");var l=new Libro{Codigo="NUEVO",ISBN="0-306-40615-2",Titulo="Libro de prueba",IdAutor=1,IdEditorial=1,Anio=2020,Existencias=2};int id=s.Guardar(l);l=s.Consultar().Libros.Single(x=>x.Id==id);Igual("0306406152",l.ISBN);l.Titulo="Editado";s.Guardar(l);Igual("Editado",s.Consultar().Libros.Single(x=>x.Id==id).Titulo);s.Eliminar<Libro>(id);Igual(3,s.Consultar().Libros.Count);});
Caso("Duplicados y rollback ISBN",()=>{var s=Nuevo("isbn");var l=s.Consultar().Libros[0];l.Id=0;l.Codigo="OTRO";Rechaza(()=>s.Guardar(l));Igual(3,s.Consultar().Libros.Count);});
Caso("Codigo unico sin distinguir mayusculas",()=>{var s=Nuevo("codigo");var l=s.Consultar().Libros[0];l.Codigo="lib-002";Rechaza(()=>s.Guardar(l));Igual("LIB-001",s.Consultar().Libros[0].Codigo);});
Caso("Documento duplicado",()=>{var s=Nuevo("duplicado");Rechaza(()=>s.Guardar(new Usuario{Nombre="Otra",Apellido="Persona",Documento="DEMO-001"}));});
Caso("Correo invalido",()=>{var s=Nuevo("correo");var u=s.Consultar().Usuarios[0];u.Correo="texto";Rechaza(()=>s.Guardar(u));});
Caso("Obligatorios y longitudes",()=>{var s=Nuevo("vacios");Rechaza(()=>s.Guardar(new Autor("","A")));Rechaza(()=>s.Guardar(new Editorial(new string('x',151))));});
Caso("FK autor / editorial",()=>{var s=Nuevo("fk");var l=s.Consultar().Libros[0];l.IdAutor=999;Rechaza(()=>s.Guardar(l));});
Caso("Prestamo reduce disponibilidad; devolucion la restaura",()=>{var s=Nuevo("devolver");int id=s.Guardar(P());Igual(4,s.Consultar().Disponibles(1));s.Devolver(id,DateTime.Today);Igual(5,s.Consultar().Disponibles(1));Igual("Devuelto",s.Consultar().Prestamos[0].Estado);Rechaza(()=>s.Devolver(id,DateTime.Today));});
Caso("Prestamo duplicado activo",()=>{var s=Nuevo("doble");s.Guardar(P());Rechaza(()=>s.Guardar(P()));Igual(1,s.Consultar().Prestamos.Count);});
Caso("Sin existencias y stock negativo",()=>{var s=Nuevo("stock");var l=s.Consultar().Libros[0];l.Existencias=0;s.Guardar(l);Rechaza(()=>s.Guardar(P()));l.Existencias=-1;Rechaza(()=>s.Guardar(l));Igual(0,s.Consultar().Disponibles(1));});
Caso("No reducir stock bajo prestamos activos",()=>{var s=Nuevo("reducir");s.Guardar(P());var l=s.Consultar().Libros[0];l.Existencias=0;Rechaza(()=>s.Guardar(l));Igual(5,s.Consultar().Libros[0].Existencias);});
Caso("Restricciones de borrado",()=>{var s=Nuevo("borrar");Rechaza(()=>s.Eliminar<Autor>(1));Rechaza(()=>s.Eliminar<Editorial>(1));s.Guardar(P());Rechaza(()=>s.Eliminar<Libro>(1));Rechaza(()=>s.Eliminar<Usuario>(1));});
Caso("Fechas incoherentes y futuras",()=>{var s=Nuevo("fechas");var p=P();p.FechaPrestamo=DateTime.Today.AddDays(1);Rechaza(()=>s.Guardar(p));p=P();p.FechaLimite=DateTime.Today.AddDays(-1);Rechaza(()=>s.Guardar(p));int id=s.Guardar(P());Rechaza(()=>s.Devolver(id,DateTime.Today.AddDays(-1)));Rechaza(()=>s.Devolver(id,DateTime.Today.AddDays(1)));});
Caso("Editar prestamo activo; eliminar libera disponibilidad",()=>{var s=Nuevo("editarprestamo");int id=s.Guardar(P());var p=s.Consultar().Prestamos[0];p.IdLibro=2;s.Guardar(p);Igual(5,s.Consultar().Disponibles(1));Igual(2,s.Consultar().Disponibles(2));s.Eliminar<Prestamo>(id);Igual(3,s.Consultar().Disponibles(2));});
Caso("Devuelto no editable y puede eliminarse",()=>{var s=Nuevo("historial");int id=s.Guardar(P());s.Devolver(id,DateTime.Today);Rechaza(()=>s.Guardar(s.Consultar().Prestamos[0]));s.Eliminar<Prestamo>(id);Igual(0,s.Consultar().Prestamos.Count);});
Caso("Fallo no persiste cambios parciales",()=>{var s=Nuevo("rollback");var u=s.Consultar().Usuarios[0];u.Documento="";Rechaza(()=>s.Guardar(u));Igual("DEMO-001",s.Consultar().Usuarios[0].Documento);});
Caso("Conflicto de edicion de registro eliminado",()=>{var s=Nuevo("obsoleto");int id=s.Guardar(new Editorial("Temporal"));s.Eliminar<Editorial>(id);Rechaza(()=>s.Guardar(new Editorial("Cambio"){Id=id}));});
Caso("Concurrencia: un ejemplar no se presta dos veces",()=>{
 var s=Nuevo("concurrencia");var l=s.Consultar().Libros[0];l.Existencias=1;s.Guardar(l);int u=s.Guardar(new Usuario{Nombre="Otro",Apellido="Usuario",Documento="D2"});
 int ok=0,no=0;Parallel.ForEach(new[]{1,u},uid=>{try{Nuevo("concurrencia").Guardar(P(1,uid));Interlocked.Increment(ref ok);}catch(ReglaException){Interlocked.Increment(ref no);}});
 Igual(1,ok);Igual(1,no);Igual(0,s.Consultar().Disponibles(1));
});
Console.WriteLine($"RESULTADO: {exitos} aprobadas; {fallos} fallidas.");
try{Directory.Delete(dir,true);}catch(IOException){}
return fallos==0?0:1;

void PruebaSql(string cadena)
{
 var servicio=new BibliotecaServicio(new AlmacenSql(cadena));
 string marca=Guid.NewGuid().ToString("N")[..12];
 int autor=0,editorial=0,usuario=0,libro=0,prestamo=0;
 try
 {
  autor=servicio.Guardar(new Autor("Prueba O'Connor",marca));
  editorial=servicio.Guardar(new Editorial("Prueba SQL "+marca));
  usuario=servicio.Guardar(new Usuario{Nombre="Prueba",Apellido=marca,Documento="TEST-"+marca,Correo="prueba@example.com"});
  string baseIsbn="979"+Random.Shared.NextInt64(100000000,999999999).ToString();
  int suma=baseIsbn.Select((c,i)=>(c-'0')*(i%2==0?1:3)).Sum();
  string isbn=baseIsbn+((10-suma%10)%10).ToString();
  libro=servicio.Guardar(new Libro{Codigo="TEST-"+marca,ISBN=isbn,Titulo="Prueba SQL con comilla ' "+marca,IdAutor=autor,IdEditorial=editorial,Anio=2020,Existencias=1});
  prestamo=servicio.Guardar(new Prestamo{IdLibro=libro,IdUsuario=usuario,FechaPrestamo=DateTime.Today,FechaLimite=DateTime.Today.AddDays(14)});
  Igual(0,servicio.Consultar().Disponibles(libro));
  Rechaza(()=>servicio.Guardar(new Prestamo{IdLibro=libro,IdUsuario=usuario}));
  servicio.Devolver(prestamo,DateTime.Today);Igual(1,servicio.Consultar().Disponibles(libro));
  Igual("Devuelto",servicio.Consultar().Prestamos.Single(x=>x.Id==prestamo).Estado);
 }
 finally
 {
  // Solo se retiran los registros creados por esta prueba, en orden de FK.
  if(prestamo>0)servicio.Eliminar<Prestamo>(prestamo);
  if(libro>0)servicio.Eliminar<Libro>(libro);
  if(usuario>0)servicio.Eliminar<Usuario>(usuario);
  if(editorial>0)servicio.Eliminar<Editorial>(editorial);
  if(autor>0)servicio.Eliminar<Autor>(autor);
 }
}
