using CarManagmentAPI.DTOs.Car;
using System.ComponentModel.DataAnnotations;

namespace CarManagmentAPI.DTOs.Brand
{
    public class BrandDto
    {
        public int Id { get; set; }

        public required string Name { get; set; } = null!;

        public string? Country { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<CarDto> Cars { get; set; } = new List<CarDto>();
    }


    public class CreateBrandDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string? Country { get; set; }
    }


    public class UpdateBrandDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string? Country { get; set; }


    }
}
