using InventoryApi.Domain.DTOs;
using InventoryApi.Domain.Interfaces;
using InventoryApi.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public CategoriesController(IUnitOfWork uow) => _uow = uow;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            // unique name check
            if (_uow.Categories.Query().Any(c => c.Name == dto.Name)) return Conflict(new { message = "Category exists" });

            var cat = new Category { Name = dto.Name, Description = dto.Description };
            await _uow.Categories.AddAsync(cat);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = cat.Id }, cat);
        }

        [HttpGet]
        public IActionResult List()
        {
            var categories = _uow.Categories.Query()
                .Select(c => new {
                    c.Id,
                    c.Name,
                    c.Description,
                    ProductCount = c.Products.Count
                }).ToList();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _uow.Categories.Query().Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();
            return Ok(new { c.Id, c.Name, c.Description, Products = c.Products });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            var c = await _uow.Categories.GetByIdAsync(id);
            if (c == null) return NotFound();

            // unique name check (exclude current)
            if (_uow.Categories.Query().Any(x => x.Name == dto.Name && x.Id != id)) return Conflict();

            c.Name = dto.Name;
            c.Description = dto.Description;
            _uow.Categories.Update(c);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _uow.Categories.Query().Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();
            if (c.Products.Any()) return Conflict(new { message = "Category has linked products" });
            _uow.Categories.Remove(c);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }

}
