using O7.Core.Interfaces.O7Interfaces;
using O7.Core.Models.O7Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.EF.Repositories.O7Repositories
{
    public class StyleRepository : BaseRepository<Style>, IStyleRepository
    {
        private readonly ApplicationDbContext _context;
        public StyleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
