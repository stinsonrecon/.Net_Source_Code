using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Services;
using BCXN.Statics;
using BCXN.ViewModels;
using BCXN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCXN.Data;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http.Headers;

namespace BCXN.Controllers
{
    [Route("api/yeucaubaocao")]
    //[Authorize]
    public class YeuCauBaoCaoController : Controller
    {
        private readonly IYeuCauBaoCaoService _service;
        private readonly ITableauService _tableauservice;
        private readonly ILogger<YeuCauBaoCaoController> _logger;

        public YeuCauBaoCaoController(IYeuCauBaoCaoService service, ILogger<YeuCauBaoCaoController> logger, ITableauService tableauservice)
        {
            _service = service;
            _tableauservice = tableauservice;
            this._logger = logger;
        }

        [HttpGet]
        public ActionResult<ResponseYeuCauBaoCaoViewModel> GetYeuCauBaoCao([FromQuery] ParamsGetYeuCauBaoCaoViewModel request)
        {
            var resp = _service.GetYeuCauBaoCao(request);
            return Ok(resp);
        }

        [HttpGet("{donViId}/{ycbcId}")]
        public ActionResult<YeuCauBaoCaoViewModel> GetYeuCauBaoCaoById(int donViId, int ycbcId)
        {
            var resp = _service.GetYeuCauBaoCaoById(donViId, ycbcId);
            return Ok(resp);
        }

        [HttpPost]
        public ActionResult<ResponsePostViewModel> CreateYeuCauBaoCao([FromBody] YeuCauBaoCaoCreateViewModel request)
        {
            var resPost = _service.CreateYeuCauBaoCao(request);
            return resPost;
        }

        [HttpPut("{ycbcId}")]
        public ActionResult<ResponsePostViewModel> UpdateYeuCauBaoCao(int ycbcId, [FromBody] YeuCauBaoCaoCreateViewModel request)
        {
            var res = _service.UpdateYeuCauBaoCao(ycbcId, request);
            return res;
        }

        [HttpDelete("{ycbcId}")]
        public ActionResult<ResponsePostViewModel> DeleteYeuCauBaoCao(int ycbcId)
        {
            var res = _service.DeleteYeuCauBaoCao(ycbcId);
            return res;
        }

        [HttpPut("xacnhan/{xnbcId}")]
        public ActionResult<ResponsePostViewModel> XacNhanYeuCauBaoCao(int xnbcId, [FromBody] XacNhanBaoCaoViewModel request)
        {
            var res = _service.XacNhanYeuCauBaoCao(xnbcId, request);
            return res;
        }

        [HttpPut("trangthai/{ycbcId}/{trangThai}")]
        public ResponsePostViewModel UpdateTrangThai(int ycbcId, int trangThai)
        {
            var res = _service.UpdateTrangThai(ycbcId, trangThai);
            return res;
        }
        [HttpPut("CapnhatBC/{xnbcId}")]
        public ActionResult<ResponsePostViewModel> UpdateXNBC(int xnbcId, [FromBody] XacNhanBaoCaoViewModel request)
        {
            var res = _service.UpdateXNBC(xnbcId, request);
            return res;
        }

        [HttpPost("uploadFileBaoCaoHieuChinh/{ycbcId}"), DisableRequestSizeLimit]
        public ActionResult<Response> UploadFileBaoCaoHieuChinh(int ycbcId)
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot", "fileTableAu");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                if (file.Length > 0)
                {                    
                    var name = "";
                    for (int i = 0; i < fileName.Split('.').Count(); i++)
                    {
                        if (i < fileName.Split('.').Count() - 1)
                        {
                            name += fileName.Split('.')[i];
                        }
                    }
                    string renameFile = name + "_" + DateTime.Now.ToString("yyyy-MM-dd") + "." + fileName.Split('.').Last();
                    var fullPath = Path.Combine(pathToSave, renameFile);
                    var dbPath = Path.Combine(folderName, renameFile);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    dbPath = _tableauservice.ConvertPPTXToImage(renameFile);

                    var resp = _service.UpdateFileBaoCaoTongHopHieuChinh(ycbcId, dbPath);
                    if (resp.Success == false)
                    {
                        _logger.LogError($"[uploadFileBaoCaoHieuChinh] Không thể cập nhật db {fileName}");
                        return StatusCode(400, new Response(message: "Lỗi: Không thể cập nhật db", data: "", errorcode: "003", success: false));
                    }
                    return StatusCode(200, new Response(message: "Upload thành công", data: "", errorcode: "", success: true));
                }
                else
                {
                    _logger.LogError($"[uploadFileBaoCaoHieuChinh] File không có nội dung {fileName}");
                    return StatusCode(400, new Response(message: "Lỗi: File không có nội dung", data: "", errorcode: "001", success: false));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[uploadFileBaoCaoHieuChinh] Internal server error {ex.StackTrace}");
                return StatusCode(500, new Response(message: "Lỗi: Internal server error", data: ex.StackTrace, errorcode: "002", success: false));
            }
        }

        [HttpGet("getFileBaoCaoHieuChinh/{ycbcId}")]
        public ActionResult<Response> GetFileBaoCaoHieuChinh(int ycbcId)
        {
            var resp = _service.GetFileBaoCaoTongHopHieuChinh(ycbcId);
            if (resp.Success == true) return StatusCode(200, resp);
            else
            {
                if (resp.ErrorCode == "001") return StatusCode(404, resp);
                else return StatusCode(500, resp);
            };
        }
    }
}