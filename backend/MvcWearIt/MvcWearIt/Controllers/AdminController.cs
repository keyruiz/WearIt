using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcWearIt.Data;

namespace MvcWearIt.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _identityContext;
        private readonly MvcWearItContexto _negocioContext;

        public AdminController(ApplicationDbContext identityContext, MvcWearItContexto negocioContext)
        {
            _identityContext = identityContext;
            _negocioContext = negocioContext;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UsuariosCount = await _identityContext.Users.CountAsync();
            ViewBag.JuegosCount = await _negocioContext.Juegos.CountAsync();
            ViewBag.ProductosCount = await _negocioContext.Productos.CountAsync();
            ViewBag.PedidosCount = await _negocioContext.Pedidos.CountAsync();
            return View();
        }
    }
}
