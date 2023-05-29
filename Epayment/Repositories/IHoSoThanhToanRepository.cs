using System;
using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ModelRequest;
using Epayment.ViewModels;

namespace Epayment.Repositories
{
    public interface IHoSoThanhToanRepository
    {
        ResponsePostHSTTViewModel CreateHoSoTT (ParmHoSoThanhToanViewModel request);
        ResponsePostViewModel UpdateHoSoTT(ParmHoSoThanhToanViewModel request);
        ResponsePostViewModel DeleteHoSoTT(Guid id, string userID);
        List<HoSoThanhToanViewModel> GetHoSoTT();
        ResponseHoSoViewModel GetHoSoTTById(string id);
        ResponseHoSoViewModel GetHoSoPaging(HoSoSearchViewModel request);
        /// <summary>
        /// Duyệt hồ sơ, truyền vào ThaoTacBuocPheDuyetId để update TrangThaiHoSo động
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ResponsePostViewModel ApproveHoSo(ApproveHoSoTT request);
        ResponsePheDuyetHoSoTTViewModel GetPheDuyetHoSoTTById(string id);
        ResponseHoSoViewModel GetHoSoTTByMHSTT(string MaSoHoSoTT);
        ResponsePostViewModel UpDateTTHoSoTT(string MaSoHoSoTT, int TrangThai, DateTime NgayThanhToan);
        ResponseHoSoViewModel GetHoSoTTByCTId(Guid ChungTuId);
        List<ThongTinTaiKhoanByHoSoIdViewModel> GetThongTinTaiKhoanByHoSoId(Guid HoSoId);
        ResponsePostViewModel UpdateFileHoSoTT(CreateFileHoSoTT request);
        ResponsePaging GetAllHoSoThamChieu(HoSoThamChieuSearchViewModel request);
        ResponsePostViewModel UpdateTrangThaiHsttById(Guid id, int TrangThai);
        ResponsePostViewModel PhanCongXuLy(Epayment.ModelRequest.PhanCongXuLy request);
    }
}