using AutoMapper;
using InventoryApi.Domain.DTOs;
using InventoryApi.Domain.Interfaces;
using InventoryApi.Infrastructure.Data.Models;

namespace InventoryApi.Application.Services
{
    public class ProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ProductService(IUnitOfWork uow, IMapper mapper, IWebHostEnvironment env)
        {
            _uow = uow;
            _mapper = mapper;
            _env = env;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            // Validate category exists
            var cat = await _uow.Categories.GetByIdAsync(dto.CategoryId);
            if (cat == null) throw new Exception("Category not found");

            var product = _mapper.Map<Product>(dto);

            // Handle image upload
            if (dto.Image != null)
            {
                var path = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
                Directory.CreateDirectory(path);
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(path, fileName);
                using var fs = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(fs);
                product.ImageUrl = "/images/" + fileName;
            }

            await _uow.Products.AddAsync(product);
            await _uow.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<(IEnumerable<ProductDto>, int)> GetFilteredAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, int page, int limit, string? search)
        {
            var (items, total) = await _uow.Products.GetFilteredAsync(categoryId, minPrice, maxPrice, page, limit, search);
            return (_mapper.Map<IEnumerable<ProductDto>>(items), total);
        }

        public async Task UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null) throw new Exception("Product not found");

            _mapper.Map(dto, product);

            if (dto.Image != null)
            {
                var path = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
                Directory.CreateDirectory(path);
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(path, fileName);
                using var fs = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(fs);
                product.ImageUrl = "/images/" + fileName;
            }

            _uow.Products.Update(product);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null) throw new Exception("Product not found");

            _uow.Products.Remove(product);
            await _uow.SaveChangesAsync();
        }
    }
}
