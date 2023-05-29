using BCXN.Data;
using Epayment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Epayment.Repositories
{
    public class QuaTrinhPheDuyetRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public QuaTrinhPheDuyetRepository(ApplicationDbContext context, ILogger<QuaTrinhPheDuyetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public DbSet<QuaTrinhPheDuyet> GetDbQueryTable()
        {
            return _context.QuaTrinhPheDuyet;
        }

        public ApplicationDbContext GetDbContext()
        {
            return _context;
        }
    }
}
