using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MvcWearIt.Data;
using MvcWearIt.Models;

[Authorize]
public class CarritoController : Controller
{
    private readonly MvcWearItContexto _context;

    public CarritoController(MvcWearItContexto context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmarPedido([FromBody] List<ItemCarrito> items)
    {
        if (items == null || items.Count == 0)
            return Json(new { exito = false, mensaje = "El carrito está vacío." });

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            return Json(new { exito = false, mensaje = "Usuario no autenticado." });

        var pedido = new Pedido
        {
            Fecha = DateTime.UtcNow,
            UserId = userId,
            UserEmail = email
        };
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        foreach (var item in items)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto == null) continue;

            _context.Detalles.Add(new Detalle
            {
                PedidoId = pedido.Id,
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                Precio = producto.Precio,
                Descuento = 0
            });
        }
        await _context.SaveChangesAsync();

        return Json(new { exito = true, mensaje = "Pedido confirmado correctamente." });
    }
}

public class ItemCarrito
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
}