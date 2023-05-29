using BCXN.Data;
using Epayment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Epayment.Repositories
{
    public class ThaoTacBuocPheDuyetRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public ThaoTacBuocPheDuyetRepository(ApplicationDbContext context, ILogger<ThaoTacBuocPheDuyetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public DbSet<ThaoTacBuocPheDuyet> GetDbQueryTable()
        {
            return _context.ThaoTacBuocPheDuyet;
        }

        public ApplicationDbContext GetDbContext()
        {
            return _context;
        }
    }
}
