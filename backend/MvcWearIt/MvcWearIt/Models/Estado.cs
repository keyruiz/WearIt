using System.ComponentModel.DataAnnotations;

namespace MvcWearIt.Models
{
    public class Estado
    {
        public int Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "La descripción es un campo requerido.")]
        public string? Descripcion { get; set; } // Ej: "Pendiente", "Completado"

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
