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

            if (_warehouseService.IsOrderCompleted(request.IdOrder))
            {
                return BadRequest("The order has already been completed");
            }
            
            if (!_warehouseService.UpdateOrderFulfilledAt(request.IdOrder))
            {
                return StatusCode(500, "An error occured while updating the order status.");
            }

            if (!_warehouseService.InsertProductWarehouseRecord(request.IdOrder, request.IdProduct, request.IdWarehouse,
                    request.Amount))
            {
                return StatusCode(500, "An error occured while inserting the product into the warehouse.");
            }
            
            return Ok("Product added to warehouse successfully.");
        }
    }  
}