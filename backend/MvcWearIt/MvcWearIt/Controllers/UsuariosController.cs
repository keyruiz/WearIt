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

        public async Task<IActionResult> Index(string rol, string search)
        {
            var usuariosQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                usuariosQuery = usuariosQuery.Where(u =>
                    u.UserName.Contains(search) || u.Email.Contains(search));

            var usuarios = await usuariosQuery.ToListAsync();
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
            ViewBag.Search = search;
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

        public async Task<IActionResult> ChangeRole(string id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] = "No puedes cambiar tu propio rol.";
                return RedirectToAction(nameof(Index));
            }

            var esAdmin = await _userManager.IsInRoleAsync(user, "Administrador");
            ViewBag.EsAdmin = esAdmin;
            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.UserName;
            ViewBag.UserEmail = user.Email;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id, string nuevoRol)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] = "No puedes cambiar tu propio rol.";
                return RedirectToAction(nameof(Index));
            }

            if (nuevoRol != "Administrador" && nuevoRol != "Usuario")
            {
                TempData["ErrorMessage"] = "Rol no válido.";
                return RedirectToAction(nameof(Index));
            }

            var esAdmin = await _userManager.IsInRoleAsync(user, "Administrador");
            var esUsuario = await _userManager.IsInRoleAsync(user, "Usuario");

            if (nuevoRol == "Administrador" && esAdmin)
            {
                TempData["ErrorMessage"] = $"El usuario '{user.UserName}' ya es Administrador.";
                return RedirectToAction(nameof(Index));
            }

            if (nuevoRol == "Usuario" && !esAdmin)
            {
                TempData["ErrorMessage"] = $"El usuario '{user.UserName}' ya es Usuario.";
                return RedirectToAction(nameof(Index));
            }

            if (esAdmin)
                await _userManager.RemoveFromRoleAsync(user, "Administrador");
            if (esUsuario)
                await _userManager.RemoveFromRoleAsync(user, "Usuario");

            await _userManager.AddToRoleAsync(user, nuevoRol);

            TempData["SuccessMessage"] = $"Rol de '{user.UserName}' cambiado a {nuevoRol} correctamente.";
            return RedirectToAction(nameof(Index));
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
