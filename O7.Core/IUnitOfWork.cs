using O7.Core.Interfaces.O7Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ICollectionRepository Collections { get; }
        ICollectionPhotoRepository CollectionPhotos { get; }
        ICollectionProductRepository CollectionProducts { get; }
        IColorRepository Colors { get; }
        IGenderRepository Genders { get; }
        IProductRepository Products { get; }
        IProductColorRepository ProductColors { get; }
        IProductColorImageRepository ProductColorImages { get; }
        IProductColorSizeRepository ProductColorSizes { get; }
        IProductTypeRepository ProductTypes { get; }
        ISeasonRepository Seasons { get; }
        ISizeRepository Sizes { get; }
        IStyleRepository Styles { get; }

        Task<bool> Complete();
    }
}
