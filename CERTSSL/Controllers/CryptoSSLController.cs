using CERTSSL.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace CERTSSL.Controllers
{
    public class CryptoSSLController : Controller
    {
        [HttpPost]
        public JsonResult SignData([System.Web.Http.FromBody] string data)
        {
            try
            {
                var rng2 = new Crypto();
                //string ret2 = rng.SignData(Data, Directory.GetCurrentDirectory() + @"\Key\EVN2512.pfx");
                //string ret = rng2.SignData(data, Server.MapPath(@"~\Key\EVN2512.pfx"));
                string ret = rng2.SignData(data, Server.MapPath(@"~") + System.Configuration.ConfigurationManager.AppSettings["privateKeyEVN"], System.Configuration.ConfigurationManager.AppSettings["passPharse"]);
                return Json(new ResponseResult("Hop le", ret, "00", true));
            }
            catch (Exception ex)
            {
                return Json(new ResponseResult(ex.Message, false, "02", false));
            }
        }
        [HttpPost]
        public JsonResult SignDataSHA256([System.Web.Http.FromBody] string data)
        {
            try
            {
                var rng2 = new Crypto();
                rng2.HASH_ALGORITHM = "SHA256";
                //string ret2 = rng.SignData(Data, Directory.GetCurrentDirectory() + @"\Key\EVN2512.pfx");
                //string ret = rng2.SignData(data, Server.MapPath(@"~\Key\EVN2512.pfx"));
                string ret = rng2.SignDataSHA256(data, Server.MapPath(@"~") + System.Configuration.ConfigurationManager.AppSettings["privateKeyEVN"], System.Configuration.ConfigurationManager.AppSettings["passPharse"]);
                return Json(new ResponseResult("Hop le", ret, "00", true));
            }
            catch (Exception ex)
            {
                return Json(new ResponseResult(ex.Message, false, "02", false));
            }
        }

        [HttpPost]
        public JsonResult SignDataXML()
        {
            try
            {
                var rng2 = new Crypto();
                //string ret2 = rng.SignData(Data, Directory.GetCurrentDirectory() + @"\Key\EVN2512.pfx");
                //string ret = rng2.SignData(data, Server.MapPath(@"~\Key\EVN2512.pfx"));
                //XElement booksFromFile = XElement.Load(@"books.xml");
                XmlDocument doc = new XmlDocument();
                string pathFile = Server.MapPath(@"~/Files/ChungTu.xml");

                string fileSign = Guid.NewGuid().ToString();
                string pathfileSign = Server.MapPath(@"~/Files/") + fileSign + ".xml";
                string ret = rng2.SignDataXML(pathFile, pathfileSign, Server.MapPath(@"~") + System.Configuration.ConfigurationManager.AppSettings["privateKeyEVN"], System.Configuration.ConfigurationManager.AppSettings["passPharse"]);
                return Json(new ResponseResult("Hop le", ret, "00", true));
            }
            catch (Exception ex)
            {
                return Json(new ResponseResult(ex.Message, false, "02", false));
            }
        }
        [HttpPost]
        public JsonResult SignDataXML2()
        {
            try
            {
                var rng2 = new Crypto();             
                XmlDocument doc = new XmlDocument();
                string pathFile = Server.MapPath(@"~/Files/ChungTu.xml");

                string fileSign = Guid.NewGuid().ToString();
                string pathfileSign = Server.MapPath(@"~/Files/") + fileSign + ".xml";
                string ret = rng2.SignDataXML2(pathFile, pathfileSign, Server.MapPath(@"~") + System.Configuration.ConfigurationManager.AppSettings["privateKeyEVN"], System.Configuration.ConfigurationManager.AppSettings["passPharse"]);
                return Json(new ResponseResult("Hop le", ret, "00", true));
            }
            catch (Exception ex)
            {
                return Json(new ResponseResult(ex.Message, false, "02", false));
            }
        }

        [HttpPost]
        public JsonResult VerifyDataXML([System.Web.Http.FromBody] string file)
        {
            try
            {
                var rng2 = new Crypto();
                XmlDocument doc = new XmlDocument();
                string pathFile = Server.MapPath(@"~/Files/"+ file);

                string fileSign = Guid.NewGuid().ToString();
                string pathfileSign = Server.MapPath(@"~/Files/") + fileSign + ".xml";
                //bool ret = rng2.VerifyDataXml(pathFile, Server.MapPath(@"~") + System.Configuration.ConfigurationManager.AppSettings["publicKeyEVN"]);
                bool ret = rng2.VerifyDataXml(pathFile, Server.MapPath(@"~") + @"files\20220121154425.cer");
                return Json(new ResponseResult("Hop le", ret, "00", true));
            }
            catch (Exception ex)
            {
                return Json(new ResponseResult(ex.Message, false, "02", false));
            }
        }
        [HttpPost]
        public JsonResult VerifyData1([System.Web.Http.FromBody] DataSign input)
        {
            try
            {
                var rng2 = new Crypto();
                rng2.HASH_ALGORITHM = "SHA256";
                rng2.CHECK_EXPIRE_DATE = true;
                bool ret = rng2.VerifyData(input.signature, input.hash, Server.MapPath(@"~") + System.Configuration.ConfigurationManager.AppSettings["publicKeyEVN"]);

                if (ret)
                    return Json(new ResponseResult("Hop le", ret, "00", true));
                else
                    return Json(new ResponseResult("Chu ky khong hop le", ret, "01", false));
            }
            catch (Exception ex)
            {
                return Json(new ResponseResult(ex.Message, false, "02", false));
            }
        }
        [HttpPost]
        public JsonResult VerifyDataVTB([System.Web.Http.FromBody] DataSign input)
        {
            try
            {
                var rng2 = new Crypto();
                
                bool ret = rng2.VerifyData(input.signature, input.hash, Server.MapPath(@"~") + System.Configuration.ConfigurationManager.AppSettings["publicKeyVTB"]);

                if (ret)
                    return Json(new ResponseResult("Hop le", ret, "00", true));
                else
                    return Json(new ResponseResult("Chu ky khong hop le", ret, "01", false));
            }
            catch (Exception ex)
            {
                return Json(new ResponseResult(ex.Message, false, "02", false));
            }
        }
        [HttpPost]
        public JsonResult VerifyData([System.Web.Http.FromBody] DataSign input)
        {
            try
            {
                string filenamecert = System.Configuration.ConfigurationManager.AppSettings["publicKey" + input.bankcode];
                var rng2 = new Crypto();
                bool ret = rng2.VerifyData(input.signature, input.hash, Server.MapPath(@"~") + @"Key\" + filenamecert);
                if (ret)
                    return Json(new ResponseResult("Hop le", ret, "00", true));
                else
                    return Json(new ResponseResult("Chu ky khong hop le", ret, "01", false));
            }
            catch (Exception ex)
            {
                return Json(new ResponseResult(ex.Message, false, "02", false));
            }
        }
        [HttpPost]
        public JsonResult VerifyDataSHA256([System.Web.Http.FromBody] DataSign input)
        {
            try
            {
                string filenamecert = System.Configuration.ConfigurationManager.AppSettings["publicKey" + input.bankcode];
                var rng2 = new Crypto();
                rng2.HASH_ALGORITHM = "SHA256";
                rng2.CHECK_EXPIRE_DATE = true;
                bool ret = rng2.VerifyData(input.signature, input.hash, Server.MapPath(@"~") + @"Key\" + filenamecert);
                if (ret)
                    return Json(new ResponseResult("Hop le", ret, "00", true));
                else
                    return Json(new ResponseResult("Chu ky khong hop le", ret, "01", false));
            }
            catch (Exception ex)
            {
                return Json(new ResponseResult(ex.Message, false, "02", false));
            }
        }
        [HttpPost]
        public string RemoveString([System.Web.Http.FromBody] string input)
        {
            string ret = Regex.Replace(input, @"[@$\^}{.\n')(<>\]\[|]+", "");
            return ret;
        }
        [HttpPost]
        public string Sign([System.Web.Http.FromBody] string input)
        {
            string ret = Regex.Replace(input, @"[^0-9a-zA-Z)(,;.+\]\[: ]+", "");
            return ret;
        }
    }
}
