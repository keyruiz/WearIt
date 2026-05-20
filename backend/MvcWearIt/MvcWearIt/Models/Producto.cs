using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcWearIt.Models
{
    public class Producto
    {
       
        public int Id { get; set; }

        [Display(Name = "Nombre de la Skin")]
        [Required(ErrorMessage = "La descripción es un campo requerido.")]
        public string? Descripcion { get; set; } // Ej: "AK-47 | Asiimov"

        public string? Texto { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Precio { get; set; }

        [Display(Name = "Precio")]
        [RegularExpression(@"^[-0123456789]+[0-9.,]*$", ErrorMessage = "El valor introducido debe ser de tipo monetario.")]
        [Required(ErrorMessage = "El precio es un campo requerido")]
        public string PrecioCadena
        {
            get { return Convert.ToString(Precio).Replace(',', '.'); }
            set { Precio = Convert.ToDecimal(value.Replace('.', ',')); }
        }

        public string? Imagen { get; set; }

        // Relación con Categoría
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        // Relación con el Juego
        public int JuegoId { get; set; }
        public Juego? Juego { get; set; }

        // Atributos y Ventas
        public ICollection<Detalle> Detalles { get; set; } = new List<Detalle>();
        
    }
}
