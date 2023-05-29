using Epayment.Repositories;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public class TrangThaiHoSoService : ITrangThaiHoSoService
    {
        private readonly ITrangThaiHoSoRepository _repo;
        public TrangThaiHoSoService(ITrangThaiHoSoRepository repo){
            _repo = repo;
        }
        public ResponseTrangThaiHoSoViewModel GetTrangThaiHoSo()
        {
            var getTTHS = _repo.getTrangThaiHoSo();
            return getTTHS;
        }
    }
}