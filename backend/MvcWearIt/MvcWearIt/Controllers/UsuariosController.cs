using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcWearIt.Data;

namespace MvcWearIt.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MvcWearItContexto _negocioContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UsuariosController(
            ApplicationDbContext context,
            MvcWearItContexto negocioContext,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _negocioContext = negocioContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Users.ToListAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.Pedidos = await _negocioContext.Pedidos
                .Where(p => p.UserId == id)
                .OrderByDescending(p => p.Fecha)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            return View(user);
        }
    }
}
