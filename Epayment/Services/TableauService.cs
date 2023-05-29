using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BCXN.Repositories;
using BCXN.Models;
using BCXN.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Xml;
using Microsoft.Extensions.Logging;
using System.Web.Helpers;
using Aspose.Slides;
using System.Drawing;
using NLog.Fluent;
using Aspose.Slides.Warnings;
using System.Threading.Tasks.Dataflow;

namespace BCXN.Services
{
    public class TableauService : ITableauService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IBaoCaoRepository _baocaoRepo;
        private readonly ITableAuRepository _tableauRepo;
        private readonly Data.ApplicationDbContext _context;
        private readonly ILogger<TableauService> _logger;

        public TableauService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, ILogger<TableauService> logger, IBaoCaoRepository baocaoRepo, Data.ApplicationDbContext context, ITableAuRepository tableauRepo)
        {
            this._configuration = configuration;
            this._webHostEnvironment = webHostEnvironment;
            this._logger = logger;
            this._baocaoRepo = baocaoRepo;
            this._tableauRepo = tableauRepo;
            this._context = context;
        }
        public OutputDataSignIn GetTokenTableau(string sitename)
        {
            string tokenUser = "LSVJ4eJPQq-vPxUgSozVGA|2xYOx4hCfpYgxJvrsQdXx0yS4xrQao63";
            //string siteId = _report.SiteId;// "55471f10-74de-45bf-b789-f0f56d4f4917";
            //string workbookId = _report.WorkbookId; //"560480ce-2153-4b00-9532-e9877200b6e7";

            var strUrlTableau = _configuration["TableauSetting:urlServer"];
            InputDataSignIn inputPostData = new InputDataSignIn();
            Site site = new Site();
            site.contentUrl = sitename;// "GIAOBAN_EVN";
            Credentials credentials = new Credentials();
            credentials.name = _configuration["TableauSetting:username"];// "BIEVN";
            credentials.password = _configuration["TableauSetting:password"];// "Evn@12345!";
            credentials.site = site;
            inputPostData.credentials = credentials;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(strUrlTableau + "api/3.7/auth/signin");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var stringPostDataJson = JsonConvert.SerializeObject(inputPostData);
                var httpContent = new StringContent(stringPostDataJson, Encoding.UTF8, "application/json");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var httpResponse = client.PostAsync(strUrlTableau + "api/3.7/auth/signin", httpContent);
                httpResponse.Wait();
                if (httpResponse.Result.Content != null)
                {
                    var responseContent = httpResponse.Result.Content.ReadAsStringAsync();
                    httpResponse.Wait();

                    OutputDataSignIn output = new OutputDataSignIn();
                    output = JsonConvert.DeserializeObject<OutputDataSignIn>(responseContent.Result);
                    return output;
                    //if (output != null && output.credentials != null && output.credentials.token != null)
                    //{
                    //    tokenUser = output.credentials.token;
                    //}

                }
            }
            return null;
        }
        public object GetTicketTableau(string sitename)
        {
            var strUrlTableau = _configuration["TableauSetting:urlServer"];
            var strClientTrustIP = _configuration["TableauSetting:clientTrustIP"];
            var strUsername = _configuration["TableauSetting:username"];

            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
                string apiDownload = strUrlTableau + "trusted?username=" + strUsername + "&client_ip=" + strClientTrustIP + "&target_site=" + sitename;
                var httpContent = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(apiDownload, httpContent).Result;
                var retticket = response.Content.ReadAsStringAsync();
                if (retticket != null)
                {
                    var ticket = retticket.Result;
                    return new { ticket };
                }

                else
                    return "-1";
            }

        }
        public void DownloadAutoBaoCaoTableau()
        {
            _logger.LogInformation("Bat dau job: " + DateTime.Now.ToString());
            OutputDataSignIn objTicket = this.GetTokenTableau("");

            if (objTicket != null && objTicket.credentials != null && objTicket.credentials.token != null && objTicket.credentials.token != "")
            {
                _logger.LogInformation("Da co ticket: " + objTicket.credentials.token);
                ResponseBaoCaoViewModel res = _baocaoRepo.GetBaoCaoTableau();
                if (res != null && res.Data != null && res.Data.Count > 0)
                {
                    _logger.LogInformation("Danh sach bao cao: " + res.Data.Count);
                    foreach (BaoCaoViewModel baocao in res.Data)
                    {
                        _logger.LogInformation("Bat dau download: " + baocao.TenBaoCao);
                        object pathfile = this.DownloadTableau(baocao.SiteId, baocao.ViewId, objTicket.credentials.token);
                        if (pathfile != null && pathfile.ToString() != "")
                        {
                            _logger.LogInformation("Download bao cao: " + baocao.TenBaoCao + ", file down: " + pathfile);
                            _logger.LogInformation("Update du lieu bao cao:" + baocao.TenBaoCao + ", BaoCaoId: " + baocao.Id + " Khong co du lieu");
                            _baocaoRepo.UpdateBaoCaoAuto(baocao.Id, "//fileTableAu//" + pathfile);

                        }
                        else
                            _logger.LogInformation("Khong download duoc bao cao " + baocao.TenBaoCao);
                    }
                }
                else
                    _logger.LogInformation("Khong co bao cao");
            }
            else
            {
                _logger.LogInformation("Ticket khong hop le");
            }
            _logger.LogInformation("Ket thuc job: " + DateTime.Now.ToString());
        }
        public string DownloadTableau(string siteId, string viewId, string token)
        {
            try
            {
                if (token != null && token != "")
                    using (var clientDownload = new HttpClient())
                    {
                        var strUrlTableau = _configuration["TableauSetting:urlServer"];
                        clientDownload.DefaultRequestHeaders.Add("X-Tableau-Auth", token);

                        //string apiDownload = strUrlTableau + "api/3.7/sites/" + siteId + "/views/" + viewId + "/pdf?maxAge=60";
                        string apiDownload = strUrlTableau + "api/3.7/sites/" + siteId + "/views/" + viewId + "/image";
                        HttpResponseMessage response = clientDownload.GetAsync(apiDownload).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string contentRootPath = @"wwwroot\fileTableAu";
                            string filename = viewId + "_" + DateTime.Now.ToString("YYMMDDhhMMss") + ".jpg";
                            string file = Path.Combine(contentRootPath, filename);
                            if (!Directory.Exists(contentRootPath))
                            {
                                Directory.CreateDirectory(contentRootPath);
                            }
                            byte[] fileBytes = response.Content.ReadAsByteArrayAsync().Result;

                            if (System.IO.File.Exists(file))
                            {
                                try
                                {
                                    System.IO.File.Delete(file);
                                }
                                catch (System.IO.IOException e)
                                {
                                    _logger.LogError("Loi xoa file: " + e.Message);
                                    Console.WriteLine(e.Message);

                                }
                            }
                            System.IO.File.WriteAllBytes(file, fileBytes);
                            string filenamepptx = filename.Replace(".jpg", ".pptx");
                            ConvertImageToPPTX(filename, "");
                            return filename;
                        }

                    }
            }
            catch (Exception ex)
            {
                _logger.LogError("Loi tai file: " + ex.Message);
            }

            return null;
        }

        public object GetView(string siteId, string viewname, string token)
        {
            if (token != null && token != "")
                using (var clientDownload = new HttpClient())
                {
                    try
                    {
                        var strUrlTableau = _configuration["TableauSetting:urlServer"];
                        clientDownload.DefaultRequestHeaders.Add("X-Tableau-Auth", token);
                        string apiDownload = strUrlTableau + "api/3.7/sites/" + siteId + "/views?filter=viewUrlName:eq:" + viewname;
                        HttpResponseMessage response = clientDownload.GetAsync(apiDownload).Result;
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(response.Content.ReadAsStringAsync().Result);
                        //XmlNodeList nodeList = doc.SelectNodes("/tsResponse/views/view");
                        if (doc != null && doc.ChildNodes != null && doc.ChildNodes.Count > 1 && doc.ChildNodes[1].ChildNodes[1] != null && doc.ChildNodes[1].ChildNodes.Count > 1 && doc.ChildNodes[1].ChildNodes[1].ChildNodes != null && doc.ChildNodes[1].ChildNodes[1].ChildNodes.Count > 0)
                        {
                            string viewid = doc.ChildNodes[1].ChildNodes[1].ChildNodes[0].Attributes["id"].Value;
                            return new { viewid };
                        }

                        //foreach (XmlNode xn in nodeList)
                        //{
                        //    return xn.Attributes["id"].Value;
                        //}
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex.Message);
                    }

                }

            return null;
        }

        public string ConvertPPTXToImage(string filenamePPTX)
        {
            try
            {
                //string webRootPath = _webHostEnvironment.WebRootPath;
                string contentRootPath = @"wwwroot\fileTableAu";

                using (Aspose.Slides.Presentation pres = new Aspose.Slides.Presentation(Path.Combine(contentRootPath, filenamePPTX)))
                {
                    // Define dimensions
                    int desiredX = 1200;
                    int desiredY = 800;
                    // Get scaled values of X and Y
                    float ScaleX = (float)(1.0 / pres.SlideSize.Size.Width) * desiredX;
                    float ScaleY = (float)(1.0 / pres.SlideSize.Size.Height) * desiredY;

                    if (pres.Slides.Count > 0)
                    {
                        // Create a full scale image
                        Bitmap bmp = pres.Slides[0].GetThumbnail(ScaleX, ScaleY);
                        // Save the image to disk in JPEG format
                        string filenameimg = filenamePPTX.ToLower().Replace(".pptx", ".jpg");
                        string filename = Path.Combine(contentRootPath, filenameimg);
                        bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return filename;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }


        public string ConvertImageToPPTX(string fileImg, string templatePPTX)
        {
            try
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                //string contentRootPath = webRootPath + "\\fileTableAu";
                string contentRootPath = @"wwwroot\fileTableAu";
                if (templatePPTX == null || templatePPTX == "")
                    templatePPTX = "template.pptx";
                using (Aspose.Slides.Presentation presentation1 = new Aspose.Slides.Presentation(Path.Combine(contentRootPath, templatePPTX)))
                {
                    //Get the first slide
                    ISlide sld = presentation1.Slides[0];
                    //Instantiate the ImageEx class
                    string file = Path.Combine(contentRootPath, fileImg);
                    Image img = (Image)new Bitmap(file);
                    IPPImage imgx = presentation1.Images.AddImage(img);
                    int pictureWidth = (int)(img.Width / 2.78);
                    int pictureHeight = (int)(img.Height / 2.78);

                    //for whatever reason, aspose measurement unit is pixel * 8

                    int left = 20;
                    int top = 85;
                    //Add Picture Frame with height and width equivalent of Picture

                    IPictureFrame pf = sld.Shapes.AddPictureFrame(ShapeType.Rectangle, left, top, presentation1.SlideSize.Size.Width - 40, presentation1.SlideSize.Size.Height - 130, imgx);

                    // Apply some formatting to PictureFrameEx
                    //pf.LineFormat.FillFormat.FillType = FillType.Solid;
                    //pf.LineFormat.FillFormat.SolidFillColor.Color = Color.Blue;
                    //pf.LineFormat.Width = 20;
                    //pf.Rotation = 45;
                    sld.Shapes.Reorder(sld.Shapes.Count - 1, pf);
                    string filenamePPTX = fileImg.ToLower().Replace(".jpg", ".pptx");
                    presentation1.Save(Path.Combine(contentRootPath, filenamePPTX), Aspose.Slides.Export.SaveFormat.Pptx);
                    return Path.Combine(contentRootPath, filenamePPTX);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }

        } 

        public string IntToRomanNumber(int number)
        {
            StringBuilder result = new StringBuilder();
            int[] digitsValues = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
            string[] romanDigits = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };
            while (number > 0)
            {
                for (int i = digitsValues.Count() - 1; i >= 0; i--)
                    if (number / digitsValues[i] >= 1)
                    {
                        number -= digitsValues[i];
                        result.Append(romanDigits[i]);
                        break;
                    }
            }
            return result.ToString();
        }

        public string[] NormalizeSlidesByID(string[] sourcePresentations, int ycycbcId, int[] listid)
        {
            try
            {
                string contentRootPath = @"wwwroot\fileTableAu";
                List<string> res = new List<string>();
                int page = 1;
                for (int index = 0; index < sourcePresentations.Length; index++)
                {
                    var resp = _tableauRepo.GetSlideInfoByID(listid[index]);
                    if (resp == null)
                    {
                        res.Add(sourcePresentations[index]);
                        page++;
                        continue;
                    };

                    Presentation pres;
                    if (File.Exists(Path.Combine(contentRootPath, sourcePresentations[index])))
                        pres = new Presentation(Path.Combine(contentRootPath, sourcePresentations[index]));
                    else
                        pres = new Presentation(Path.Combine(contentRootPath, "template.pptx"));

                    for (int slideNum = 0; slideNum < pres.Slides.Count(); slideNum++)
                    {
                        ISlide sld = pres.Slides[slideNum];

                        IAutoShape ashp1 = sld.Shapes.AddAutoShape(ShapeType.Rectangle, 0, 0, 58, 56);
                        ashp1.FillFormat.FillType = FillType.NoFill;
                        ashp1.LineFormat.FillFormat.FillType = FillType.NoFill;
                        ITextFrame txtFrame1 = ashp1.TextFrame;
                        IParagraph para1 = txtFrame1.Paragraphs[0];
                        IPortion portion1 = para1.Portions[0];

                        string orderRoman = "";
                        if (resp.OrderBaoCao > 0)
                        {
                            orderRoman = IntToRomanNumber(resp.OrderBaoCao.Value);
                        }

                        portion1.Text = $"{orderRoman}";
                        portion1.PortionFormat.FontBold = NullableBool.True;
                        portion1.PortionFormat.FontHeight = 30;
                        portion1.PortionFormat.FillFormat.FillType = FillType.Solid;
                        portion1.PortionFormat.FillFormat.SolidFillColor.Color = Color.White;

                        IAutoShape ashp2 = sld.Shapes.AddAutoShape(ShapeType.Rectangle, 58, 0, pres.SlideSize.Size.Width - 58, 53);
                        ashp2.FillFormat.FillType = FillType.NoFill;
                        ashp2.LineFormat.FillFormat.FillType = FillType.NoFill;
                        ITextFrame txtFrame2 = ashp2.TextFrame;
                        IParagraph para2 = txtFrame2.Paragraphs[0];
                        para2.ParagraphFormat.Alignment = TextAlignment.Left;
                        IPortion portion2 = para2.Portions[0];
                        portion2.Text = resp.TenBaoCao != "" ? $"{resp.LinhVucBaoCao} - {resp.TenBaoCao}" : "";
                        portion2.PortionFormat.FontHeight = 25;
                        portion2.PortionFormat.FontBold = NullableBool.True;
                        portion2.PortionFormat.FillFormat.FillType = FillType.Solid;
                        portion2.PortionFormat.FillFormat.SolidFillColor.Color = Color.Black;

                        IAutoShape ashp3 = sld.Shapes.AddAutoShape(ShapeType.Rectangle, 50, pres.SlideSize.Size.Height - 35, pres.SlideSize.Size.Width - 50, 35);
                        ashp3.FillFormat.FillType = FillType.NoFill;
                        ashp3.LineFormat.FillFormat.FillType = FillType.NoFill;
                        ITextFrame txtFrame3 = ashp3.TextFrame;
                        IParagraph para3 = txtFrame3.Paragraphs[0];
                        para3.ParagraphFormat.Alignment = TextAlignment.Right;
                        IPortion portion3 = para3.Portions[0];
                        portion3.Text = $"{resp.PhongBan}";
                        portion3.PortionFormat.FontHeight = 20;
                        portion3.PortionFormat.FillFormat.FillType = FillType.Solid;
                        portion3.PortionFormat.FillFormat.SolidFillColor.Color = Color.Black;

                        IAutoShape ashp4 = sld.Shapes.AddAutoShape(ShapeType.Rectangle, 0, pres.SlideSize.Size.Height - 35, 50, 35);
                        ashp4.FillFormat.FillType = FillType.NoFill;
                        ashp4.LineFormat.FillFormat.FillType = FillType.NoFill;
                        ITextFrame txtFrame4 = ashp4.TextFrame;
                        IParagraph para4 = txtFrame4.Paragraphs[0];
                        para4.ParagraphFormat.Alignment = TextAlignment.Center;
                        IPortion portion4 = para4.Portions[0];
                        portion4.Text = $"{page}";
                        portion4.PortionFormat.FontHeight = 20;
                        portion4.PortionFormat.FillFormat.FillType = FillType.Solid;
                        portion4.PortionFormat.FillFormat.SolidFillColor.Color = Color.Black;

                        page++;
                    }
                    // lưu ra file tạm, append vào mảng két quả
                    string tempPPTX = Guid.NewGuid().ToString().Replace("-", "") + ".pptx";
                    pres.Save(Path.Combine(contentRootPath, tempPPTX), Aspose.Slides.Export.SaveFormat.Pptx);
                    res.Add(tempPPTX);                    
                }
                return res.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                _logger.LogError(ex.Message);
                return new string[] { };
            }
        }

        public object MergeSlide(string[] sourcePresentations, int ycbcId)
        {
            try
            {
                string contentRootPath = @"wwwroot\fileTableAu";
                if (sourcePresentations != null && sourcePresentations.Count() > 0)
                {
                    string filePPTXFullPath = "";
                    string filePPTX = "merge-file" + Guid.NewGuid().ToString() + ".pptx";
                    filePPTXFullPath = Path.Combine(contentRootPath, filePPTX);
                    Presentation pres = new Presentation(Path.Combine(@"wwwroot\template", "TopSlide.pptx"));
                    using (Presentation presentation1 = new Presentation(Path.Combine(contentRootPath, sourcePresentations[0])))//Lay file dau tien
                    {
                        // Instantiate a Presentation object that represents a source presentation file
                        for (int j = 0; j < sourcePresentations.Length; j++)//Lay cac file tiep theo ghep vao
                        {
                            using (Aspose.Slides.Presentation presentation2 = new Aspose.Slides.Presentation(Path.Combine(contentRootPath, sourcePresentations[j])))
                            {
                                // Merge only even slides of presentation2 (first slide is at 0 index)
                                for (int i = 0; i < presentation2.Slides.Count; i++)
                                {
                                    pres.Slides.AddClone(presentation2.Slides[i]);
                                }
                            }
                        }
                    }

                    var resp = _baocaoRepo.ThongKeBC(ycbcId);
                    int remaing_rows = resp.Count;
                    int currentPages = pres.Slides.Count;
                    int page = currentPages - 1;
                    int order = 0;
                    int tableRow;
                    while (true)
                    {
                        page++;
                        if (remaing_rows > 10) tableRow = 10;
                        else tableRow = remaing_rows;
                        using (Presentation blankSlide = new Presentation(Path.Combine(@"wwwroot\template", "template.pptx")))
                        {
                            ISlide sld = blankSlide.Slides[0];

                            IAutoShape headerSHP = sld.Shapes.AddAutoShape(ShapeType.Rectangle, 70, 0, blankSlide.SlideSize.Size.Width, 56);
                            headerSHP.FillFormat.FillType = FillType.NoFill;
                            headerSHP.LineFormat.FillFormat.FillType = FillType.NoFill;
                            ITextFrame txtFrameHeader = headerSHP.TextFrame;
                            IParagraph paraHeader = txtFrameHeader.Paragraphs[0];
                            paraHeader.ParagraphFormat.Alignment = TextAlignment.Left;
                            IPortion portionHeader = paraHeader.Portions[0];
                            portionHeader.Text = "Tổng hợp tình hình báo cáo";
                            portionHeader.PortionFormat.FontBold = NullableBool.True;
                            portionHeader.PortionFormat.FontHeight = 30;
                            portionHeader.PortionFormat.FillFormat.FillType = FillType.Solid;
                            portionHeader.PortionFormat.FillFormat.SolidFillColor.Color = Color.Black;

                            IAutoShape leftFooterSHP = sld.Shapes.AddAutoShape(ShapeType.Rectangle, 0, blankSlide.SlideSize.Size.Height - 35, blankSlide.SlideSize.Size.Width / 2, 35);
                            leftFooterSHP.FillFormat.FillType = FillType.NoFill;
                            leftFooterSHP.LineFormat.FillFormat.FillType = FillType.NoFill;
                            ITextFrame leftFooterTextFrame = leftFooterSHP.TextFrame;
                            IParagraph paraLeftFooter = leftFooterTextFrame.Paragraphs[0];
                            paraLeftFooter.ParagraphFormat.Alignment = TextAlignment.Left;
                            IPortion leftFooterPortion = paraLeftFooter.Portions[0];
                            leftFooterPortion.Text = $"{page}";
                            leftFooterPortion.PortionFormat.FontHeight = 20;
                            leftFooterPortion.PortionFormat.FillFormat.FillType = FillType.Solid;
                            leftFooterPortion.PortionFormat.FillFormat.SolidFillColor.Color = Color.Black;

                            IAutoShape rightFooterSHP = sld.Shapes.AddAutoShape(ShapeType.Rectangle, blankSlide.SlideSize.Size.Width / 2, blankSlide.SlideSize.Size.Height - 35, blankSlide.SlideSize.Size.Width / 2, 35);
                            rightFooterSHP.FillFormat.FillType = FillType.NoFill;
                            rightFooterSHP.LineFormat.FillFormat.FillType = FillType.NoFill;
                            ITextFrame rightFooterTextFrame = rightFooterSHP.TextFrame;
                            IParagraph paraRightFooter = rightFooterTextFrame.Paragraphs[0];
                            paraRightFooter.ParagraphFormat.Alignment = TextAlignment.Right;
                            IPortion rightFooterPortion = paraRightFooter.Portions[0];
                            rightFooterPortion.Text = "Ban kế hoạch";
                            rightFooterPortion.PortionFormat.FontHeight = 20;
                            rightFooterPortion.PortionFormat.FillFormat.FillType = FillType.Solid;
                            rightFooterPortion.PortionFormat.FillFormat.SolidFillColor.Color = Color.Black;

                            double[] dblCols = { 40, 400, 110, 110, 100, 100 };
                            double[] dblRows = Enumerable.Repeat<double>(35, tableRow + 2).ToArray();

                            ITable tbl = sld.Shapes.AddTable(50, 80, dblCols, dblRows);
                            for (int row = 0; row < tbl.Rows.Count; row++)
                            {
                                for (int cell = 0; cell < tbl.Rows[row].Count; cell++)
                                {
                                    tbl.Rows[row][cell].CellFormat.BorderTop.FillFormat.FillType = FillType.Solid;
                                    tbl.Rows[row][cell].CellFormat.BorderTop.FillFormat.SolidFillColor.Color = Color.Black;
                                    tbl.Rows[row][cell].CellFormat.BorderTop.Width = 1;

                                    tbl.Rows[row][cell].CellFormat.BorderBottom.FillFormat.FillType = (FillType.Solid);
                                    tbl.Rows[row][cell].CellFormat.BorderBottom.FillFormat.SolidFillColor.Color = Color.Black;
                                    tbl.Rows[row][cell].CellFormat.BorderBottom.Width = 1;

                                    tbl.Rows[row][cell].CellFormat.BorderLeft.FillFormat.FillType = FillType.Solid;
                                    tbl.Rows[row][cell].CellFormat.BorderLeft.FillFormat.SolidFillColor.Color = Color.Black;
                                    tbl.Rows[row][cell].CellFormat.BorderLeft.Width = 1;

                                    tbl.Rows[row][cell].CellFormat.BorderRight.FillFormat.FillType = FillType.Solid;
                                    tbl.Rows[row][cell].CellFormat.BorderRight.FillFormat.SolidFillColor.Color = Color.Black;
                                    tbl.Rows[row][cell].CellFormat.BorderRight.Width = 1;

                                    if (row == 1)
                                    {
                                        tbl.Rows[row][cell].CellFormat.FillFormat.FillType = FillType.Solid;
                                        tbl.Rows[row][cell].CellFormat.FillFormat.SolidFillColor.Color = Color.FromArgb(255, 68, 114, 196);
                                        tbl.Rows[row][cell].TextFrame.Paragraphs[0].Portions[0].PortionFormat.FillFormat.FillType = FillType.Solid;
                                        tbl.Rows[row][cell].TextFrame.Paragraphs[0].Portions[0].PortionFormat.FillFormat.SolidFillColor.Color = Color.WhiteSmoke;
                                        tbl.Rows[row][cell].TextFrame.Paragraphs[0].Portions[0].PortionFormat.FontBold = NullableBool.True;
                                    }
                                }
                            }
                            tbl.MergeCells(tbl.Rows[0][0], tbl.Rows[1][0], false);
                            tbl.MergeCells(tbl.Rows[0][1], tbl.Rows[1][1], false);
                            tbl.MergeCells(tbl.Rows[0][2], tbl.Rows[0][3], false);
                            tbl.MergeCells(tbl.Rows[0][4], tbl.Rows[1][4], false);
                            tbl.MergeCells(tbl.Rows[0][5], tbl.Rows[1][5], false);
                            tbl.Rows[0][0].TextFrame.Text = "TT";
                            tbl.Rows[0][1].TextFrame.Text = "Ban";
                            tbl.Rows[0][2].TextFrame.Text = "Đã xác nhận";
                            tbl.Rows[1][2].TextFrame.Text = "Số liệu đúng";
                            tbl.Rows[1][3].TextFrame.Text = "Chưa đúng";
                            tbl.Rows[0][4].TextFrame.Text = "Chưa xác nhận";
                            tbl.Rows[0][5].TextFrame.Text = "Tổng số";

                            int data_row_index = 2;
                            foreach (var item in resp.Skip(resp.Count - remaing_rows).Take(10).Select((bc, index) => (bc, index)))
                            {
                                tbl.Rows[data_row_index][0].TextFrame.Text = $"{item.index + 1 + order}";
                                tbl.Rows[data_row_index][1].TextFrame.Text = $"{item.bc.TenDonVi}";
                                tbl.Rows[data_row_index][2].TextFrame.Text = $"{item.bc.SoLieuDung}";
                                tbl.Rows[data_row_index][3].TextFrame.Text = $"{item.bc.SoLieuChuaDung}";
                                tbl.Rows[data_row_index][4].TextFrame.Text = $"{item.bc.ChuaXacNhan}";
                                tbl.Rows[data_row_index][5].TextFrame.Text = $"{item.bc.DaXacNhan + item.bc.ChuaXacNhan}";

                                tbl.Rows[data_row_index][2].TextFrame.Paragraphs[0].ParagraphFormat.Alignment = TextAlignment.Right;
                                tbl.Rows[data_row_index][3].TextFrame.Paragraphs[0].ParagraphFormat.Alignment = TextAlignment.Right;
                                tbl.Rows[data_row_index][4].TextFrame.Paragraphs[0].ParagraphFormat.Alignment = TextAlignment.Right;
                                tbl.Rows[data_row_index][5].TextFrame.Paragraphs[0].ParagraphFormat.Alignment = TextAlignment.Right;

                                data_row_index++;
                            }
                            pres.Slides.AddClone(sld);
                        }
                        remaing_rows -= 10;
                        order += 10;
                        if (remaing_rows < 0)
                        {
                            break;
                        } 
                    }

                    using (Presentation end = new Presentation(Path.Combine(@"wwwroot\template", "EndSlide.pptx")))
                    {
                        for (int i = 0; i < end.Slides.Count; i++)
                        {
                            pres.Slides.AddClone(end.Slides[i]);
                        }
                    }

                    pres.Save(filePPTXFullPath, Aspose.Slides.Export.SaveFormat.Pptx);

                    return new { filePPTXFullPath };
                }
                _logger.LogInformation("Khong tim thay file de ghep");
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                _logger.LogError(ex.Message);
                return "";
            }
        }        
    }
}
