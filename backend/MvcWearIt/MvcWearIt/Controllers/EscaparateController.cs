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
                .Where(p => p.JuegoId == juegoId);

            if (id == null)
            {
                var destacados = await consulta.Where(p => p.Escaparate == true).ToListAsync();
                return Json(destacados);
            }
            else
            {
                var porCategoria = await consulta.Where(p => p.CategoriaId == id).ToListAsync();
                return Json(porCategoria);
            }
        }
    }
}
