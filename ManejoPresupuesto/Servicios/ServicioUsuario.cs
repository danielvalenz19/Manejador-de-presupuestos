namespace ManejoPresupuesto.Servicios
{

	public interface IserviciosUsuarios
	{
		int ObtenerUsuarioId();
	}
	public class ServicioUsuario : IserviciosUsuarios
	{

		public int ObtenerUsuarioId()
		{
			return 1;
		}

	}
}
