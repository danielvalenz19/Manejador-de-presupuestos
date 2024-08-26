using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace ManejoPresupuesto.Controllers
{
	public class CuentasController : Controller
	{
		private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
		private readonly IserviciosUsuarios servicioUsuario;
		private readonly IRepositorioCuentas repositorioCuentas;

		public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IserviciosUsuarios servicioUsuario, IRepositorioCuentas repositorioCuentas)
		{
			this.repositorioTiposCuentas = repositorioTiposCuentas;
			this.servicioUsuario = servicioUsuario;
			this.repositorioCuentas = repositorioCuentas;
		}

		public async Task <IActionResult> Index()
		{
			var usuarioId = servicioUsuario.ObtenerUsuarioId();
			var cuentaConTipoCuenta = await repositorioCuentas.Buscar(usuarioId);
			var modelo = cuentaConTipoCuenta.GroupBy(x => x.TipoCuenta).Select(grupo => new IndiceCuentasViewModel
			{
				TipoCuenta = grupo.Key,
				Cuentas = grupo.AsEnumerable(),
			}).ToList();
			return View(modelo);
		}


		[HttpGet]
		public async Task<IActionResult> Crear()
		{
			var usuarioId = servicioUsuario.ObtenerUsuarioId();
			var tiposCuentas = await ObtenerTiposCuentas(usuarioId);

			var modelo = new CuentaCreacionViewModel
			{
				TiposCuentas = tiposCuentas
			};

			return View(modelo);
		}

		[HttpPost]
		public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
		{
			var usuarioId = servicioUsuario.ObtenerUsuarioId();
			var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

			if (tipoCuenta is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

			if (!ModelState.IsValid)
			{
				cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
				return View(cuenta);
			}

			await repositorioCuentas.Crear(cuenta);
			return RedirectToAction("Index");
		}

		private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
		{
			var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
			return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
		}

	}
}
