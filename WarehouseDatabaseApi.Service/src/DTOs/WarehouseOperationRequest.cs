using System.ComponentModel.DataAnnotations;

namespace WarehouseDatabaseApi.DTOs;

public class WarehouseOperationRequest
{
    [Required]
    public int IdProduct { get; set; }
    
    [Required]
    public int IdWarehouse { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 1.")]
    public int Amount { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public int IdOrder { get; set; }
}