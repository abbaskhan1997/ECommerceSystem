using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

   
    public string? ImageUrl { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public int CategoryId { get; set; }

    [ValidateNever]
    public Category? Category { get; set; }

    public int SchoolId { get; set; }

    [ValidateNever]
    public School? School { get; set; }

    public int? SchoolClassId { get; set; }

    [ValidateNever]
    public SchoolClass? SchoolClass { get; set; }
}