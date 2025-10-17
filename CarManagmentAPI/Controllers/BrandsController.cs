using CarManagmentAPI.DataContext;
using CarManagmentAPI.DataContext.Model;
using CarManagmentAPI.DTOs.Brand;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BrandsController(AppDbContext context)
        {
            _context = context;
        }

      
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            try
            {
                var brands = await _context.Brands
                    .Include(b => b.Cars)
                    .Select(b => new BrandDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Country = b.Country,
                        CreatedAt = b.CreatedAt,
                    })
                    .ToListAsync();

                return Ok(brands);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving brands", error = ex.Message });
            }
        }

   
        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDto>> GetBrand(int id)
        {
            try
            {
                var brand = await _context.Brands
                    .Include(b => b.Cars)
                    .Where(b => b.Id == id)
                    .Select(b => new BrandDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Country = b.Country,
                        CreatedAt = b.CreatedAt,
                    })
                    .FirstOrDefaultAsync();

                if (brand == null)
                {
                    return NotFound(new { message = $"Brand with ID {id} not found" });
                }

                return Ok(brand);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving brand", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<BrandDto>> CreateBrand(CreateBrandDto createBrandDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var brand = new Brand
                {
                    Name = createBrandDto.Name,
                    Country = createBrandDto.Country,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();

                var brandDto = new BrandDto
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Country = brand.Country,
                    CreatedAt = brand.CreatedAt,
                };

                return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brandDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating brand", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, UpdateBrandDto updateBrandDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var brand = await _context.Brands.FindAsync(id);
                if (brand == null)
                {
                    return NotFound(new { message = $"Brand with ID {id} not found" });
                }

                brand.Name = updateBrandDto.Name;
                brand.Country = updateBrandDto.Country;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error updating brand - concurrency issue" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating brand", error = ex.Message });
            }
        }

     
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            try
            {
                var brand = await _context.Brands.FindAsync(id);
                if (brand == null)
                {
                    return NotFound(new { message = $"Brand with ID {id} not found" });
                }

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting brand", error = ex.Message });
            }
        }
    }
}