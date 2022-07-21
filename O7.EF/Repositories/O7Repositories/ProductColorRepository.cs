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
    public class ProductColorRepository : BaseRepository<ProductColor>, IProductColorRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductColorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductColor>> GetProductColorsAsync(int? prodcutId, string businessId)
        {
            return await _context.ProductColors
                .Include(e => e.ProductColorSizes.Where(e => !e.IsDeleted)).ThenInclude(e => e.Size)
                .Include(e => e.Color)
                .Include(e => e.ProductColorImages)
                .Where(e => e.ProductId == prodcutId && !e.IsDeleted && e.BusinessId == businessId)
                .ToListAsync();
        }

        public async Task<ProductColor> GetProductColorAsync(int? prodcutColorId, string businessId)
        {
            return await _context.ProductColors
                .Include(e => e.ProductColorSizes.Where(e => !e.IsDeleted)).ThenInclude(e => e.Size)
                .Include(e => e.Color)
                .Include(e => e.ProductColorImages)
                .FirstOrDefaultAsync(e => e.Id == prodcutColorId && !e.IsDeleted && e.BusinessId == businessId);
        }

    }
}
