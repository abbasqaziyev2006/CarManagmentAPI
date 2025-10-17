using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarManagmentAPI.DataContext.Model
{
    public class Car
    {
        public int Id { get; set; }

        public required string Model { get; set; } = string.Empty;

        public required int Year { get; set; }

        public string? Color { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int BrandId { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }
    }
}
