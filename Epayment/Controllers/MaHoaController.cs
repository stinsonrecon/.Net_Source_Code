using System;
using System.IO;
using BCXN.Models;
using BCXN.ViewModels;
using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EVNSample;
namespace Epayment.Controllers
{
    [Route("api/YeuCauTT")]
    [Authorize]
    public class MaHoaController : Controller
    {
        private readonly IMaHoaPGDService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        public MaHoaController(IMaHoaPGDService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }
        [HttpPost("Encrypt")]
        public ActionResult<Response> GetYeuCauChiTien()
        {
                // string directoryPath = @"D:\Works\hoctoantap.com\New Directory";
                // DirectoryInfo directory = new DirectoryInfo(directoryPath);
                // directory.Create();
            var nameFile = "TMP111981"+DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + DateTime.Now.Millisecond;
            string fileName = @"wwwroot\RequestTransferMoney\" +nameFile + ".csv";  
            var OutNameFile = "Sec_TMP111981"+DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + DateTime.Now.Millisecond;
            string OutFileName = @"wwwroot\RequestTransferMoney\" +OutNameFile + ".csv";  

            using (StreamWriter writer = new StreamWriter(fileName))  
            {  
                writer.WriteLine("Payment Serial Number:1");  
                writer.WriteLine("Transaction Type Code:BT"); 
                writer.WriteLine("Payment Type: Blank"); 
                writer.WriteLine("Customer Reference No: "); 
                writer.WriteLine("Debit Account No: "); 
                writer.WriteLine("Beneficiary Account No: "); 
                writer.WriteLine("Beneficiary Name: "); 
                writer.WriteLine("Document ID: "); 
                writer.WriteLine("Place of Issue: "); 
                writer.WriteLine("ID Issuance Date: "); 
                writer.WriteLine("Beneficiary Bank Swift Code / IFSC Code: Citad code"); 
                writer.WriteLine("Transaction Currency: VND"); 
                writer.WriteLine("Payment Amount: 100000 "); 
                writer.WriteLine("Charge Type: OUR"); 
                writer.WriteLine("Payment details: Test"); 
                writer.WriteLine("Purpose Code: 1"); 
                writer.WriteLine("Value Date: 06/12/2021"); 
                writer.WriteLine("Beneficiary ID: "); 
                writer.WriteLine("Beneficiary Addr: "); 
                writer.WriteLine("Beneficiary Addr: "); 
            }   
            //var resp = _service.Encrypt(@"wwwroot\fileChiTietGiayToHoSTT\test.txt", @"wwwroot\fileChiTietGiayToHoSTT\pub.key", @"wwwroot\fileLichSuChiTietGTHSTT\test_sign.txt");
            var resp = _service.Encrypt(fileName, @"wwwroot\fileChiTietGiayToHoSTT\pub.key", OutFileName);
            //var resp = _service.Encrypt("Temfile.sign", "EVNPublicRSA.asc", "Temfile.encrypt");
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }

        [HttpPost("DescryptFile")]
        public ActionResult<Response> DescryptFile()
        {
            PGPDecryptVerify.Decrypt(@"wwwroot\OutRequestTransferMoney\TMP111981_1000436_0000000983_14122021_14122021153846606_10.csv", @"wwwroot\RSAkeySign\bankSceretDSA.asc", "123", @"wwwroot\fileLichSuChiTietGTHSTT\Sce_9.decrypt");
            PGPDecryptVerify.Verify(@"wwwroot\fileLichSuChiTietGTHSTT\Sce_9.decrypt", @"wwwroot\RSAkeySign\EVNPublicRSA.asc", @"wwwroot\fileLichSuChiTietGTHSTT\ketqua_7.csv");


            var resp = _service.Decrypt(@"wwwroot\inPutRequestTransferMoney\ACK_TMP112000_10001089_0014447633_MD_18102021_274_120751897_003.decrypt", @"wwwroot/RSAkeySign/EVNSceretRSA.asc", "123", @"wwwroot\fileLichSuChiTietGTHSTT\Sce_.csv");
            //var resp = _service.Encrypt("Temfile.sign", "EVNPublicRSA.asc", "Temfile.encrypt");
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }
        [HttpPost]
        public ActionResult<Response> CreateYeuCauChiTienVCBK([FromBody]YeuCauChiTienParams request)
        {
            var resp = _service.CreateYeuCauChuyenKhoanVCB(request);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }
    }
}