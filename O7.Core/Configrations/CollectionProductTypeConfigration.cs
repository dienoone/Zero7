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
    public class CollectionProductTypeConfigration : IEntityTypeConfiguration<CollectionProduct>
    {
        public void Configure(EntityTypeBuilder<CollectionProduct> builder)
        {
            builder
                .HasOne(b => b.Collection)
                .WithMany(bl => bl.CollectionProducts)
                .HasForeignKey(bi => bi.CollectionId);


            builder
                .HasOne(l => l.Product)
                .WithMany(bl => bl.CollectionProducts)
                .HasForeignKey(li => li.ProductId);

        }
    }
}
