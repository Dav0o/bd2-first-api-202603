using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace bd2_first_api_202603.Models;

[Table("Producto")]
[Index("Nombre", Name = "UQ_Producto_Nombre", IsUnique = true)]
public partial class Producto
{
    [Key]
    public int IdProducto { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
}
