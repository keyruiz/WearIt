using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcWearIt.Data;
namespace MvcWearIt.Controllers
{

    public class EscaparateController : Controller
    {
        private readonly MvcWearItContexto _context;

        public EscaparateController(MvcWearItContexto context)
        {
            _context = context;
        }

        // GET: /Escaparate?juegoId=3&id=5 (id es la categoría, ambos opcionales)
        public async Task<IActionResult> Index(int? juegoId, int? id)
        {
            if (juegoId == null)
            {
                return RedirectToAction("Index", "Juegos");
            }

            var categorias = await _context.Categorias
                .Where(c => c.Productos.Any(p => p.JuegoId == juegoId))
                .OrderBy(c => c.Descripcion)
                .ToListAsync();
            ViewData["ListaCategorias"] = categorias;

            ViewData["JuegoIdSeleccionado"] = juegoId;
            ViewData["CategoriaIdSeleccionada"] = id;

            return View();
        }

        // API JSON para tu JavaScript puro
        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(int juegoId, int? id)
        {
             var consulta = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Juego)
                .Where(p => p.JuegoId == juegoId);

            if (id == null)
            {
                return Json(await consulta.ToListAsync());
            }
            else
            {
                return Json(await consulta.Where(p => p.CategoriaId == id).ToListAsync());
            }
        }

        // GET: /Escaparate/Detalle/5
        public async Task<IActionResult> Detalle(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Juego)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();

            return View(producto);
        }
    }
}
