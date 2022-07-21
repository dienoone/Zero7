using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using O7.Core.Models.O7Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Configrations
{
    public class ProductColorSizeTypeConfigration : IEntityTypeConfiguration<ProductColorSize>
    {
        public void Configure(EntityTypeBuilder<ProductColorSize> builder)
        {
            builder
                .HasOne(b => b.ProductColor)
                .WithMany(bl => bl.ProductColorSizes)
                .HasForeignKey(bi => bi.ProductColorId);


            builder
                .HasOne(l => l.Size)
                .WithMany(bl => bl.ProductColorSizes)
                .HasForeignKey(li => li.SizeId);

        }
    }
}
