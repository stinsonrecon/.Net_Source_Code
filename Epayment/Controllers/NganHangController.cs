using System.Collections.Generic;
using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BCXN.ViewModels;
using System;
using System.Security.Claims;

namespace Epayment.Controllers
{
    [Authorize]
    public class NganHangController: Controller
    {
        private readonly INganHangService _service;

        public NganHangController(INganHangService service)
        {
            _service = service;
        }
        
        [HttpGet("api/NgangHang")]
        public List<NganHangViewModel> GetNganHang(){
            var resPost = _service.GetNganHang();
            return resPost;
        }

        [HttpPost("api/NganHang/Create")]
        public ActionResult<Response> CreateNganHang([FromBody] NganHangParams request)
        {
            var resPost = _service.CreateNganHang(request);
            // if (resPost.Success == true) return StatusCode(201, resPost);
            // else return StatusCode(500, resPost);
            return resPost;
        }

        [HttpPost("api/ChiNhanhNganHang/Create")]
        public ActionResult<Response> CreateChiNhanhNganHang([FromBody] ChiNhanhNganHangParams request)
        {
            var resPost = _service.CreateChiNhanhNganHang(request);
            // if (resPost.Success == true) return StatusCode(201, resPost);
            // else return StatusCode(500, resPost);
            return resPost;
        }

        [HttpPost("api/TaiKhoanNganHang/Create")]
        public ActionResult<Response> CreateTaiKhoanNganHang([FromBody] TaiKhoanNganHangParams request)
        {
            request.NguoiTaoId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resPost = _service.CreateTaiKhoanNganHang(request);
            // if (resPost.Success == true) return StatusCode(201, resPost);
            // else return StatusCode(500, resPost);
            return resPost;
        }

        [HttpGet("api/nganHang/getAll")]
        public ResponseGetNganHang GetAllNganHang([FromQuery] NganHangPagination nganHangPagination)
        {
            var resPost = _service.GetAllNganHang(nganHangPagination);
            return resPost;
        }

        [HttpGet("api/chiNhanhNganHang/getAll")]
        public ResponseGetChiNhanh GetAllChiNhanh([FromQuery] ChiNhanhNganHangPagination chiNhanhNganHangPagination)
        {
            var resPost = _service.GetAllChiNhanh(chiNhanhNganHangPagination);
            return resPost;
        }

        [HttpGet("api/taiKhoanNganHang/getAll")]
        public ResponseGetTaiKhoanNganHang GetAllTaiKhoanNganHang([FromQuery] TaiKhoanNganHangPagination taiKhoanNganHangPagination)
        {
            var resPost = _service.GetAllTaiKhoanNganHang(taiKhoanNganHangPagination);
            return resPost;
        }

        [HttpPut("api/nganHang/update")]
        public Response UpdateNganHang([FromBody] NganHangViewModel nganHang)
        {
            var resp = _service.UpdateNganHang(nganHang);
            return resp;
        }

        [HttpPut("api/chiNhanhNganHang/update")]
        public Response UpdateChiNhanh([FromBody] ChiNhanhViewModel chiNhanh)
        {
            var resp = _service.UpdateChiNhanh(chiNhanh);
            return resp;
        }

        [HttpPut("api/taiKhoanNganHang/update")]
        public Response UpdateTaiKhoanNganHang([FromBody] TaiKhoanNganHangViewModel taiKhoanNganHang)
        {
            taiKhoanNganHang.NguoiTaoId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resp = _service.UpdateTaiKhoanNganHang(taiKhoanNganHang);
            return resp;
        }

        [HttpDelete("api/nganHang/delete")]
        public Response DeleteNganHang([FromQuery] Guid nganHangId)
        {
            var resp = _service.DeleteNganHang(nganHangId);
            return resp;
        }

        [HttpDelete("api/chiNhanhNganHang/delete")]
        public Response DeleteChiNhanh([FromQuery] Guid chiNhanhNganHangId)
        {
            var resp = _service.DeleteChiNhanh(chiNhanhNganHangId);
            return resp;
        }

        [HttpDelete("api/taiKhoanNganHang/delete")]
        public Response DeleteTaiKhoanNganHang([FromQuery] Guid taiKhoanNganHangId)
        {
            var resp = _service.DeleteTaiKhoanNganHang(taiKhoanNganHangId);
            return resp;
        }
        [HttpPut("api/taiKhoanNganHang/Approve")]
        public Response ApproveTaiKhoanNganHang([FromQuery] Guid taiKhoanNganHangId, int trangThai )
        {
            var resp = _service.ApproveTaiKhoanNganHang(taiKhoanNganHangId, trangThai);
            return resp;
        }
        // [HttpGet("api/quocGia/getAll")]
        // public List<QuocGiaViewModel> GetAllQuocGia()
        // {
        //     var resp = _service.GetAllQuocGia();
        //     return resp;
        // }
        // [HttpGet("api/tinhTp/{quocGiaId}")]
        // public List<TinhThanhPhoViewModel> GetTinhTpByQuocGiaId(Guid quocGiaId)
        // {
        //     var resp = _service.GetTinhTPByQuocGiaId(quocGiaId);
        //     return resp;
        // }
    }
}