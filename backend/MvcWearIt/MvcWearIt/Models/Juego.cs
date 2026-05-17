using System.ComponentModel.DataAnnotations;

namespace MvcWearIt.Models
{
    public class Juego
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del juego es requerido.")]
        [Display(Name = "Nombre del Juego")]
        public string? Nombre { get; set; } // Ej: "Counter-Strike 2", "Valorant"

        [Required]
        public string? Slug { get; set; } // Ej: "cs2", "valorant" (para las URLs)

        [Display(Name = "Imagen de Portada")]
        public string? ImagenPortada { get; set; } // La foto para la tarjeta del inicio

        [Display(Name = "Color Corporativo (Hex)")]
        public string? ColorHex { get; set; } // Ej: "#FF4655"

        // Relaciones
        public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
