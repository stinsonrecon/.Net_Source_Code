using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using BCXN.ViewModels;
using Epayment.Repositories;
using Epayment.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Epayment.Services
{
    public interface INganHangService
    {
        List<NganHangViewModel> GetNganHang();
        Response CreateNganHang(NganHangParams nganhang);
        Response CreateChiNhanhNganHang(ChiNhanhNganHangParams chinhanhnganhang);
        Response CreateTaiKhoanNganHang(TaiKhoanNganHangParams taikhoannganhang);
        ResponseGetNganHang GetAllNganHang(NganHangPagination nganHangPagination);
        ResponseGetChiNhanh GetAllChiNhanh(ChiNhanhNganHangPagination chiNhanhNganHangPagination);
        ResponseGetTaiKhoanNganHang GetAllTaiKhoanNganHang(TaiKhoanNganHangPagination taiKhoanNganHangPagination);
        Response UpdateNganHang(NganHangViewModel nganHang);
        Response UpdateChiNhanh(ChiNhanhViewModel chiNhanh);
        Response UpdateTaiKhoanNganHang(TaiKhoanNganHangViewModel taiKhoanNganHang);
        Response DeleteNganHang(Guid nganHangId);
        Response DeleteChiNhanh(Guid chiNhanhNganHangId);
        Response DeleteTaiKhoanNganHang(Guid taiKhoanNganHangId);
        Response ApproveTaiKhoanNganHang(Guid taiKhoanNganHangId, int trangThai);
        // List<QuocGiaViewModel> GetAllQuocGia();
        // List<TinhThanhPhoViewModel> GetTinhTPByQuocGiaId(Guid quocGiaId);
    }

    public class NganHangService:INganHangService
    {
        private readonly INganHangRepository _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public NganHangService(INganHangRepository repo, IMapper mapper, IConfiguration configuration)
        {
            _repo = repo;
            _mapper = mapper;
            _configuration = configuration;
        }

        public List<NganHangViewModel> GetNganHang()
        {
            var ret = _repo.GetNganHang();
            return ret;
        }

        public Response CreateNganHang(NganHangParams nganhang)
        {
            var resp = _repo.CreateNganHang(nganhang);
            if (resp.Success == true)
            {
                try
                {
                    Dictionary<string, object> payload = new Dictionary<string, object> {
                        { "TenNganHang" , nganhang.TenNganHang }
                    };
                    HttpClient client = new HttpClient();
                    var resultResponse = client.PostAsync(
                        _configuration["ErpSettings:InsertChiNhanhNganHangAddress"],
                        new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                    );
                    if (resultResponse.Result.Content != null)
                    {
                        resultResponse.Wait();
                        var erpResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                        dynamic erpResponseJson = JToken.Parse(erpResponseString);
                        if (erpResponseJson.Success == false) return new Response(message: "Lỗi: Chưa đồng bộ được chi nhánh ngân hàng tới erp", data: "", errorcode: "sv001", success: false);
                    }
                    else
                    {
                        return new Response(message: "Lỗi: Chưa đồng bộ được chi nhánh ngân hàng tới erp", data: "", errorcode: "sv002", success: false);
                    }
                }
                catch (Exception e)
                {

                }
            }

            return resp;
        }

        public Response CreateChiNhanhNganHang(ChiNhanhNganHangParams chinhanhnganhang)
        {
            var resp = _repo.CreateChiNhanhNganHang(chinhanhnganhang);
            if (resp.Success == true)
            {
                try
                {
                    Dictionary<string, object> payload = new Dictionary<string, object> {
                        { "TenChiNhanh" , chinhanhnganhang.TenChiNhanh }
                    };
                    HttpClient client = new HttpClient();
                    var resultResponse = client.PostAsync(
                        _configuration["ErpSettings:InsertChiNhanhNganHangAddress"],
                        new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                    );
                    if (resultResponse.Result.Content != null)
                    {
                        resultResponse.Wait();
                        var erpResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                        dynamic erpResponseJson = JToken.Parse(erpResponseString);
                        if (erpResponseJson.Success == false) return new Response(message: "Lỗi: Chưa đồng bộ được chi nhánh ngân hàng tới erp", data: "", errorcode: "sv001", success: false);
                    }
                    else
                    {
                        return new Response(message: "Lỗi: Chưa đồng bộ được chi nhánh ngân hàng tới erp", data: "", errorcode: "sv002", success: false);
                    }
                }
                catch (Exception e)
                {

                }
            }
            return resp;
        }

        public Response CreateTaiKhoanNganHang(TaiKhoanNganHangParams taikhoannganhang)
        {
            var resp = _repo.CreateTaiKhoanNganHang(taikhoannganhang);
            return resp;
        }

        public ResponseGetNganHang GetAllNganHang(NganHangPagination nganHangPagination)
        {
            var resp = _repo.GetAllNganHang(nganHangPagination);
            return resp;
        }

        public ResponseGetChiNhanh GetAllChiNhanh(ChiNhanhNganHangPagination chiNhanhNganHangPagination)
        {
            var resp = _repo.GetAllChiNhanh(chiNhanhNganHangPagination);
            return resp;
        }

        public ResponseGetTaiKhoanNganHang GetAllTaiKhoanNganHang(TaiKhoanNganHangPagination taiKhoanNganHangPagination)
        {
            var resp = _repo.GetAllTaiKhoanNganHang(taiKhoanNganHangPagination);
            return resp;
        }

        public Response UpdateNganHang(NganHangViewModel nganHang)
        {
            var resp = _repo.UpdateNganHang(nganHang);
            return resp;
        }

        public Response UpdateChiNhanh(ChiNhanhViewModel chiNhanh)
        {
            var resp = _repo.UpdateChiNhanh(chiNhanh);
            return resp;
        }

        public Response UpdateTaiKhoanNganHang(TaiKhoanNganHangViewModel taiKhoanNganHang)
        {
            var resp = _repo.UpdateTaiKhoanNganHang(taiKhoanNganHang);
            return resp;
        }

        public Response DeleteNganHang(Guid nganHangId)
        {
            var resp = _repo.DeleteNganHang(nganHangId);
            return resp;
        }

        public Response DeleteChiNhanh(Guid chiNhanhNganHangId)
        {
            var resp = _repo.DeleteChiNhanh(chiNhanhNganHangId);
            return resp;
        }

        public Response DeleteTaiKhoanNganHang(Guid taiKhoanNganHangId)
        {
            var resp = _repo.DeleteTaiKhoanNganHang(taiKhoanNganHangId);
            return resp;
        }
        public Response ApproveTaiKhoanNganHang(Guid taiKhoanNganHangId, int trangThai)
        {
             var resp = _repo.ApproveTaiKhoanNganHang(taiKhoanNganHangId, trangThai );
            return resp;
        }

        // public List<QuocGiaViewModel> GetAllQuocGia()
        // {
        //     var resp = _repo.GetAllQuocGia();
        //     return resp;
        // }

        // public List<TinhThanhPhoViewModel> GetTinhTPByQuocGiaId(Guid quocGiaId)
        // {
        //     var resp = _repo.GetTinhTPByQuocGiaId(quocGiaId);
        //     return resp;
        // }
    }
}