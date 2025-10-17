using CarManagmentAPI.DataContext;
using CarManagmentAPI.DataContext.Model;
using CarManagmentAPI.DTOs.Car;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024;

        public CarsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarDto>>> GetCars()
        {
            try
            {
                var cars = await _context.Cars
                    .Include(c => c.Brand)
                    .Select(c => new CarDto
                    {
                        Id = c.Id,
                        Model = c.Model,
                        Year = c.Year,
                        Color = c.Color,
                        Price = c.Price,
                        BrandId = c.BrandId,
                        BrandName = c.Brand!.Name,
                        ImagePath = c.ImagePath,
                        CreatedAt = c.CreatedAt
                    })
                    .ToListAsync();

                return Ok(cars);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving cars", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetCar(int id)
        {
            try
            {
                var car = await _context.Cars
                    .Include(c => c.Brand)
                    .Where(c => c.Id == id)
                    .Select(c => new CarDto
                    {
                        Id = c.Id,
                        Model = c.Model,
                        Year = c.Year,
                        Color = c.Color,
                        Price = c.Price,
                        BrandId = c.BrandId,
                        BrandName = c.Brand!.Name,
                        ImagePath = c.ImagePath,
                        CreatedAt = c.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (car == null)
                {
                    return NotFound(new { message = $"Car with ID {id} not found" });
                }

                return Ok(car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving car", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CarDto>> CreateCar([FromForm] CreateCarDto createCarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var brandExists = await _context.Brands.AnyAsync(b => b.Id == createCarDto.BrandId);
                if (!brandExists)
                {
                    return BadRequest(new { message = "Invalid brand ID" });
                }

                var car = new Car
                {
                    Model = createCarDto.Model,
                    Year = createCarDto.Year,
                    Color = createCarDto.Color,
                    Price = createCarDto.Price,
                    BrandId = createCarDto.BrandId,
                    CreatedAt = DateTime.UtcNow
                };


                if (createCarDto.Image != null)
                {
                    var uploadResult = await SaveImageAsync(createCarDto.Image);
                    if (!uploadResult.Success)
                    {
                        return BadRequest(new { message = uploadResult.Message });
                    }
                    car.ImagePath = uploadResult.FilePath;
                }

                _context.Cars.Add(car);
                await _context.SaveChangesAsync();


                await _context.Entry(car).Reference(c => c.Brand).LoadAsync();

                var carDto = new CarDto
                {
                    Id = car.Id,
                    Model = car.Model,
                    Year = car.Year,
                    Color = car.Color,
                    Price = car.Price,
                    BrandId = car.BrandId,
                    BrandName = car.Brand!.Name,
                    ImagePath = car.ImagePath,
                    CreatedAt = car.CreatedAt
                };

                return CreatedAtAction(nameof(GetCar), new { id = car.Id }, carDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating car", error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, [FromForm] CreateCarDto updateCarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var car = await _context.Cars.FindAsync(id);
                if (car == null)
                {
                    return NotFound(new { message = $"Car with ID {id} not found" });
                }


                var brandExists = await _context.Brands.AnyAsync(b => b.Id == updateCarDto.BrandId);
                if (!brandExists)
                {
                    return BadRequest(new { message = "Invalid brand ID" });
                }

                car.Model = updateCarDto.Model;
                car.Year = updateCarDto.Year;
                car.Color = updateCarDto.Color;
                car.Price = updateCarDto.Price;
                car.BrandId = updateCarDto.BrandId;


                if (updateCarDto.Image != null)
                {

                    if (!string.IsNullOrEmpty(car.ImagePath))
                    {
                        DeleteImage(car.ImagePath);
                    }

                    var uploadResult = await SaveImageAsync(updateCarDto.Image);
                    if (!uploadResult.Success)
                    {
                        return BadRequest(new { message = uploadResult.Message });
                    }
                    car.ImagePath = uploadResult.FilePath;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error updating car - concurrency issue" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating car", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                var car = await _context.Cars.FindAsync(id);
                if (car == null)
                {
                    return NotFound(new { message = $"Car with ID {id} not found" });
                }

                if (!string.IsNullOrEmpty(car.ImagePath))
                {
                    DeleteImage(car.ImagePath);
                }

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting car", error = ex.Message });
            }
        }

        private async Task<(bool Success, string? FilePath, string? Message)> SaveImageAsync(IFormFile file)
        {
            try
            {
                if (file.Length > _maxFileSize)
                {
                    return (false, null, "File size exceeds 5MB limit");
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return (false, null, "Invalid file type. Only JPG, JPEG, and PNG are allowed");
                }

                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return (true, $"/uploads/{uniqueFileName}", null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Error saving file: {ex.Message}");
            }
        }

        private void DeleteImage(string imagePath)
        {
            try
            {
                var filePath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}