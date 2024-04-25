using Microsoft.AspNetCore.Mvc;
using WarehouseDatabaseApi.DTOs;
using WarehouseDatabaseApi.Services;

namespace WarehouseDatabaseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseService _warehouseService;

        public WarehouseController(WarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpPost]
        public IActionResult AddProductToWarehouse([FromBody] WarehouseOperationRequest request)
        {
            if (!_warehouseService.ProductExists(request.IdProduct))
            {
                return NotFound("Product does not exist.");
            }

            if (!_warehouseService.WarehouseExists(request.IdWarehouse))
            {
                return NotFound("Warehouse does not exist.");
            }

            if (!_warehouseService.PurchaseOrderExists(request.IdProduct, request.Amount, request.CreatedAt))
            {
                return BadRequest("No matching purchase order found, or the order date is not valid.");
            }
            
            return Ok("Product added to warehouse successfully.");
        }
    }  
}