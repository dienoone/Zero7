using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Models.O7Models.Main
{
    public class Product : AuditModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int StyleId { get; set; }
        public Style Style { get; set; }
        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }

        public ICollection<CollectionProduct> CollectionProducts { get; set; }
        public ICollection<ProductColor> ProductColors { get; set; }

    }
}
