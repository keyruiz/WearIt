using System.ComponentModel.DataAnnotations;

namespace MvcWearIt.Models
{
    public class Pedido
    {
        [Display(Name = "Núm. Pedido")]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }

        public ICollection<Detalle> Detalles { get; set; } = new List<Detalle>();
    }
}
