using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcWearIt.Data;

namespace MvcWearIt.Controllers
{
    [Authorize]
    public class MisPedidosController : Controller
    {
        private readonly MvcWearItContexto _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MisPedidosController(MvcWearItContexto context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var pedidos = await _context.Pedidos
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Fecha)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            return View(pedidos);
        }
    }
}
