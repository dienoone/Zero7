using Microsoft.EntityFrameworkCore;
using O7.Core.Interfaces.O7Interfaces;
using O7.Core.Models.O7Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.EF.Repositories.O7Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(string businessId)
        {
            return await _context.Products
                .Include(e => e.Style)
                .Include(e => e.ProductType)
                .Include(e => e.CollectionProducts)
                .Include(e => e.ProductColors).ThenInclude(e => e.Color)
                .Include(e => e.ProductColors).ThenInclude(e => e.ProductColorImages)
                .Include(e => e.ProductColors).ThenInclude(e => e.ProductColorSizes).ThenInclude(e => e.Size)
                .Where(e => e.BusinessId == businessId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<Product> GetProductAsync(int? productId, string businessId)
        {
            return await _context.Products
                .Include(e => e.Style)
                .Include(e => e.ProductType)
                .Include(e => e.CollectionProducts)
                .Include(e => e.ProductColors).ThenInclude(e => e.Color)
                .Include(e => e.ProductColors).ThenInclude(e => e.ProductColorImages)
                .Include(e => e.ProductColors).ThenInclude(e => e.ProductColorSizes).ThenInclude(e => e.Size)
                .FirstOrDefaultAsync(e => e.BusinessId == businessId && !e.IsDeleted && e.Id == productId);
        }

    }
}
