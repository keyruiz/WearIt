using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcWearIt.Data;
using MvcWearIt.Models;

namespace MvcWearIt.Controllers
{
    public class JuegosController : Controller
    {
        private readonly MvcWearItContexto _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public JuegosController(MvcWearItContexto context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Juegos
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Administrador"))
                return RedirectToAction("Index", "Admin");

            return View(await _context.Juegos.ToListAsync());
        }

        // GET: Juegos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juego = await _context.Juegos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (juego == null)
            {
                return NotFound();
            }

            return View(juego);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AdminIndex()
        {
            return View(await _context.Juegos.ToListAsync());
        }

        // GET: Juegos/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Juegos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Slug,ImagenPortada,ColorHex")] Juego juego)
        {
            if (ModelState.IsValid)
            {
                _context.Add(juego);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminIndex));
            }
            return View(juego);
        }

        // GET: Juegos/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juego = await _context.Juegos.FindAsync(id);
            if (juego == null)
            {
                return NotFound();
            }
            return View(juego);
        }

        // POST: Juegos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Slug,ColorHex")] Juego juego, IFormFile imagen)
        {
            if (id != juego.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var juegoExistente = await _context.Juegos.FindAsync(id);
                    if (juegoExistente == null) return NotFound();

                    juegoExistente.Nombre = juego.Nombre;
                    juegoExistente.Slug = juego.Slug;
                    juegoExistente.ColorHex = juego.ColorHex;

                    if (imagen != null)
                    {
                        string strRutaImagenes = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes");
                        Directory.CreateDirectory(strRutaImagenes);
                        string strExtension = Path.GetExtension(imagen.FileName);
                        string strNombreFichero = "juego_" + juego.Id.ToString() + strExtension;
                        string strRutaFichero = Path.Combine(strRutaImagenes, strNombreFichero);

                        using (var fileStream = new FileStream(strRutaFichero, FileMode.Create))
                        {
                            await imagen.CopyToAsync(fileStream);
                        }

                        juegoExistente.ImagenPortada = strNombreFichero;
                    }

                    _context.Update(juegoExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JuegoExists(juego.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AdminIndex));
            }
            return View(juego);
        }

        // GET: Juegos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juego = await _context.Juegos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (juego == null)
            {
                return NotFound();
            }

            return View(juego);
        }

        // POST: Juegos/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var juego = await _context.Juegos.FindAsync(id);
            if (juego != null)
            {
                _context.Juegos.Remove(juego);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminIndex));
        }

        private bool JuegoExists(int id)
        {
            return _context.Juegos.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarImagen(int? id)
        {
            if (id == null) return NotFound();
            var juego = await _context.Juegos.FindAsync(id);
            if (juego == null) return NotFound();
            return View(juego);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarImagen(int? id, IFormFile imagen)
        {
            if (id == null) return NotFound();
            var juego = await _context.Juegos.FindAsync(id);
            if (juego == null) return NotFound();
            if (imagen == null)
            {
                TempData["ErrorMessage"] = "Debes seleccionar una imagen.";
                return RedirectToAction(nameof(CambiarImagen), new { id });
            }

            if (ModelState.IsValid)
            {
                string strRutaImagenes = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes");
                        Directory.CreateDirectory(strRutaImagenes);
                string strExtension = Path.GetExtension(imagen.FileName);
                string strNombreFichero = "juego_" + juego.Id.ToString() + strExtension;
                string strRutaFichero = Path.Combine(strRutaImagenes, strNombreFichero);

                using (var fileStream = new FileStream(strRutaFichero, FileMode.Create))
                {
                    await imagen.CopyToAsync(fileStream);
                }

                juego.ImagenPortada = strNombreFichero;
                try
                {
                    _context.Update(juego);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JuegoExists(juego.Id)) return NotFound();
                    else throw;
                }
            }
            return RedirectToAction(nameof(AdminIndex));
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var juegos = await _context.Juegos.ToListAsync();
            return Json(juegos);
        }

        // GET: Juegos/Productos/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Productos(int? id, int? categoriaId)
        {
            if (id == null) return NotFound();

            var juego = await _context.Juegos.FirstOrDefaultAsync(m => m.Id == id);
            if (juego == null) return NotFound();

            var productos = _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.JuegoId == id);

            if (categoriaId.HasValue)
                productos = productos.Where(p => p.CategoriaId == categoriaId.Value);

            ViewBag.Juego = juego;
            ViewBag.Categorias = new SelectList(
                await _context.Categorias.Where(c => c.Productos.Any(p => p.JuegoId == id)).ToListAsync(),
                "Id", "Descripcion", categoriaId);

            return View(await productos.ToListAsync());
        }
    }
}
