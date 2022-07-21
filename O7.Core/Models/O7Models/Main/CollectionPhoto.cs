using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Models.O7Models.Main
{
    public class CollectionPhoto : AuditModel
    {
        public int Id { get; set; }
        public bool IsFavorite { get; set; }
        public string Photo { get; set; }
        public bool IsActive { get; set; }

        public int CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
