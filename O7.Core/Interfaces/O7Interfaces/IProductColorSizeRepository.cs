using O7.Core.Models.O7Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Interfaces.O7Interfaces
{
    public interface IProductColorSizeRepository : IBaseRepository<ProductColorSize>
    {
        Task<IEnumerable<ProductColorSize>> GetProductColorSizesAsync(int? proudctColorId, string businessId);
        Task<ProductColorSize> GetProductColorSizeAsync(int? ProductColorSizeId, string businessId);
    }
}
