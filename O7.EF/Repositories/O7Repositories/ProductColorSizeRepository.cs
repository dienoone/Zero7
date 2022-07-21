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
    public class ProductColorSizeRepository : BaseRepository<ProductColorSize>, IProductColorSizeRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductColorSizeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductColorSize>> GetProductColorSizesAsync(int? proudctColorId, string businessId)
        {
            return await _context.ProductColorSizes
                .Include(e => e.Size)
                .Where(e => e.ProductColorId == proudctColorId && e.BusinessId == businessId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<ProductColorSize> GetProductColorSizeAsync(int? ProductColorSizeId, string businessId)
        {
            return await _context.ProductColorSizes
                .Include(e => e.Size)
                .FirstOrDefaultAsync(e => e.ProductColorId == ProductColorSizeId && e.BusinessId == businessId && !e.IsDeleted);
        }

    }
}
