using MathNet.Numerics;
using System.ComponentModel.DataAnnotations;
namespace CarManagmentAPI.DTOs.Car
{
    public class CarDto
    {
        public int Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }



    public class CreateCarDto
    {
        [Required(ErrorMessage = "Model is required")]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public int BrandId { get; set; }

        public IFormFile? Image { get; set; }
    }

    public class UpdateCarDto
    {
        [Required(ErrorMessage = "Model is required")]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public int BrandId { get; set; }

        public IFormFile? Image { get; set; }
    }
}
