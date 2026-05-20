using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcWearIt.Data;
using MvcWearIt.Models;

namespace MvcWearIt.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PedidosController : Controller
    {
        private readonly MvcWearItContexto _context;
        private readonly ApplicationDbContext _identityContext;

        public PedidosController(MvcWearItContexto context, ApplicationDbContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
        }

        // GET: Pedidos
        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            var userIds = pedidos.Where(p => p.UserId != null).Select(p => p.UserId).Distinct().ToList();
            var usuarios = await _identityContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);

            ViewBag.Usuarios = usuarios;
            return View(pedidos);
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            if (pedido.UserId != null)
            {
                var user = await _identityContext.Users.FindAsync(pedido.UserId);
                ViewBag.UserEmail = user?.Email;
            }

            return View(pedido);
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}
