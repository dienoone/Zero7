using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Models.O7Models.Main
{
    public class ProductColor : AuditModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ColorId { get; set; }
        public Color Color { get; set; }
        public string ProductImage { get; set; }
        public bool IsActive { get; set; }

        public ICollection<ProductColorImage> ProductColorImages { get; set; }
        public ICollection<ProductColorSize> ProductColorSizes { get; set; }
    }
}
