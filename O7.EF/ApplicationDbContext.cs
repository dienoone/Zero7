using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using O7.Core.Configrations;
using O7.Core.Models;
using O7.Core.Models.O7Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // Tables:
        #region Main:
        public DbSet<Style> Styles { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Gender> Genders { get; set; }
        #endregion

        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionPhoto> CollectionPhotos { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CollectionProduct> CollectionProducts { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductColorImage> ProductColorImages { get; set; }
        public DbSet<ProductColorSize> ProductColorSizes { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            // Many To Many Relations:
            builder.ApplyConfigurationsFromAssembly(typeof(CollectionProductTypeConfigration).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ProductColorTypeConfigration).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ProductColorSizeTypeConfigration).Assembly);

        }
    }
}
