using System.ComponentModel.DataAnnotations;
using WarehouseAPI.Models;

namespace WarehouseAPI.DTOs.Operations;

public class OperationCreateDto
{
	[Required(ErrorMessage = "Тип операции обязателен")]
	public OperationType Type { get; set; }  // Income / Sale / Transfer / WriteOff

	public int? CounterpartyId { get; set; }  // поставщик при приходе, клиент при продаже

	[MaxLength(500)]
	public string? Comment { get; set; }

	[Required]
	[MinLength(1, ErrorMessage = "Операция должна содержать хотя бы один товар")]
	public List<OperationItemDto> Items { get; set; } = new();
}