using O7.Core.Interfaces.O7Interfaces;
using O7.Core.Models.O7Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.EF.Repositories.O7Repositories
{
    public class GenderRepository : BaseRepository<Gender>, IGenderRepository
    {
        private readonly ApplicationDbContext _context;
        public GenderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
