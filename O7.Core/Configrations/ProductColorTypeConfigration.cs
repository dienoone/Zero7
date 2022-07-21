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
    public class ProductColorTypeConfigration : IEntityTypeConfiguration<ProductColor>
    {
        public void Configure(EntityTypeBuilder<ProductColor> builder)
        {
            builder
                .HasOne(b => b.Product)
                .WithMany(bl => bl.ProductColors)
                .HasForeignKey(bi => bi.ProductId);


            builder
                .HasOne(l => l.Color)
                .WithMany(bl => bl.ProductColors)
                .HasForeignKey(li => li.ColorId);
        }
    }
}
