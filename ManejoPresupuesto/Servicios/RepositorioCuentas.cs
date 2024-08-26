﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
	public interface IRepositorioCuentas
	{
		Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
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
		public async Task<IEnumerable<Cuenta>> Buscar (int usuarioId)
		{
			using var conection = new SqlConnection(connectionString);
			return await conection.QueryAsync<Cuenta>(@"Select Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre AS																  TipoCuenta
																				FROM Cuentas
																				INNER JOIN TiposCuentas tc
																				ON tc.Id = Cuentas.TipoCuentaId
																				WHERE tc.UsuarioId = @UsuarioId
																				ORDER BY tc.Orden", new {usuarioId});
		}


	}
}
