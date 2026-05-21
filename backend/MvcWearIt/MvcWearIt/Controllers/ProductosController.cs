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
    [Authorize(Roles = "Administrador")]
    public class ProductosController : Controller
    {
        private readonly MvcWearItContexto _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductosController(MvcWearItContexto context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Productos
        public async Task<IActionResult> Index(string search, int? juegoId)
        {
            IQueryable<Producto> consulta = _context.Productos.Include(p => p.Categoria).Include(p => p.Juego);

            if (!string.IsNullOrWhiteSpace(search))
                consulta = consulta.Where(p => p.Descripcion.Contains(search));

            if (juegoId.HasValue && juegoId.Value > 0)
                consulta = consulta.Where(p => p.JuegoId == juegoId.Value);

            ViewBag.Search = search;
            ViewBag.JuegoId = juegoId;
            ViewBag.Juegos = new SelectList(await _context.Juegos.OrderBy(j => j.Nombre).ToListAsync(), "Id", "Nombre", juegoId);
            return View(await consulta.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Juego)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion");
            ViewData["JuegoId"] = new SelectList(_context.Juegos, "Id", "Nombre");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Texto,PrecioCadena,CategoriaId,JuegoId")] Producto producto, IFormFile imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null)
                {
                    string strRutaImagenes = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes");
                    Directory.CreateDirectory(strRutaImagenes);
                    string strExtension = Path.GetExtension(imagen.FileName);

                    _context.Add(producto);
                    await _context.SaveChangesAsync();

                    string strNombreFichero = producto.Id.ToString() + strExtension;
                    string strRutaFichero = Path.Combine(strRutaImagenes, strNombreFichero);

                    using (var fileStream = new FileStream(strRutaFichero, FileMode.Create))
                    {
                        await imagen.CopyToAsync(fileStream);
                    }

                    producto.Imagen = strNombreFichero;
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Add(producto);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion", producto.CategoriaId);
            ViewData["JuegoId"] = new SelectList(_context.Juegos, "Id", "Nombre", producto.JuegoId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion", producto.CategoriaId);
            ViewData["JuegoId"] = new SelectList(_context.Juegos, "Id", "Nombre", producto.JuegoId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Texto,PrecioCadena,CategoriaId,JuegoId")] Producto producto, IFormFile imagen)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productoExistente = await _context.Productos.FindAsync(id);
                    if (productoExistente == null) return NotFound();

                    productoExistente.Descripcion = producto.Descripcion;
                    productoExistente.Texto = producto.Texto;
                    productoExistente.Precio = producto.Precio;
                    productoExistente.CategoriaId = producto.CategoriaId;
                    productoExistente.JuegoId = producto.JuegoId;

                    if (imagen != null)
                    {
                        string strRutaImagenes = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes");
                    Directory.CreateDirectory(strRutaImagenes);
                        string strExtension = Path.GetExtension(imagen.FileName);
                        string strNombreFichero = producto.Id.ToString() + strExtension;
                        string strRutaFichero = Path.Combine(strRutaImagenes, strNombreFichero);

                        using (var fileStream = new FileStream(strRutaFichero, FileMode.Create))
                        {
                            await imagen.CopyToAsync(fileStream);
                        }

                        productoExistente.Imagen = strNombreFichero;
                    }

                    _context.Update(productoExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Descripcion", producto.CategoriaId);
            ViewData["JuegoId"] = new SelectList(_context.Juegos, "Id", "Nombre", producto.JuegoId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Juego)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        // GET: Productos/CambiarImagen/5
        public async Task<IActionResult> CambiarImagen(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Productos/CambiarImagen/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarImagen(int? id, IFormFile imagen)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            if (imagen == null) return NotFound();

            if (ModelState.IsValid)
            {
                string strRutaImagenes = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes");
                    Directory.CreateDirectory(strRutaImagenes);
                string strExtension = Path.GetExtension(imagen.FileName);
                string strNombreFichero = producto.Id.ToString() + strExtension;
                string strRutaFichero = Path.Combine(strRutaImagenes, strNombreFichero);

                using (var fileStream = new FileStream(strRutaFichero, FileMode.Create))
                {
                    await imagen.CopyToAsync(fileStream);
                }

                producto.Imagen = strNombreFichero;
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id)) return NotFound();
                    else throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorJuego(int juegoId)
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.JuegoId == juegoId)
                .ToListAsync();
            return Json(productos);
        }

        // Ruta: /Productos/ObtenerCategorias
        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias()
        {
            var categorias = await _context.Categorias.ToListAsync();
            return Json(categorias);
        }
    }
}
