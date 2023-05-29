using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Epayment.Repositories
{
    public class QuyTrinhPheDuyetRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public QuyTrinhPheDuyetRepository(ApplicationDbContext context, ILogger<QuyTrinhPheDuyetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public DbSet<QuyTrinhPheDuyet> GetDbQueryTable()
        {
            return _context.QuyTrinhPheDuyet;
        }

        public ApplicationDbContext GetDbContext()
        {
            return _context;
        }
    }
}
