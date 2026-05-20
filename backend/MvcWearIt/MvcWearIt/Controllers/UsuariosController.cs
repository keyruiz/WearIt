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
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosController(
            ApplicationDbContext context,
            MvcWearItContexto negocioContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _negocioContext = negocioContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string rol)
        {
            var usuarios = await _context.Users.ToListAsync();
            var usuariosConRoles = new List<(IdentityUser User, bool EsAdmin)>();

            foreach (var user in usuarios)
            {
                var esAdmin = await _userManager.IsInRoleAsync(user, "Administrador");
                if (string.IsNullOrEmpty(rol) || rol == "Todos" ||
                    (rol == "Administrador" && esAdmin) ||
                    (rol == "Usuario" && !esAdmin))
                {
                    usuariosConRoles.Add((user, esAdmin));
                }
            }

            ViewBag.RolSeleccionado = rol ?? "Todos";
            return View(usuariosConRoles);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var esAdmin = await _userManager.IsInRoleAsync(user, "Administrador");
            ViewBag.EsAdmin = esAdmin;

            ViewBag.Pedidos = await _negocioContext.Pedidos
                .Where(p => p.UserId == id)
                .OrderByDescending(p => p.Fecha)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            return View(user);
        }

        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(string userName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Todos los campos son obligatorios.");
                return View();
            }

            var user = new IdentityUser { UserName = userName, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Administrador");
                TempData["SuccessMessage"] = $"Administrador '{userName}' creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
    }
}
