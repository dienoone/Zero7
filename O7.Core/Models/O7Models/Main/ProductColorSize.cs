using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Models.O7Models.Main
{
    public class ProductColorSize : AuditModel
    {
        public int Id { get; set; }
        public int ProductColorId { get; set; }
        public ProductColor ProductColor { get; set; }
        public int SizeId { get; set; }
        public Size Size { get; set; }
        public bool IsActive { get; set; }
        public int Quantity { get; set; }
    }
}
