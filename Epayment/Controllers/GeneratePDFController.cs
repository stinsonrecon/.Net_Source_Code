using Epayment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Epayment.Controllers
{
    [Route("api/")]
    [ApiController]
    public class GeneratePDFController : ControllerBase
    {
        private readonly IGeneratePDFService _service;

        public GeneratePDFController(IGeneratePDFService service)
        {
            _service = service;
        }

        [HttpGet("generateUyNhiemChi")]
        public object GenerateUyNhiemChi([FromQuery] Guid chungTuId)
        {
            var res = _service.GenerateUyNhiemChi(chungTuId, 1);
            return res;
        }

        [HttpGet("generatePhieuChi")]
        public object GeneratePhieuChi([FromQuery] Guid chungTuId)
        {
            var res = _service.GeneratePhieuChi(chungTuId, 1);
            return res;
        }

        [HttpGet("mergePCUNC")]
        public string MergePCUNC([FromQuery] Guid chungTuId)
        {
            var res = _service.MergePCUNC(chungTuId);
            return res;
        }

        [HttpGet("convertWordToPDF")]
        public object ConvertWordToPDF()
        {
            var res = _service.ConvertWordToPDF();
            return res;
        }
    }
}
