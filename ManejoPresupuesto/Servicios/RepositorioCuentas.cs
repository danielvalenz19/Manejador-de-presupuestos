using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
	public interface IRepositorioCuentas
	{
		Task Crear(Cuenta cuenta);
	}
	public class RepositorioCuentas : IRepositorioCuentas
	{
		private readonly string connectionString;
		public RepositorioCuentas(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public async Task Crear(Cuenta cuenta)
		{
			using var conection = new SqlConnection(connectionString);
			var id = await conection.QuerySingleAsync<int>(@"INSERT INTO Cuentas (Nombre,													  TipoCuentaId,Descripcion,Balance)
												VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance);
												SELECT SCOPE_IDENTITY();", cuenta);
			cuenta.Id = id;
		}
	}
}
