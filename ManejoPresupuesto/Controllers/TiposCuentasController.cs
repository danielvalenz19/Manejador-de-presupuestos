using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;
using ManejoPresupuesto.Servicios;
namespace ManejoPresupuesto.Controllers
{
	public class TiposCuentasController : Controller
	{
		private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
		private readonly IserviciosUsuarios servicioUsuario;

		public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IserviciosUsuarios servicioUsuario)
		{
			this.repositorioTiposCuentas = repositorioTiposCuentas;
			this.servicioUsuario = servicioUsuario;
		}


		public IActionResult Crear()
		{

			return View();
		}

		public async Task<IActionResult> Index()
		{
			var usuarioId = servicioUsuario.ObtenerUsuarioId();
			var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
			return View(tiposCuentas);


		}


		[HttpPost]
		public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
		{

			if (!ModelState.IsValid)
			{
				return View(tipoCuenta);

			}

			tipoCuenta.UsuarioId = servicioUsuario.ObtenerUsuarioId();

			var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
			if (yaExisteTipoCuenta)
			{
				ModelState.AddModelError(nameof(tipoCuenta.Nombre),
					$"El nombre {tipoCuenta.Nombre} Ya existe.");
				return View(tipoCuenta);


			}


			await repositorioTiposCuentas.Crear(tipoCuenta);
			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<ActionResult> Editar(int id)
		{
			var usuarioId = servicioUsuario.ObtenerUsuarioId();//metodo dentro de otro servicio que toma el usuario para Guardarlo en una variable 
			var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);


			if (tipoCuenta is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

			return View(tipoCuenta);
		}

		[HttpPost]
		public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
		{
			var usuarioId = servicioUsuario.ObtenerUsuarioId();
			var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

			if (tipoCuentaExiste is null)
			{
				return RedirectToAction("NoENcontrado", "Home");
			}
			await repositorioTiposCuentas.Actualizar(tipoCuenta);
			return RedirectToAction("Index");

		}




		[HttpGet]
		public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
		{
			var usuarioId = servicioUsuario.ObtenerUsuarioId();
			var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);
			if (yaExisteTipoCuenta)
			{
				return Json($"El nombre {nombre} ya existe");

			}



			return Json(true);
		}

		public async Task<IActionResult> Borrar(int id)
		{
			var usuarioId = servicioUsuario.ObtenerUsuarioId();
			var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
			if (tipoCuenta is null)
			{
				return RedirectToAction("NoENcontrado", "Home");
			}
			return View(tipoCuenta);
		}
		[HttpPost]
		public async Task<IActionResult> BorrarTipoCuenta(int id)
		{

			var usuarioId = servicioUsuario.ObtenerUsuarioId();
			var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
			if (tipoCuenta is null)
			{
				return RedirectToAction("NoENcontrado", "Home");
			}
			await repositorioTiposCuentas.Borrar(id);
			return RedirectToAction("index");
		}

		[HttpPost]
		public async Task<IActionResult> Ordenar([FromBody] int[] ids)
		{
			var usuarioId = servicioUsuario.ObtenerUsuarioId();
			var tiposCuenta = await repositorioTiposCuentas.Obtener(usuarioId);
			var idsTipoCuentas = tiposCuenta.Select(x => x.Id);

			var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTipoCuentas).ToList();

			 if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
			{
				return Forbid();

			}

			var tiposCuentasOrdenados = ids.Select((Valor, indice) => new TipoCuenta{ Id = Valor, Orden = indice + 1 }).AsEnumerable();

			await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

			return Ok();
		}

	}
}
