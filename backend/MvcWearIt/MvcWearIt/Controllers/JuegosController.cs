using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public JuegosController(MvcWearItContexto context)
        {
            _context = context;
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Slug,ImagenPortada,ColorHex")] Juego juego)
        {
            if (id != juego.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(juego);
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
