using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Models.O7Models.Main
{
    public class CollectionProduct : AuditModel
    {
        public int Id { get; set; }
        public Collection Collection { get; set; }
        public int CollectionId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
