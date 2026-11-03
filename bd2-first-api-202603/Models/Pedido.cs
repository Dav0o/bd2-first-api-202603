using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace bd2_first_api_202603.Models;

[Table("Pedido")]
[Index("IdCliente", Name = "IX_Pedido_IdCliente")]
public partial class Pedido
{
    [Key]
    public int IdPedido { get; set; }

    public DateTime FechaPedido { get; set; }

    public int IdCliente { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Estado { get; set; } = null!;

    [InverseProperty("IdPedidoNavigation")]
    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
}
