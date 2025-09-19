using AutoMapper;
using InventoryApi.Domain.DTOs;
using InventoryApi.Domain.Interfaces;
using InventoryApi.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Application.Services
{
    public class CategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Category> CreateAsync(CreateCategoryDto dto)
        {
            if (_uow.Categories.Query().Any(c => c.Name == dto.Name))
                throw new Exception("Category already exists");

            var category = _mapper.Map<Category>(dto);
            await _uow.Categories.AddAsync(category);
            await _uow.SaveChangesAsync();
            return category;
        }

        public IEnumerable<object> GetAll()
        {
            return _uow.Categories.Query()
                .Include(c => c.Products)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    ProductCount = c.Products.Count
                })
                .ToList();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _uow.Categories.Query().Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) throw new Exception("Category not found");
            if (category.Products.Any()) throw new Exception("Category has products, cannot delete");

            _uow.Categories.Remove(category);
            await _uow.SaveChangesAsync();
        }
    }
}
