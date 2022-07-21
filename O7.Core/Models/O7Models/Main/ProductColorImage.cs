using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Models.O7Models.Main
{
    public class ProductColorImage : AuditModel
    {
        public int Id { get; set; }
        public ProductColor ProductColor { get; set; }
        public int ProductColorId { get; set; }
        public string Photo { get; set; }
        public bool IsActive { get; set; }
    }
}
