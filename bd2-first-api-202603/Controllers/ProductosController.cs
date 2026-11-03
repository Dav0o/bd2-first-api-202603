using bd2_first_api_202603.DTOs;
using bd2_first_api_202603.Models;
using bd2_first_api_202603.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bd2_first_api_202603.Controllers
{
    [ApiController]
    [Route("api/productos")]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET /api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetAll()
        {
            var productos = await _productoService.ObtenerTodosAsync();
            return Ok(productos);
        }

        // GET /api/productos/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto is null)
                return NotFound(new { mensaje = $"El producto con Id {id} no existe." });

            return Ok(producto);
        }

        // POST /api/productos
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductoCreateDto dto)
        {
            try
            {
                var nuevoId = await _productoService.CrearAsync(dto);
                var creado = await _productoService.ObtenerPorIdAsync(nuevoId);
                return CreatedAtAction(nameof(GetById), new { id = nuevoId }, creado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT /api/productos/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductoUpdateDto dto)
        {
            try
            {
                await _productoService.ActualizarAsync(id, dto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE /api/productos/{id}  (baja lógica, Activo = 0)
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _productoService.EliminarAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
