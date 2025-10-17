using System.ComponentModel.DataAnnotations;

namespace CarManagmentAPI.DataContext.Model
{
    public class Brand
    {
        public int Id { get; set; }

        public required string Name { get; set; } = string.Empty;

        public string? Country { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
