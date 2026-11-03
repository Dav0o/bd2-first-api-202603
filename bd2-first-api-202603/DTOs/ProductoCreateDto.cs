using System.ComponentModel.DataAnnotations;

namespace bd2_first_api_202603.DTOs
{
    public class ProductoCreateDto
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
