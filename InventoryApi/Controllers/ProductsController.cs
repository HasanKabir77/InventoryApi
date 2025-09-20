using AutoMapper;
using InventoryApi.Domain.DTOs;
using InventoryApi.Domain.Interfaces;
using InventoryApi.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork uow, IWebHostEnvironment env, IMapper mapper)
        {
            _uow = uow;
            _env = env;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromForm] CreateProductDto dto)
        {
            var cat = await _uow.Categories.GetByIdAsync(dto.CategoryId);
            if (cat == null) return BadRequest(new { message = "Category not found." });

            var product = _mapper.Map<Product>(dto);

            if (dto.Image != null)
            {
                var imagesPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
                Directory.CreateDirectory(imagesPath);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(imagesPath, fileName);
                using var fs = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(fs);
                product.ImageUrl = $"/images/{fileName}";
            }

            await _uow.Products.AddAsync(product);
            await _uow.SaveChangesAsync();

            var result = _mapper.Map<ProductDto>(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<object>> List([FromQuery] int? categoryId, [FromQuery] decimal? minPrice,
                                                     [FromQuery] decimal? maxPrice, [FromQuery] int page = 1,
                                                     [FromQuery] int limit = 10, [FromQuery] string? search = null)
        {
            var (items, total) = await _uow.Products.GetFilteredAsync(categoryId, minPrice, maxPrice, page, limit, search);
            var dtoItems = _mapper.Map<IEnumerable<ProductDto>>(items);

            return Ok(new { total, page, limit, items = dtoItems });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _uow.Products.Query()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null) return NotFound();

            var cat = await _uow.Categories.GetByIdAsync(dto.CategoryId);
            if (cat == null) return BadRequest(new { message = "Category not found." });

            _mapper.Map(dto, product);

            if (dto.Image != null)
            {
                var imagesPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
                Directory.CreateDirectory(imagesPath);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(imagesPath, fileName);
                using var fs = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(fs);
                product.ImageUrl = $"/images/{fileName}";
            }

            _uow.Products.Update(product);
            await _uow.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null) return NotFound();

            _uow.Products.Remove(product);
            await _uow.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<object>> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var (items, total) = await _uow.Products.GetFilteredAsync(null, null, null, page, limit, q);
            var dtoItems = _mapper.Map<IEnumerable<ProductDto>>(items);

            return Ok(new { total, page, limit, items = dtoItems });
        }
    }
}
