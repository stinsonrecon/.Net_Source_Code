using System.Collections.Generic;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Epayment.ViewModels;

namespace Epayment.Repositories
{
    public interface ILichSuChiTietGiayToRepository
    {
        Task<ResponsePostViewModel> CreateLichSuChiTietGiayToHSTT (ParmChiTietGiayToHSTTViewModel request , List<string> url);
        ResponseLichSuChiTietGiayToHSTTViewModel GetLichSuChiTietGiayHSTTByHoSoId(string hoSoId , string giayToId);
    }
}