using bd2_first_api_202603.Data;
using bd2_first_api_202603.DTOs;
using bd2_first_api_202603.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace bd2_first_api_202603.Services
{
    public class ProductoService : IProductoService
    {
        private readonly TiendaDbContext _context;

        public ProductoService(TiendaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            return await _context.Productos
                .FromSqlRaw("EXEC dbo.sp_ObtenerProductos")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Productos
                .FromSqlInterpolated($"EXEC dbo.sp_ObtenerProductoPorId @IdProducto={id}")
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<int> CrearAsync(ProductoCreateDto dto)
        {
            var nombreParam = new SqlParameter("@Nombre", dto.Nombre);
            var precioParam = new SqlParameter("@Precio", dto.Precio);
            var stockParam = new SqlParameter("@Stock", dto.Stock);
            var idOutputParam = new SqlParameter
            {
                ParameterName = "@IdProducto",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_CrearProducto @Nombre, @Precio, @Stock, @IdProducto OUTPUT",
                    nombreParam, precioParam, stockParam, idOutputParam);
            }
            catch (SqlException ex)
            {
                // RAISERROR del SP (ej. nombre duplicado) llega aquí
                throw new InvalidOperationException(ex.Message, ex);
            }

            return (int)idOutputParam.Value;
        }

        public async Task ActualizarAsync(int id, ProductoUpdateDto dto)
        {
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"EXEC dbo.sp_ActualizarProducto @IdProducto={id}, @Nombre={dto.Nombre}, @Precio={dto.Precio}, @Stock={dto.Stock}");
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public async Task EliminarAsync(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"EXEC dbo.sp_EliminarProducto @IdProducto={id}");
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
    }
}
