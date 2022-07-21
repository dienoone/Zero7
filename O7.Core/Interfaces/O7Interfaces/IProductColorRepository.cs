using O7.Core.Models.O7Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Interfaces.O7Interfaces
{
    public interface IProductColorRepository : IBaseRepository<ProductColor>
    {
        Task<IEnumerable<ProductColor>> GetProductColorsAsync(int? prodcutId, string businessId);
        Task<ProductColor> GetProductColorAsync(int? prodcutColorId, string businessId);
    }
}
