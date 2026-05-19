using System.ComponentModel.DataAnnotations;

namespace MvcWearIt.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "La descripción es un campo requerido.")]
        public string? Descripcion { get; set; } // Ej: "Cuchillos", "Rifles"
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
