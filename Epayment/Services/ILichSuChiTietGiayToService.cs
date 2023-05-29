using System.Collections.Generic;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public interface ILichSuChiTietGiayToService
    {
        Task<ResponsePostViewModel> CreateLichSuChiTietGiayToHSTT (ParmChiTietGiayToHSTTViewModel request , List<string> url);
        ResponseLichSuChiTietGiayToHSTTViewModel GetLichSuChiTietGiayHSTTByHoSoId(string hoSoId, string giayToId);
    }
}