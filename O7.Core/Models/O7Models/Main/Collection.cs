using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.Models.O7Models.Main
{
    public class Collection : AuditModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int SeasonId { get; set; }
        public Season Season { get; set; }

        public ICollection<CollectionPhoto> CollectionPhotos { get; set; }
        public ICollection<CollectionProduct> CollectionProducts { get; set; }

    }
}
