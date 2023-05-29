using BCXN.Data;
using Epayment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Epayment.Repositories
{
    public class BuocPheDuyetRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public BuocPheDuyetRepository(ApplicationDbContext context, ILogger<BuocPheDuyetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public DbSet<BuocPheDuyet> GetDbQueryTable()
        {
            return _context.BuocPheDuyet;
        }

        public ApplicationDbContext GetDbContext()
        {
            return _context;
        }
    }
}
