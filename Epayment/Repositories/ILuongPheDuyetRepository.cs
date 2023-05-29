using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.ViewModels;

namespace Epayment.Repositories
{
    public interface ILuongPheDuyetRepository
    {
        // ResponseLuongPheDuyetViewModel getLuongPheDuyet();
        ResponsePostViewModel CreateLuongPheDuyet (List<CreateLuongPheDuyet> request);
        ResponsePostViewModel UpdateLuongPheDuyet(List<CreateLuongPheDuyet> request);
        ResponseLuongPheDuyetViewModel getLuongPheDuyet(string LoaiHoSoId);
        ResponsePostViewModel deleteLuongPheDuyet(string LoaiHoSoId, string LuongPheDuyetId);
        // ResponsePostViewModel updateLuongPheDuyet(UpdateLuongPheDuyet request);
    }
}