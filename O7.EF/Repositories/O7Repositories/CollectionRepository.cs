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
    public class CollectionRepository : BaseRepository<Collection>, ICollectionRepository
    {
        private readonly ApplicationDbContext _context;
        public CollectionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Collection>> GetCollectionsWithPhotos(string businessId)
        {
            return await _context.Collections
                .Include(e => e.CollectionPhotos)
                .Where(e => e.BusinessId == businessId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<Collection> GetCollectionWithPhotos(int? collectionId, string businessId)
        {
            return await _context.Collections
                .Include(e => e.CollectionPhotos)
                .FirstOrDefaultAsync(e => e.BusinessId == businessId && !e.IsDeleted && e.Id == collectionId);
        }
    }
}
