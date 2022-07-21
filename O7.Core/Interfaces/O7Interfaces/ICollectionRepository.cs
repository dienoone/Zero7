using O7.Core.Models.O7Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Interfaces.O7Interfaces
{
    public interface ICollectionRepository : IBaseRepository<Collection>
    {
        Task<IEnumerable<Collection>> GetCollectionsWithPhotos(string businessId);
        Task<Collection> GetCollectionWithPhotos(int? collectionId, string businessId);
    }
}
