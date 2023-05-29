using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public interface ILuongPheDuyetService
    {
        // ResponseLuongPheDuyetViewModel GetLuongPheDuyet();
        ResponsePostViewModel CreateLuongPheDuyet ( List<CreateLuongPheDuyet> request);
        ResponsePostViewModel UpdateLuongPheDuyet( List<CreateLuongPheDuyet> request);
        ResponseLuongPheDuyetViewModel GetLuongPheDuyet(string LoaiHoSoId);
        ResponsePostViewModel DeleteLuongPheDuyet(string loaiHoSoId, string luongPheDuyetId);
        // ResponsePostViewModel UpdateLuongPheDuyet(UpdateLuongPheDuyet request);
    }
}