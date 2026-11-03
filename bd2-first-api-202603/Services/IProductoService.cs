using bd2_first_api_202603.DTOs;
using bd2_first_api_202603.Models;

namespace bd2_first_api_202603.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<int> CrearAsync(ProductoCreateDto dto);
        Task ActualizarAsync(int id, ProductoUpdateDto dto);
        Task EliminarAsync(int id);
    }
}
