using System.ComponentModel.DataAnnotations;

namespace MvcWearIt.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "La descripción es un campo requerido.")]
        public string? Descripcion { get; set; } // Ej: "Cuchillos", "Rifles"

        // Relación con el Juego
        public int JuegoId { get; set; }
        public Juego? Juego { get; set; }

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
