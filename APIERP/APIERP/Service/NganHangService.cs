using APIERP.Repository;
using APIERP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIERP.Service
{
    public interface INganHangService 
    {
        Response Insert_Tai_Khoan_Ngan_Hang(TaiKhoanNganHangParams tk);
        Response Update_Tai_Khoan_Ngan_Hang_Nhan(TaiKhoanNganHangParams tk);
        Response Insert_Chi_Nhanh_Ngan_Hang(ChiNhanhNganHangParams cn);
        Response Insert_Ngan_Hang(NganHangParams nh);
    }
    public class NganHangService : INganHangService
    {
        private readonly INganHangRepository _repo;
        public NganHangService(INganHangRepository repo)
        {
            _repo = repo;
        }
        public Response Insert_Tai_Khoan_Ngan_Hang(TaiKhoanNganHangParams tk)
        {
            var res = _repo.Insert_Tai_Khoan_Ngan_Hang(tk);
            return res;
        }
        public Response Update_Tai_Khoan_Ngan_Hang_Nhan(TaiKhoanNganHangParams tk)
        {
            var res = _repo.Update_Tai_Khoan_Ngan_Hang_Nhan(tk);
            return res;
        }
        public Response Insert_Chi_Nhanh_Ngan_Hang(ChiNhanhNganHangParams cn)
        {
            var res = _repo.Insert_Chi_Nhanh_Ngan_Hang(cn);
            return res;
        }

        public Response Insert_Ngan_Hang(NganHangParams nh)
        {
            var res = _repo.Insert_Ngan_Hang(nh);
            return res;
        }
    }
}
