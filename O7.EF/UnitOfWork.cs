using O7.Core;
using O7.Core.Interfaces.O7Interfaces;
using O7.EF.Repositories.O7Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICollectionRepository Collections { get; private set; }
        public ICollectionPhotoRepository CollectionPhotos { get; private set; }
        public ICollectionProductRepository CollectionProducts { get; private set; }
        public IColorRepository Colors { get; private set; }
        public IGenderRepository Genders { get; private set; }
        public IProductRepository Products { get; private set; }
        public IProductColorRepository ProductColors { get; private set; }
        public IProductColorImageRepository ProductColorImages { get; private set; }
        public IProductColorSizeRepository ProductColorSizes { get; private set; }
        public IProductTypeRepository ProductTypes { get; private set; }
        public ISeasonRepository Seasons { get; private set; }
        public ISizeRepository Sizes { get; private set; }
        public IStyleRepository Styles { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Collections = new CollectionRepository(context);
            CollectionPhotos = new CollectionPhotoRepository(context);
            CollectionProducts = new CollectionProductRepository(context);
            Colors = new ColorRepository(context);
            Genders = new GenderRepository(context);
            Products = new ProductRepository(context);
            ProductColors = new ProductColorRepository(context);
            ProductColorImages = new ProductColorImageRepository(context);
            ProductColorSizes = new ProductColorSizeRepository(context);
            ProductTypes = new ProductTypeRepository(context);
            Seasons = new SeasonRepository(context);
            Sizes = new SizeRepository(context);
            Styles = new StyleRepository(context);
        }

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
