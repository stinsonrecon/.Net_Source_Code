using BCXN.Data;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Epayment.Repositories
{
    public class TrangThaiHoSoRepository : ITrangThaiHoSoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TrangThaiHoSoRepository> _logger;
        public TrangThaiHoSoRepository(ApplicationDbContext context, ILogger<TrangThaiHoSoRepository> logger){
            _context = context;
            _logger = logger;
        }
        public ResponseTrangThaiHoSoViewModel getTrangThaiHoSo()
        { 
            try
            {
                 var trangThaiHoSo = from tths in _context.TrangThaiHoSo
                                    select new TrangThaiHoSoViewModel {
                                        TrangThaiHoSoId = tths.TrangThaiHoSoId,
                                        TenTrangThaiHoSo = tths.TenTrangThaiHoSo,
                                        GiaTri = tths.GiaTri,
                                        TrangThai = tths.TrangThai
                                    };
                    if(trangThaiHoSo.Any())
                    {
                        var totalRecord = trangThaiHoSo.ToList().Count();
                        return new ResponseTrangThaiHoSoViewModel(trangThaiHoSo.ToList(), 200, totalRecord);
                    }
                    else{
                        return new ResponseTrangThaiHoSoViewModel(null, 204, 0);
                    }
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }
    }
}