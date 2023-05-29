using BCXN.Data;
using BCXN.Statics;
using ceTe.DynamicPDF.Conversion;
using Epayment.Repositories;
using Epayment.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Epayment.Services
{
    public interface IGeneratePDFService
    {
        public object GenerateUyNhiemChi(Guid chungTuId, int type);
        public object GeneratePhieuChi(Guid chungTuId, int type, bool init = true);
        public string MergePCUNC(Guid chungTuId);
        public object ConvertWordToPDF();
        public string GeneratePCUNC(Guid chungTuId, bool init = true);
        public string GenerateCTGS(Guid chungTuId);
    }

    public class GeneratePDFService : IGeneratePDFService
    {
        private readonly ILogger<GeneratePDFService> _logger;
        private readonly IChungTuRepository _chungTuRepo;
        private readonly IChiTietHachToanRepository _hachToanRepo;
        private readonly ApplicationDbContext _context;
        private readonly IUploadToFtpService _uploadService;

        public GeneratePDFService(ApplicationDbContext context, ILogger<GeneratePDFService> logger, IChungTuRepository chungTuRepo, IChiTietHachToanRepository hachToanRepo, IUploadToFtpService uploadService)
        {
            _logger = logger;
            _chungTuRepo = chungTuRepo;
            _hachToanRepo = hachToanRepo;
            _context = context;
            _uploadService = uploadService;
        }

        public object GeneratePhieuChi(Guid chungTuId, int type, bool init = true)
        {
            try
            {
                var chungTu = _chungTuRepo.GetChungTuByID(chungTuId);
                var chiTietHachToan = _hachToanRepo.GetChiTietHachToan(new ChiTietHachToanParams { ChungTuId = chungTuId });
                var kySo1 = _chungTuRepo.GetKySoChungTu(new KySoChungTuParams
                {
                    CapKy = 1,
                    ChungTuId = chungTuId
                });
                var kySo2 = _chungTuRepo.GetKySoChungTu(new KySoChungTuParams
                {
                    CapKy = 2,
                    ChungTuId = chungTuId
                });
                var kySo3 = _chungTuRepo.GetKySoChungTu(new KySoChungTuParams
                {
                    CapKy = 3,
                    ChungTuId = chungTuId
                });

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                BaseFont baseNormalFont = BaseFont.CreateFont("wwwroot\\fonts\\times.ttf", BaseFont.IDENTITY_H, true);
                BaseFont baseBoldFont = BaseFont.CreateFont("wwwroot\\fonts\\timesbd.ttf", BaseFont.IDENTITY_H, true);
                BaseFont baseBoldItalicFont = BaseFont.CreateFont("wwwroot\\fonts\\timesbi.ttf", BaseFont.IDENTITY_H, true);
                BaseFont baseItalicFont = BaseFont.CreateFont("wwwroot\\fonts\\timesi.ttf", BaseFont.IDENTITY_H, true);
                var headerFont = new Font(baseBoldFont, 18);
                var normalFont = new Font(baseNormalFont, 12);
                var boldFont = new Font(baseBoldFont, 12);
                var boldItalicFont = new Font(baseBoldItalicFont, 12);
                var italicFont = new Font(baseItalicFont, 12);
                var blueFont = new Font(baseNormalFont, 9, Font.NORMAL, new BaseColor(51, 96, 185));

                var donVi = chungTu.DonViThanhToan.TenDonVi;
                var diaChi = chungTu.DiaChiChuyen;
                var soPhieuChi = chungTu.SoChungTuERP;
                string ngayLapPhieuChi = "";
                if (chungTu.NgayLapChungTu != DateTime.MinValue) ngayLapPhieuChi = chungTu.NgayLapChungTu.ToString("dd/MM/yyyy").Replace("-", "/");
                var taiKhoanNo = new List<string>();
                var taiKhoanCo = new List<string>();
                var soTien = new List<string>();
                if (chiTietHachToan.Success)
                {
                    IEnumerable<object> collection = (IEnumerable<object>)chiTietHachToan.Data["Items"];
                    foreach (var item in collection)
                    {
                        if (item.GetType().GetProperty("TKCo").GetValue(item) == null || item.GetType().GetProperty("TKCo").GetValue(item).ToString().Trim() == "")
                        {
                            taiKhoanCo.Add("0");
                        }
                        else
                        {
                            taiKhoanCo.Add(item.GetType().GetProperty("TKCo").GetValue(item).ToString());
                        }
                        if (item.GetType().GetProperty("TKNo").GetValue(item) == null || item.GetType().GetProperty("TKNo").GetValue(item).ToString().Trim() == "")
                        {
                            taiKhoanNo.Add("0");
                        }
                        else
                        {
                            taiKhoanNo.Add(item.GetType().GetProperty("TKNo").GetValue(item).ToString());
                        }
                        if (item.GetType().GetProperty("SoTien").GetValue(item) == null || item.GetType().GetProperty("SoTien").GetValue(item).ToString().Trim() == "")
                        {
                            soTien.Add("0");
                        }
                        else
                        {
                            //soTien.Add((Convert.ToDecimal(item.GetType().GetProperty("SoTien").GetValue(item)).ToString("C").Replace("$", "").Split(".")[0].Replace(",", ".") + "," + Convert.ToDecimal(item.GetType().GetProperty("SoTien").GetValue(item)).ToString("C").Replace("$", "").Split(".")[1]).Replace(",00", ""));
                            string st = Convert.ToDecimal(item.GetType().GetProperty("SoTien").GetValue(item)).ToString("C").Replace("$", "").Replace(",", " ").Replace(".", ",");
                            if (chungTu.LoaiTienTe.ToUpper() == "VND") st = st.Replace(",00", "");
                            soTien.Add(st);
                        }
                    }
                }
                var loaiTien = chungTu.LoaiTienTe;
                var nguoiNhanTien = chungTu.TenTaiKhoanNhan;
                var diaChiNhanTien = chungTu.DiaChiNhan;
                var lyDoChi = chungTu.NoiDungTT;
                if (lyDoChi == null || lyDoChi.Trim() == "")
                {
                    lyDoChi = "Không có";
                }
                var tmpTongSoTienPhieuChi = chungTu.SoTien;
                var tienChuPhieuChi = tmpTongSoTienPhieuChi > 0 ? NumberToText((double)tmpTongSoTienPhieuChi, loaiTien) : "Không đồng";
                var tongSoTienPhieuChi = tmpTongSoTienPhieuChi.ToString("C").Replace("$", "").Replace(",", " ").Replace(".", ",");
                if (loaiTien.ToUpper() == "VND") tongSoTienPhieuChi = tongSoTienPhieuChi.Replace(",00", "");

                var diaChiLapPhieuChi = chungTu.MaTinhTPChuyen;
                var ngayLap = ngayLapPhieuChi != "" ? ngayLapPhieuChi.Split("/")[0] : "";
                var thangLap = ngayLapPhieuChi != "" ? ngayLapPhieuChi.Split("/")[1] : "";
                var namLap = ngayLapPhieuChi != "" ? ngayLapPhieuChi.Split("/")[2] : "";
                string nguoiLap_KyBoi = null;
                string nguoiLap_KyNgay = null;
                bool nguoiLap_DaKy = false;
                string truongPhongTaiChinh_KyBoi = null;
                string truongPhongTaiChinh_KyNgay = null;
                bool truongPhongTaiChinh_DaKy = false;
                string giamDoc_KyBoi = null;
                string giamDoc_KyNgay = null;
                bool giamDoc_DaKy = false;
                if (kySo1.Success)
                {
                    IEnumerable<object> collection = (IEnumerable<object>)kySo1.Data["Items"];
                    foreach (var item in collection)
                    {
                        if (item.GetType().GetProperty("CapKy").GetValue(item).Equals(1))
                        {
                            nguoiLap_DaKy = item.GetType().GetProperty("DaKy").GetValue(item).Equals(true);
                            if (item.GetType().GetProperty("NguoiKy").GetValue(item) != null && item.GetType().GetProperty("NguoiKy").GetValue(item).ToString().Trim() != "")
                            {
                                var nguoiKy = item.GetType().GetProperty("NguoiKy").GetValue(item);
                                nguoiLap_KyBoi = nguoiKy.GetType().GetProperty("FirstName").GetValue(nguoiKy).ToString() + " " + nguoiKy.GetType().GetProperty("LastName").GetValue(nguoiKy).ToString();
                            }
                            if (item.GetType().GetProperty("NgayKy").GetValue(item) != null && item.GetType().GetProperty("NgayKy").GetValue(item).ToString().Trim() != "")
                            {
                                string sNgayLap = item.GetType().GetProperty("NgayKy").GetValue(item).ToString();
                                DateTime dNgayLap = Convert.ToDateTime(sNgayLap);
                                nguoiLap_KyNgay = dNgayLap.ToString("dd/MM/yyyy").Replace("-", "/");

                            }
                        }
                    }
                }
                if (kySo2.Success)
                {
                    IEnumerable<object> collection = (IEnumerable<object>)kySo2.Data["Items"];
                    foreach (var item in collection)
                    {
                        if (item.GetType().GetProperty("CapKy").GetValue(item).Equals(2))
                        {
                            truongPhongTaiChinh_DaKy = item.GetType().GetProperty("DaKy").GetValue(item).Equals(true);
                            if (item.GetType().GetProperty("NguoiKy").GetValue(item) != null && item.GetType().GetProperty("NguoiKy").GetValue(item).ToString().Trim() != "")
                            {
                                var nguoiKy = item.GetType().GetProperty("NguoiKy").GetValue(item);
                                truongPhongTaiChinh_KyBoi = nguoiKy.GetType().GetProperty("FirstName").GetValue(nguoiKy).ToString() + " " + nguoiKy.GetType().GetProperty("LastName").GetValue(nguoiKy).ToString();
                            }
                            if (item.GetType().GetProperty("NgayKy").GetValue(item) != null && item.GetType().GetProperty("NgayKy").GetValue(item).ToString().Trim() != "")
                            {
                                string sNgayLap = item.GetType().GetProperty("NgayKy").GetValue(item).ToString();
                                DateTime dNgayLap = Convert.ToDateTime(sNgayLap);
                                truongPhongTaiChinh_KyNgay = dNgayLap.ToString("dd/MM/yyyy").Replace("-", "/");
                            }
                        }
                    }
                }
                if (kySo3.Success)
                {
                    IEnumerable<object> collection = (IEnumerable<object>)kySo3.Data["Items"];
                    foreach (var item in collection)
                    {
                        if (item.GetType().GetProperty("CapKy").GetValue(item).Equals(3))
                        {
                            giamDoc_DaKy = item.GetType().GetProperty("DaKy").GetValue(item).Equals(true);
                            if (item.GetType().GetProperty("NguoiKy").GetValue(item) != null && item.GetType().GetProperty("NguoiKy").GetValue(item).ToString().Trim() != "")
                            {
                                var nguoiKy = item.GetType().GetProperty("NguoiKy").GetValue(item);
                                giamDoc_KyBoi = nguoiKy.GetType().GetProperty("FirstName").GetValue(nguoiKy).ToString() + " " + nguoiKy.GetType().GetProperty("LastName").GetValue(nguoiKy).ToString();
                            }
                            if (item.GetType().GetProperty("NgayKy").GetValue(item) != null && item.GetType().GetProperty("NgayKy").GetValue(item).ToString().Trim() != "")
                            {
                                string sNgayLap = item.GetType().GetProperty("NgayKy").GetValue(item).ToString();
                                DateTime dNgayLap = Convert.ToDateTime(sNgayLap);
                                giamDoc_KyNgay = dNgayLap.ToString("dd/MM/yyyy").Replace("-", "/");
                            }
                        }
                    }
                }

                PdfPTable table = null;
                PdfPCell cell = null;
                PdfContentByte cb = null;
                Paragraph para = null;
                Phrase p = null;
                Image img = null;

                Document doc = new Document(iTextSharp.text.PageSize.A4.Rotate());
                doc.SetMargins(72f, 72f, 72f, 72f);
                string path = "";
                if (type == 1)
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "PC-" + chungTuId + ".pdf");
                }
                else if(type == 2)
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "temp-PC-" + chungTuId + ".pdf");
                }
                FileStream fs = new FileStream(path, FileMode.Create);
                var writer = PdfWriter.GetInstance(doc, fs);

                doc.Open();

                table = new PdfPTable(3);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                float[] widths = new float[] { 1.75f, 2.25f, 1f };
                table.SetWidths(widths);
                p = new Phrase("ĐƠN VỊ: " + donVi + "\nĐỊA CHỈ: " + diaChi, boldFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("PHIẾU CHI NGÂN HÀNG", headerFont);
                para = new Paragraph(p);
                p = new Phrase("\nSỐ: " + soPhieuChi, boldFont);
                para.Add(p);
                p = new Phrase("\nNgày: " + ngayLapPhieuChi, italicFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.Border = 0;
                cell.SetLeading(3f, 1f);
                table.AddCell(cell);
                p = new Phrase("Mẫu số 02-TT\n(Ban hành theo TT200/2014/TT-BTC ngày 22/12/2014 của BTC)", normalFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                doc.Add(table);

                cb = new PdfContentByte(writer);
                cb = writer.DirectContent;
                cb.MoveTo(663f, doc.PageSize.Height - 135f);
                cb.LineTo(735f, doc.PageSize.Height - 135f);
                cb.Stroke();

                p = new Phrase("\n\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                if (taiKhoanCo.Count <= 4 && taiKhoanNo.Count <= 4 && soTien.Count <= 4 &&
                    taiKhoanCo.Count > 0 && taiKhoanNo.Count > 0 && soTien.Count > 0)
                {
                    table = new PdfPTable(6);
                    table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                    table.LockedWidth = true;
                    widths = new float[] { 1.5f, 1.5f, 1.5f, 1.5f, 2f, 2f };
                    table.SetWidths(widths);
                    p = new Phrase("TK nợ: ", boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = 0;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    p = new Phrase(taiKhoanNo[0], boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = 0;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    p = new Phrase("TK có: ", boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = 0;
                    cell.SetLeading(3f, 1f);
                    table.AddCell(cell);
                    p = new Phrase(taiKhoanCo[0], boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = 0;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    p = new Phrase("Số Tiền: ", boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = 0;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    p = new Phrase(soTien[0] + " " + loaiTien, boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = 0;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    for (var index = 1; index < soTien.Count; index++)
                    {
                        cell = new PdfPCell();
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        cell.SetLeading(2f, 1f);
                        table.AddCell(cell);
                        p = new Phrase(taiKhoanNo[index], boldFont);
                        cell = new PdfPCell(p);
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        cell.SetLeading(2f, 1f);
                        table.AddCell(cell);
                        cell = new PdfPCell();
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        cell.SetLeading(3f, 1f);
                        table.AddCell(cell);
                        p = new Phrase(taiKhoanCo[index], boldFont);
                        cell = new PdfPCell(p);
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        cell.SetLeading(2f, 1f);
                        table.AddCell(cell);
                        cell = new PdfPCell();
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        cell.SetLeading(2f, 1f);
                        table.AddCell(cell);
                        p = new Phrase(soTien[index] + " " + loaiTien, boldFont);
                        cell = new PdfPCell(p);
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        cell.SetLeading(2f, 1f);
                        table.AddCell(cell);
                    }
                    doc.Add(table);

                    p = new Phrase("\n", normalFont);
                    para = new Paragraph();
                    para.Add(p);
                    doc.Add(para);
                }

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Họ tên người nhận tiền: " + nguoiNhanTien, normalFont);
                cell = new PdfPCell(p);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Địa chỉ: " + diaChiNhanTien, normalFont);
                cell = new PdfPCell(p);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Lý do chi: " + lyDoChi, normalFont);
                cell = new PdfPCell(p);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Số tiền: ", normalFont);
                para = new Paragraph(p);
                p = new Phrase(tongSoTienPhieuChi + " " + loaiTien, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Viết bằng chữ: ", normalFont);
                para = new Paragraph(p);
                p = new Phrase(tienChuPhieuChi, italicFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                doc.Add(table);

                p = new Phrase("\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                table = new PdfPTable(2);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                widths = new float[] { 11f, 1f };
                table.SetWidths(widths);
                p = new Phrase(diaChiLapPhieuChi + ", ngày " + ngayLap + " tháng " + thangLap + " năm " + namLap, italicFont);
                cell = new PdfPCell(p);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                doc.Add(table);

                table = new PdfPTable(3);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Giám đốc", boldFont);
                para = new Paragraph(p);
                p = new Phrase("\n( Ký, họ tên )", normalFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Trưởng phòng Tài chính", boldFont);
                para = new Paragraph(p);
                p = new Phrase("\n( Ký, họ tên )", normalFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Người lập", boldFont);
                para = new Paragraph(p);
                p = new Phrase("\n( Ký, họ tên )", normalFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                doc.Add(table);

                table = new PdfPTable(12);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                widths = new float[] { 1f, 1.5f, 3f, 0.5f, 1f, 1.5f, 3f, 0.5f, 1f, 1.5f, 3f, 0.5f };
                table.SetWidths(widths);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký bởi:", blueFont);
                //cell = giamDoc_DaKy ? new PdfPCell(p) : new PdfPCell();
                if (init == true) cell = giamDoc_DaKy ? new PdfPCell(p) : new PdfPCell();
                else cell = new PdfPCell(p);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                //p = new Phrase(giamDoc_KyBoi, blueFont);
                //cell = giamDoc_DaKy ? new PdfPCell(p) : new PdfPCell();
                p = new Phrase("Nguyễn Xuân Nam", blueFont);
                if (init == true) cell = giamDoc_DaKy ? new PdfPCell(p) : new PdfPCell();
                else cell = new PdfPCell(p);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký bởi:", blueFont);
                //cell = truongPhongTaiChinh_DaKy ? new PdfPCell(p) : new PdfPCell();
                if (init == true) cell = truongPhongTaiChinh_DaKy ? new PdfPCell(p) : new PdfPCell();
                else cell = new PdfPCell(p);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                //p = new Phrase(truongPhongTaiChinh_KyBoi, blueFont);
                //cell = truongPhongTaiChinh_DaKy ? new PdfPCell(p) : new PdfPCell();
                p = new Phrase("Võ Hồng Lĩnh", blueFont);
                if (init == true) cell = truongPhongTaiChinh_DaKy ? new PdfPCell(p) : new PdfPCell();
                else cell = new PdfPCell(p);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký bởi:", blueFont);
                cell = nguoiLap_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(nguoiLap_KyBoi, blueFont);
                cell = nguoiLap_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký ngày:", blueFont);
                cell = giamDoc_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(giamDoc_KyNgay, blueFont);
                cell = giamDoc_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký ngày:", blueFont);
                cell = truongPhongTaiChinh_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(truongPhongTaiChinh_KyNgay, blueFont);
                cell = truongPhongTaiChinh_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký ngày:", blueFont);
                cell = nguoiLap_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(nguoiLap_KyNgay, blueFont);
                cell = nguoiLap_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                doc.Add(table);

                /*cb = new PdfContentByte(writer);
                cb = writer.DirectContent;
                img = Image.GetInstance(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "tick.png"));
                cb.AddImage(img, 10f, 10f, 10f, 10f, 10f, 10f);*/

                if (taiKhoanNo.Count > 4 || taiKhoanCo.Count > 4 || soTien.Count > 4)
                {
                    doc.NewPage();

                    para = new Paragraph("ĐỊNH KHOẢN PHIẾU CHI NGÂN HÀNG", headerFont);
                    para.Alignment = Element.ALIGN_CENTER;
                    doc.Add(para);

                    para = new Paragraph("Số: " + soPhieuChi, boldFont);
                    para.Alignment = Element.ALIGN_CENTER;
                    doc.Add(para);

                    para = new Paragraph("Ngày: " + ngayLapPhieuChi, italicFont);
                    para.Alignment = Element.ALIGN_CENTER;
                    doc.Add(para);

                    para = new Paragraph("\n");
                    doc.Add(para);

                    table = new PdfPTable(4);
                    table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                    table.LockedWidth = true;
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    p = new Phrase("Tài khoản nợ", boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    p = new Phrase("Tài khoản có", boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    p = new Phrase("Diễn giải", boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    p = new Phrase("Số tiền", boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    for (var index = 0; index < soTien.Count; index++)
                    {
                        p = new Phrase(taiKhoanNo[index], normalFont);
                        cell = new PdfPCell(p);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.SetLeading(2f, 1f);
                        table.AddCell(cell);
                        p = new Phrase(taiKhoanCo[index], normalFont);
                        cell = new PdfPCell(p);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.SetLeading(2f, 1f);
                        table.AddCell(cell);
                        cell = new PdfPCell();
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.SetLeading(2f, 1f);
                        table.AddCell(cell);
                        p = new Phrase(soTien[index] + " " + loaiTien, normalFont);
                        cell = new PdfPCell(p);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.SetLeading(2f, 1f);
                        table.AddCell(cell);
                    }
                    cell = new PdfPCell();
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.SetLeading(2f, 1f);
                    cell.Border = 0;
                    table.AddCell(cell);
                    cell = new PdfPCell();
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.SetLeading(2f, 1f);
                    cell.Border = 0;
                    table.AddCell(cell);
                    p = new Phrase("Tổng cộng", boldFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    p = new Phrase(tongSoTienPhieuChi + " " + loaiTien, normalFont);
                    cell = new PdfPCell(p);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.SetLeading(2f, 1f);
                    table.AddCell(cell);
                    doc.Add(table);
                }

                doc.Close();
                fs.Close();
                writer.Close();

                return new ResponseBase64(ConvertPDFToBase64(path));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return null;
            }
        }

        public object GenerateUyNhiemChi(Guid chungTuId, int type)
        {
            try
            {
                var chungTu = _chungTuRepo.GetChungTuByID(chungTuId);
                var kySo1 = _chungTuRepo.GetKySoChungTu(new KySoChungTuParams
                {
                    CapKy = 1,
                    ChungTuId = chungTuId
                });
                var kySo2 = _chungTuRepo.GetKySoChungTu(new KySoChungTuParams
                {
                    CapKy = 2,
                    ChungTuId = chungTuId
                });
                var kySo3 = _chungTuRepo.GetKySoChungTu(new KySoChungTuParams
                {
                    CapKy = 3,
                    ChungTuId = chungTuId
                });

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                BaseFont baseNormalFont = BaseFont.CreateFont("wwwroot\\fonts\\times.ttf", BaseFont.IDENTITY_H, true);
                BaseFont baseBoldFont = BaseFont.CreateFont("wwwroot\\fonts\\timesbd.ttf", BaseFont.IDENTITY_H, true);
                BaseFont baseBoldItalicFont = BaseFont.CreateFont("wwwroot\\fonts\\timesbi.ttf", BaseFont.IDENTITY_H, true);
                BaseFont baseItalicFont = BaseFont.CreateFont("wwwroot\\fonts\\timesi.ttf", BaseFont.IDENTITY_H, true);
                var headerFont = new Font(baseBoldFont, 18);
                var normalFont = new Font(baseNormalFont, 12);
                var boldFont = new Font(baseBoldFont, 12);
                var boldItalicFont = new Font(baseBoldItalicFont, 12);
                var italicFont = new Font(baseItalicFont, 12);
                var blueFont = new Font(baseNormalFont, 9, Font.NORMAL, new BaseColor(51, 96, 185));

                string ngayLapUyNhiemChi = "";
                if (chungTu.NgayLapChungTu != DateTime.MinValue) ngayLapUyNhiemChi = chungTu.NgayLapChungTu.ToString("dd/MM/yyyy").Replace("-", "/");
                var soUyNhiemChi = chungTu.SoChungTuERP;
                var ctyTraTien = chungTu.TenTaiKhoanChuyen;
                var stkTraTien = chungTu.SoTaiKhoanChuyen;
                var nhTraTien = chungTu.TenNganHangChuyen;
                var chiNhanhTraTien = _context.ChiNhanhNganHang.FirstOrDefault(x => x.MaChiNhanhErp == chungTu.MaChiNhanhChuyen) == null ? "" : _context.ChiNhanhNganHang.FirstOrDefault(x => x.MaChiNhanhErp == chungTu.MaChiNhanhChuyen).TenChiNhanh;
                var tTraTien = chungTu.MaTinhTPChuyen;
                var ctyNhanTien = chungTu.TenTaiKhoanNhan;
                var stkNhanTien = chungTu.SoTaiKhoanNhan;
                var nhNhanTien = chungTu.TenNganHangNhan;
                var chiNhanhNhanTien = _context.ChiNhanhNganHang.FirstOrDefault(x => x.MaChiNhanhErp == chungTu.MaChiNhanhNhan) == null ? "" : _context.ChiNhanhNganHang.FirstOrDefault(x => x.MaChiNhanhErp == chungTu.MaChiNhanhNhan).TenChiNhanh;
                var tNhanTien = chungTu.MaTinhTPNhan;
                var tmpTienSo = chungTu.SoTien;
                var loaiTien = chungTu.LoaiTienTe;
                var tienSo = tmpTienSo.ToString("C").Replace("$", "").Replace(",", " ").Replace(".", ",");
                if (loaiTien.ToUpper() == "VND") tienSo = tienSo.Replace(",00", "");
                var tienChu = tmpTienSo > 0 ? NumberToText((double)tmpTienSo, loaiTien) : "Không đồng";
                var noiDung = chungTu.NoiDungTT;
                if (noiDung == null || noiDung.Trim() == "")
                {
                    noiDung = "Không có";
                }
                var ngayGhiSo = "".Replace("-", "/");
                var trangThaiGiaoDich = chungTu.TrangThaiCT == 6 ? "Thành công" : "Không thành công";
                string keToan_KyBoi = null;
                string keToan_KyNgay = null;
                bool keToan_DaKy = false;
                string chuTaiKhoan_KyBoi = null;
                string chuTaiKhoan_KyNgay = null;
                bool chuTaiKhoan_DaKy = false;
                if (kySo2.Success)
                {
                    IEnumerable<object> collection = (IEnumerable<object>)kySo2.Data["Items"];
                    foreach (var item in collection)
                    {
                        if (item.GetType().GetProperty("CapKy").GetValue(item).Equals(2))
                        {
                            keToan_DaKy = item.GetType().GetProperty("DaKy").GetValue(item).Equals(true);
                            if (item.GetType().GetProperty("NguoiKy").GetValue(item) != null && item.GetType().GetProperty("NguoiKy").GetValue(item).ToString().Trim() != "")
                            {
                                var nguoiKy = item.GetType().GetProperty("NguoiKy").GetValue(item);
                                keToan_KyBoi = nguoiKy.GetType().GetProperty("FirstName").GetValue(nguoiKy).ToString() + " " + nguoiKy.GetType().GetProperty("LastName").GetValue(nguoiKy).ToString();
                            }
                            if (item.GetType().GetProperty("NgayKy").GetValue(item) != null && item.GetType().GetProperty("NgayKy").GetValue(item).ToString().Trim() != "")
                            {
                                string sNgayLap = item.GetType().GetProperty("NgayKy").GetValue(item).ToString();
                                DateTime dNgayLap = Convert.ToDateTime(sNgayLap);
                                keToan_KyNgay = dNgayLap.ToString("dd/MM/yyyy").Replace("-", "/");
                            }
                        }
                    }
                }
                if (kySo3.Success)
                {
                    IEnumerable<object> collection = (IEnumerable<object>)kySo3.Data["Items"];
                    foreach (var item in collection)
                    {
                        if (item.GetType().GetProperty("CapKy").GetValue(item).Equals(3))
                        {
                            chuTaiKhoan_DaKy = item.GetType().GetProperty("DaKy").GetValue(item).Equals(true);
                            if (item.GetType().GetProperty("NguoiKy").GetValue(item) != null && item.GetType().GetProperty("NguoiKy").GetValue(item).ToString().Trim() != "")
                            {
                                var nguoiKy = item.GetType().GetProperty("NguoiKy").GetValue(item);
                                chuTaiKhoan_KyBoi = nguoiKy.GetType().GetProperty("FirstName").GetValue(nguoiKy).ToString() + " " + nguoiKy.GetType().GetProperty("LastName").GetValue(nguoiKy).ToString();
                            }
                            if (item.GetType().GetProperty("NgayKy").GetValue(item) != null && item.GetType().GetProperty("NgayKy").GetValue(item).ToString().Trim() != "")
                            {
                                string sNgayLap = item.GetType().GetProperty("NgayKy").GetValue(item).ToString();
                                DateTime dNgayLap = Convert.ToDateTime(sNgayLap);
                                chuTaiKhoan_KyNgay = dNgayLap.ToString("dd/MM/yyyy").Replace("-", "/");
                            }
                        }
                    }
                }
                float space = 0;

                PdfPTable table = null;
                PdfPCell cell = null;
                PdfContentByte cb = null;
                Paragraph para = null;
                Phrase p = null;
                Image img = null;

                Document doc = new Document(iTextSharp.text.PageSize.A4);
                doc.SetMargins(72f, 72f, 72f, 72f);
                string path = "";
                if(type == 1)
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "UNC-" + chungTuId + ".pdf");
                }
                else if(type == 2)
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "temp-UNC-" + chungTuId + ".pdf");
                }
                FileStream fs = new FileStream(path, FileMode.Create);
                var writer = PdfWriter.GetInstance(doc, fs);

                doc.Open();

                table = new PdfPTable(3);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                float[] widths = new float[] { 1f, 2f, 1f };
                table.SetWidths(widths);
                cell = new PdfPCell();
                cell.Border = 0;
                table.AddCell(cell);
                p = new Phrase("ỦY NHIỆM CHI", headerFont);
                para = new Paragraph(p);
                p = new Phrase("\nCHUYỂN KHOẢN, CHUYỂN TIỀN\nTHƯ, ĐIỆN", boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(3f, 1f);
                table.AddCell(cell);
                p = new Phrase("Số: " + soUyNhiemChi, normalFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                doc.Add(table);

                para = new Paragraph("Ngày lập: " + ngayLapUyNhiemChi, normalFont);
                para.Alignment = Element.ALIGN_CENTER;
                doc.Add(para);

                p = new Phrase("\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Tên đơn vị trả tiền: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(ctyTraTien, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Số tài khoản: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(stkTraTien, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Tại chi nhánh: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(chiNhanhTraTien, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = 430f - 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Tỉnh,TP: ", normalFont);
                para = new Paragraph(p);
                p = new Phrase(tTraTien, normalFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                cb = new PdfContentByte(writer);
                cb = writer.DirectContent;
                cb.MoveTo(72f, doc.PageSize.Height - 240f - space);
                cb.LineTo(doc.PageSize.Width - 72f, doc.PageSize.Height - 240f - space);
                cb.Stroke();
                cb.MoveTo(435f, doc.PageSize.Height - 210f - space);
                cb.LineTo(430f, doc.PageSize.Height - 210f - space);
                cb.LineTo(430f, doc.PageSize.Height - 290f - space);
                cb.LineTo(doc.PageSize.Width - 72f, doc.PageSize.Height - 290f - space);
                cb.LineTo(doc.PageSize.Width - 72f, doc.PageSize.Height - 210f - space);
                cb.LineTo(doc.PageSize.Width - 77f, doc.PageSize.Height - 210f - space);
                cb.Stroke();

                cb = new PdfContentByte(writer);
                cb = writer.DirectContent;
                cb.SetFontAndSize(baseNormalFont, 9);
                cb.ShowTextAligned(Element.ALIGN_CENTER, "PHẦN DO NH GHI", 475f, doc.PageSize.Height - 205f - space, 0);
                cb.ShowTextAligned(Element.ALIGN_CENTER, "TÀI KHOẢN NỢ", 475f, doc.PageSize.Height - 220f - space, 0);
                cb.ShowTextAligned(Element.ALIGN_CENTER, "TÀI KHOẢN CÓ", 475f, doc.PageSize.Height - 255f - space, 0);
                cb.Stroke();

                p = new Phrase("\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 4 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Tên đơn vị nhận tiền: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(ctyNhanTien, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Số tài khoản: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(stkNhanTien, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Tại chi nhánh: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(chiNhanhNhanTien, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = 430f - 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Tỉnh,TP: ", normalFont);
                para = new Paragraph(p);
                p = new Phrase(tNhanTien, normalFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                cb = new PdfContentByte(writer);
                cb = writer.DirectContent;
                cb.MoveTo(72f, doc.PageSize.Height - 325f - space);
                cb.LineTo(430f, doc.PageSize.Height - 325f - space);
                cb.MoveTo(430f, doc.PageSize.Height - 305f - space);
                cb.LineTo(430f, doc.PageSize.Height - 345f - space);
                cb.LineTo(doc.PageSize.Width - 72f, doc.PageSize.Height - 345f - space);
                cb.LineTo(doc.PageSize.Width - 72f, doc.PageSize.Height - 305f - space);
                cb.LineTo(430f, doc.PageSize.Height - 305f - space);
                cb.Stroke();   

                cb = new PdfContentByte(writer);
                cb = writer.DirectContent;
                cb.SetFontAndSize(baseNormalFont, 9);
                cb.ShowTextAligned(Element.ALIGN_CENTER, "SỐ TIỀN BẰNG SỐ", 475f, doc.PageSize.Height - 320f - space, 0);
                cb.Stroke();

                cb = new PdfContentByte(writer);
                cb = writer.DirectContent;
                cb.SetFontAndSize(baseBoldFont, 9);
                cb.ShowTextAligned(Element.ALIGN_CENTER, tienSo + " " + loaiTien, 475f, doc.PageSize.Height - 337f - space, 0);
                cb.Stroke();

                p = new Phrase("\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                table = new PdfPTable(1);
                table.TotalWidth = 430f - 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Số tiền bằng chữ: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(tienChu, italicFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                p = new Phrase("\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Nội dung thanh toán: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(noiDung, normalFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                p = new Phrase("\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                table = new PdfPTable(2);
                table.TotalWidth = doc.PageSize.Width - 2 * 36f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                p = new Phrase("Đơn vị trả tiền", boldFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ngân hàng", boldFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                if (chungTu.TrangThaiCT >= TrangThaiChungTu.DaChuyenTien) p = new Phrase("Ghi sổ ngày: " + ngayGhiSo + "\nTrạng thái giao dịch: " + trangThaiGiaoDich, boldFont);
                else p = new Phrase("Ghi sổ ngày: " + ngayGhiSo, boldFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(4);
                table.TotalWidth = doc.PageSize.Width - 2 * 36f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                p = new Phrase("Kế toán", boldFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Chủ tài khoản", boldFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Kế toán", boldFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Người phê duyệt", boldFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(8);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                widths = new float[] { 0.15f, 1.25f, 2f, 0.15f, 1.25f, 2f, 3.4f, 3.4f };
                table.SetWidths(widths);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký bởi:", blueFont);
                cell = keToan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(keToan_KyBoi, blueFont);
                cell = keToan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký bởi:", blueFont);
                cell = chuTaiKhoan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(chuTaiKhoan_KyBoi, blueFont);
                cell = chuTaiKhoan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký ngày:", blueFont);
                cell = keToan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(keToan_KyNgay, blueFont);
                cell = keToan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký ngày:", blueFont);
                cell = chuTaiKhoan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(chuTaiKhoan_KyNgay, blueFont);
                cell = chuTaiKhoan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                //table = new PdfPTable(4);
                //table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                //table.LockedWidth = true;
                //table.HorizontalAlignment = Element.ALIGN_LEFT;
                //p = new Phrase("\n\n\n\n\n\n\n\n\nVõ Hồng Lĩnh", boldFont);
                //cell = new PdfPCell(p);
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //cell.Border = 0;
                //table.AddCell(cell);
                //p = new Phrase("\n\n\n\n\n\n\n\n\nNguyễn Xuân Nam", boldFont);
                //cell = new PdfPCell(p);
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //cell.Border = 0;
                //table.AddCell(cell);
                //p = new Phrase("", boldFont);
                //cell = new PdfPCell(p);
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //cell.Border = 0;
                //table.AddCell(cell);
                //p = new Phrase("", boldFont);
                //cell = new PdfPCell(p);
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //cell.Border = 0;
                //table.AddCell(cell);
                //space += table.CalculateHeights() - 18f;
                //doc.Add(table);

                /*cb = new PdfContentByte(writer);
                cb = writer.DirectContent;
                img = Image.GetInstance(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "tick.png"));
                cb.AddImage(img, 0, 0, 10f, 10f, 10f, 10f);*/

                doc.Close();
                fs.Close();
                writer.Close();

                return new ResponseBase64(ConvertPDFToBase64(path));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return null;
            }
        }

        public string GeneratePCUNC(Guid chungTuId, bool init = true)
        {
            try
            {
                GeneratePhieuChi(chungTuId, 2, init);
                GenerateUyNhiemChi(chungTuId, 2);

                var fileNames = new List<string>();

                var pathPC = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "temp-PC-" + chungTuId + ".pdf");
                var pathUNC = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "temp-UNC-" + chungTuId + ".pdf");

                if (File.Exists(pathPC))
                {
                    fileNames.Add(pathPC);
                }
                if (File.Exists(pathUNC))
                {
                    fileNames.Add(pathUNC);
                }

                Document doc = new Document();
                string path = Path.Combine("wwwroot", "filePDF", "PC-UNC-" + chungTuId + ".pdf");
                FileStream fs = new FileStream(path, FileMode.Create);
                var writer = new PdfSmartCopy(doc, fs);

                doc.Open();

                foreach (var fileName in fileNames)
                {
                    PdfReader reader = new PdfReader(fileName);
                    writer.AddDocument(reader);
                    reader.Close();
                }
                writer.Close();
                doc.Close();
                fs.Close();

                if (File.Exists(pathPC))
                {
                    File.Delete(pathPC);
                }
                if (File.Exists(pathUNC))
                {
                    File.Delete(pathUNC);
                }
                return path;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return "";
            }
        }

        public string MergePCUNC(Guid chungTuId)
        {
            try
            {
                GeneratePhieuChi(chungTuId, 2);
                GenerateUyNhiemChi(chungTuId, 2);
                var hoSoId = _context.ChungTu.FirstOrDefault(x => x.ChungTuId == chungTuId).HoSoThanhToan.HoSoId;
                var maHoSo = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == hoSoId).MaHoSo;
                var fileNames = new List<string>();

                var pathPC = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "temp-PC-" + chungTuId + ".pdf");
                var pathUNC = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "temp-UNC-" + chungTuId + ".pdf");

                if(File.Exists(pathPC))
                {
                    fileNames.Add(pathPC);
                }
                if(File.Exists(pathUNC))
                {
                    fileNames.Add(pathUNC);
                }

                Document doc = new Document();
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "PC-UNC-" + chungTuId + ".pdf");
                FileStream fs = new FileStream(path, FileMode.Create);
                var writer = new PdfSmartCopy(doc, fs);

                doc.Open();

                foreach(var fileName in fileNames)
                {
                    PdfReader reader = new PdfReader(fileName);
                    writer.AddDocument(reader);
                    reader.Close();
                }
                writer.Close();
                doc.Close();
                fs.Close();

                if (File.Exists(pathPC))
                {
                    File.Delete(pathPC);
                }
                if (File.Exists(pathUNC))
                {
                    File.Delete(pathUNC);
                }
                // var pathFileServer = _uploadService.UploadByteFileFTP(File.ReadAllBytes(path), maHoSo, "PCNH/UNC", "pdf");
                // return new ResponseBase64(ConvertPDFToBase64(pathFileServer));
                return path;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return null;
            }
        }

        public static string NumberToText(double inputNumber, string loaiTien)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            if (loaiTien.ToUpper() != "VND") return result + " " + loaiTien;
            else return result + " đồng chẵn";
        }

        public string ConvertPDFToBase64(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            string file = Convert.ToBase64String(bytes);
            return file;
        }

        public object ConvertWordToPDF()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "test.docx");
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "temp-test.pdf");
            string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "test.pdf");

            // comvert word to pdf
            WordConverter wordDocConverter = new WordConverter(path);
            wordDocConverter.Convert(tempPath);

            // xoa watermark
            PdfReader pdfReader = new PdfReader(tempPath);
            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (PdfStamper stamper = new PdfStamper(pdfReader, fs))
                {
                    int PageCount = pdfReader.NumberOfPages;
                    for (int x = 1; x <= PageCount; x++)
                    {
                        PdfContentByte cb = stamper.GetOverContent(x);
                        Rectangle rectangle = new Rectangle(0, pdfReader.GetPageSize(x).Height, pdfReader.GetPageSize(x).Width, pdfReader.GetPageSize(x).Height - 25f);
                        rectangle.BackgroundColor = BaseColor.WHITE;
                        cb.Rectangle(rectangle);
                    }
                }
            }

            pdfReader.Close();

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
            return null;
        }

        public string GenerateCTGS(Guid chungTuId)
        {
            try
            {
                var chungTu = _chungTuRepo.GetChungTuByID(chungTuId);
                var chiTietHachToan = _hachToanRepo.GetChiTietHachToan(new ChiTietHachToanParams { ChungTuId = chungTuId });
                var kySo1 = _chungTuRepo.GetKySoChungTu(new KySoChungTuParams
                {
                    CapKy = 1,
                    ChungTuId = chungTuId
                });
                var kySo2 = _chungTuRepo.GetKySoChungTu(new KySoChungTuParams
                {
                    CapKy = 2,
                    ChungTuId = chungTuId
                });
                var kySo3 = _chungTuRepo.GetKySoChungTu(new KySoChungTuParams
                {
                    CapKy = 3,
                    ChungTuId = chungTuId
                });

                var taiKhoanNo = new List<string>();
                var taiKhoanCo = new List<string>();
                var soTien = new List<string>();
                if (chiTietHachToan.Success)
                {
                    IEnumerable<object> collection = (IEnumerable<object>)chiTietHachToan.Data["Items"];
                    foreach (var item in collection)
                    {
                        if (item.GetType().GetProperty("TKCo").GetValue(item) == null || item.GetType().GetProperty("TKCo").GetValue(item).ToString().Trim() == "")
                        {
                            taiKhoanCo.Add("0");
                        }
                        else
                        {
                            taiKhoanCo.Add(item.GetType().GetProperty("TKCo").GetValue(item).ToString());
                        }
                        if (item.GetType().GetProperty("TKNo").GetValue(item) == null || item.GetType().GetProperty("TKNo").GetValue(item).ToString().Trim() == "")
                        {
                            taiKhoanNo.Add("0");
                        }
                        else
                        {
                            taiKhoanNo.Add(item.GetType().GetProperty("TKNo").GetValue(item).ToString());
                        }
                        if (item.GetType().GetProperty("SoTien").GetValue(item) == null || item.GetType().GetProperty("SoTien").GetValue(item).ToString().Trim() == "")
                        {
                            soTien.Add("0");
                        }
                        else
                        {
                            //soTien.Add((Convert.ToDecimal(item.GetType().GetProperty("SoTien").GetValue(item)).ToString("C").Replace("$", "").Split(".")[0].Replace(",", ".") + "," + Convert.ToDecimal(item.GetType().GetProperty("SoTien").GetValue(item)).ToString("C").Replace("$", "").Split(".")[1]).Replace(",00", ""));
                            string st = Convert.ToDecimal(item.GetType().GetProperty("SoTien").GetValue(item)).ToString("C").Replace("$", "").Replace(",", " ").Replace(".", ",");
                            // if (chungTu.LoaiTienTe.ToUpper() == "VND") st = st.Replace(",00", "");
                            soTien.Add(st);
                        }
                    }
                }

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                BaseFont baseNormalFont = BaseFont.CreateFont("wwwroot\\fonts\\times.ttf", BaseFont.IDENTITY_H, true);
                BaseFont baseBoldFont = BaseFont.CreateFont("wwwroot\\fonts\\timesbd.ttf", BaseFont.IDENTITY_H, true);
                BaseFont baseBoldItalicFont = BaseFont.CreateFont("wwwroot\\fonts\\timesbi.ttf", BaseFont.IDENTITY_H, true);
                BaseFont baseItalicFont = BaseFont.CreateFont("wwwroot\\fonts\\timesi.ttf", BaseFont.IDENTITY_H, true);
                var headerFont = new Font(baseBoldFont, 18);
                var normalFont = new Font(baseNormalFont, 12);
                var boldFont = new Font(baseBoldFont, 12);
                var boldItalicFont = new Font(baseBoldItalicFont, 12);
                var italicFont = new Font(baseItalicFont, 12);
                var blueFont = new Font(baseNormalFont, 9, Font.NORMAL, new BaseColor(51, 96, 185));

                var donVi = chungTu.DonViThanhToan.TenDonVi;
                var diaChi = chungTu.DiaChiChuyen;
                var noiDungTT = chungTu.NoiDungTT;
                var diaChiChuyen = chungTu.DiaChiChuyen;
                var soChungTu = chungTu.SoChungTuERP;
                string ngayLapUyNhiemChi = "";
                if (chungTu.NgayLapChungTu != DateTime.MinValue) ngayLapUyNhiemChi = chungTu.NgayLapChungTu.ToString("dd/MM/yyyy").Replace("-", "/");
                var soUyNhiemChi = chungTu.SoChungTuERP;
                var ctyTraTien = chungTu.TenTaiKhoanChuyen;
                var stkTraTien = chungTu.SoTaiKhoanChuyen;
                var nhTraTien = chungTu.TenNganHangChuyen;
                var chiNhanhTraTien = _context.ChiNhanhNganHang.FirstOrDefault(x => x.MaChiNhanhErp == chungTu.MaChiNhanhChuyen) == null ? "" : _context.ChiNhanhNganHang.FirstOrDefault(x => x.MaChiNhanhErp == chungTu.MaChiNhanhChuyen).TenChiNhanh;
                var tTraTien = chungTu.MaTinhTPChuyen;
                var ctyNhanTien = chungTu.TenTaiKhoanNhan;
                var stkNhanTien = chungTu.SoTaiKhoanNhan;
                var nhNhanTien = chungTu.TenNganHangNhan;
                var chiNhanhNhanTien = _context.ChiNhanhNganHang.FirstOrDefault(x => x.MaChiNhanhErp == chungTu.MaChiNhanhNhan) == null ? "" : _context.ChiNhanhNganHang.FirstOrDefault(x => x.MaChiNhanhErp == chungTu.MaChiNhanhNhan).TenChiNhanh;
                var tNhanTien = chungTu.MaTinhTPNhan;
                var tmpTienSo = chungTu.SoTien;
                string ngayLapPhieuChi = "";
                if (chungTu.NgayLapChungTu != DateTime.MinValue) ngayLapPhieuChi = chungTu.NgayLapChungTu.ToString("dd/MM/yyyy").Replace("-", "/");
                var ngayLap = ngayLapPhieuChi != "" ? ngayLapPhieuChi.Split("/")[0] : "";
                var thangLap = ngayLapPhieuChi != "" ? ngayLapPhieuChi.Split("/")[1] : "";
                var namLap = ngayLapPhieuChi != "" ? ngayLapPhieuChi.Split("/")[2] : "";
                // var loaiTien = chungTu.LoaiTienTe;
                var tienSo = tmpTienSo.ToString("C").Replace("$", "").Replace(",", " ").Replace(".", ",");
                // if (loaiTien.ToUpper() == "VND") tienSo = tienSo.Replace(",00", "");
                // var tienChu = tmpTienSo > 0 ? NumberToText((double)tmpTienSo, loaiTien) : "Không đồng";
                var noiDung = chungTu.NoiDungTT;
                if (noiDung == null || noiDung.Trim() == "")
                {
                    noiDung = "Không có";
                }
                var ngayGhiSo = "".Replace("-", "/");
                var trangThaiGiaoDich = chungTu.TrangThaiCT == 6 ? "Thành công" : "Không thành công";
                string keToan_KyBoi = null;
                string keToan_KyNgay = null;
                bool keToan_DaKy = false;
                string chuTaiKhoan_KyBoi = null;
                string chuTaiKhoan_KyNgay = null;
                bool chuTaiKhoan_DaKy = false;
                if (kySo2.Success)
                {
                    IEnumerable<object> collection = (IEnumerable<object>)kySo2.Data["Items"];
                    foreach (var item in collection)
                    {
                        if (item.GetType().GetProperty("CapKy").GetValue(item).Equals(2))
                        {
                            keToan_DaKy = item.GetType().GetProperty("DaKy").GetValue(item).Equals(true);
                            if (item.GetType().GetProperty("NguoiKy").GetValue(item) != null && item.GetType().GetProperty("NguoiKy").GetValue(item).ToString().Trim() != "")
                            {
                                var nguoiKy = item.GetType().GetProperty("NguoiKy").GetValue(item);
                                keToan_KyBoi = nguoiKy.GetType().GetProperty("FirstName").GetValue(nguoiKy).ToString() + " " + nguoiKy.GetType().GetProperty("LastName").GetValue(nguoiKy).ToString();
                            }
                            if (item.GetType().GetProperty("NgayKy").GetValue(item) != null && item.GetType().GetProperty("NgayKy").GetValue(item).ToString().Trim() != "")
                            {
                                string sNgayLap = item.GetType().GetProperty("NgayKy").GetValue(item).ToString();
                                DateTime dNgayLap = Convert.ToDateTime(sNgayLap);
                                keToan_KyNgay = dNgayLap.ToString("dd/MM/yyyy").Replace("-", "/");
                            }
                        }
                    }
                }
                if (kySo3.Success)
                {
                    IEnumerable<object> collection = (IEnumerable<object>)kySo3.Data["Items"];
                    foreach (var item in collection)
                    {
                        if (item.GetType().GetProperty("CapKy").GetValue(item).Equals(3))
                        {
                            chuTaiKhoan_DaKy = item.GetType().GetProperty("DaKy").GetValue(item).Equals(true);
                            if (item.GetType().GetProperty("NguoiKy").GetValue(item) != null && item.GetType().GetProperty("NguoiKy").GetValue(item).ToString().Trim() != "")
                            {
                                var nguoiKy = item.GetType().GetProperty("NguoiKy").GetValue(item);
                                chuTaiKhoan_KyBoi = nguoiKy.GetType().GetProperty("FirstName").GetValue(nguoiKy).ToString() + " " + nguoiKy.GetType().GetProperty("LastName").GetValue(nguoiKy).ToString();
                            }
                            if (item.GetType().GetProperty("NgayKy").GetValue(item) != null && item.GetType().GetProperty("NgayKy").GetValue(item).ToString().Trim() != "")
                            {
                                string sNgayLap = item.GetType().GetProperty("NgayKy").GetValue(item).ToString();
                                DateTime dNgayLap = Convert.ToDateTime(sNgayLap);
                                chuTaiKhoan_KyNgay = dNgayLap.ToString("dd/MM/yyyy").Replace("-", "/");
                            }
                        }
                    }
                }
                float space = 0;

                PdfPTable table = null;
                PdfPCell cell = null;
                PdfContentByte cb = null;
                Paragraph para = null;
                Phrase p = null;
                Image img = null;

                Document doc = new Document(iTextSharp.text.PageSize.A4);
                doc.SetMargins(72f, 72f, 72f, 72f);
                string path = "";
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "filePDF", "CTGS-" + chungTuId + ".pdf");

                FileStream fs = new FileStream(path, FileMode.Create);
                var writer = PdfWriter.GetInstance(doc, fs);

                doc.Open();

                

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Đơn vị: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(donVi, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Địa chỉ: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(diaChiChuyen, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Chi nhánh: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(chiNhanhTraTien, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(3);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                float[] widths = new float[] { 1f, 2f, 1f };
                table.SetWidths(widths);
                cell = new PdfPCell();
                cell.Border = 0;
                table.AddCell(cell);
                p = new Phrase("CHỨNG TỪ GHI SỔ", headerFont);
                para = new Paragraph(p);
                // p = new Phrase("\nCHUYỂN KHOẢN, CHUYỂN TIỀN\nTHƯ, ĐIỆN", boldFont);
                // para.Add(p);
                cell = new PdfPCell(para);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(3f, 1f);
                table.AddCell(cell);
                p = new Phrase("Số chứng từ: " + soChungTu, normalFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                doc.Add(table);

                para = new Paragraph("Ngày " + ngayLap + " tháng " + thangLap + " năm " + namLap, normalFont);
                para.Alignment = Element.ALIGN_CENTER;
                doc.Add(para);

                p = new Phrase("\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                p = new Phrase("\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 4 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p = new Phrase("Họ và tên:: ", normalFont);
                para = new Paragraph();
                para.Add(p);
                p = new Phrase(ctyNhanTien, boldFont);
                para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                table = new PdfPTable(1);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_RIGHT;
                p = new Phrase("Đơn vị tính: " + chungTu.LoaiTienTe, normalFont);
                para = new Paragraph();
                para.Add(p);
                // p = new Phrase("đồng", boldFont);
                // para.Add(p);
                cell = new PdfPCell(para);
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);
                
                p = new Phrase("\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                PdfPTable table1 = new PdfPTable(5);
                table1.TotalWidth = doc.PageSize.Width - 2 * 72f;
                // table1.TotalWidth = 500f;
                table1.LockedWidth = true;
                float[] widthsTable = new float[] { 260f, 60f, 60f, 60f, 60f };
                table1.SetWidths(widthsTable);
                cell = new PdfPCell(new Phrase("Trích yếu", normalFont));
                table1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Số hiệu tài khoản", normalFont));  
                cell.Colspan = 2;  
                table1.AddCell(cell);

                cell = new PdfPCell(new Phrase("Số tiền", normalFont));  
                cell.Colspan = 2;  
                table1.AddCell(cell);
  
                table1.AddCell("");
                cell = new PdfPCell(new Phrase("Nợ", normalFont));
                table1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Có", normalFont));
                table1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Nợ", normalFont));
                table1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Có", normalFont));
                table1.AddCell(cell);

                var tongTienNo = 0;
                var tongTienCo = 0;
                for (var index = 0; index < soTien.Count; index++)
                {
                    cell = new PdfPCell(new Phrase(noiDungTT, normalFont));
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Phrase(taiKhoanNo[index] == "0" ? "" : taiKhoanNo[index], normalFont));
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Phrase(taiKhoanCo[index] == "0" ? "" : taiKhoanCo[index], normalFont));
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Phrase((taiKhoanNo[index] != "" && taiKhoanNo[index] != "0") ? soTien[index] : "", normalFont));
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Phrase((taiKhoanCo[index] != "" && taiKhoanCo[index] != "0") ? soTien[index] : "", normalFont));
                    table1.AddCell(cell);
                    if(soTien[index] != "0" && soTien[index] != ""){
                        var tienChan = soTien[index].Split(",");
                        string tienChan1 = tienChan[0].Replace(@" ",@"");
                        if(taiKhoanNo[index] != "" && taiKhoanNo[index] != "0"){
                            tongTienNo += Int32.Parse(tienChan1);
                        }
                        if(taiKhoanCo[index] != "" && taiKhoanCo[index] != "0"){
                            tongTienCo += Int32.Parse(tienChan1); 
                        }
                    }
                }
                

                cell = new PdfPCell(new Phrase("Tổng", normalFont));  
                cell.Colspan = 3;  
                table1.AddCell(cell); 
                table1.AddCell(new PdfPCell(new Phrase(tongTienNo == 0 ? "" : tongTienNo.ToString(), normalFont)));
                table1.AddCell(new PdfPCell(new Phrase(tongTienCo == 0 ? "" : tongTienCo.ToString(), normalFont)));
                // table1.AddCell("");
                // table1.AddCell("");

                doc.Add(table1);

                p = new Phrase("\n", normalFont);
                para = new Paragraph();
                para.Add(p);
                doc.Add(para);

                table = new PdfPTable(2);
                table.TotalWidth = doc.PageSize.Width - 2 * 36f;
                table.LockedWidth = true;
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                p = new Phrase("Người lập biểu", boldFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Kế toán trưởng", boldFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("(ký, họ tên)" + ngayGhiSo, normalFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("(ký, họ tên)" + ngayGhiSo, normalFont);
                cell = new PdfPCell(p);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                // table = new PdfPTable(4);
                // table.TotalWidth = doc.PageSize.Width - 2 * 36f;
                // table.LockedWidth = true;
                // table.HorizontalAlignment = Element.ALIGN_CENTER;
                // p = new Phrase("Kế toán", boldFont);
                // cell = new PdfPCell(p);
                // cell.HorizontalAlignment = Element.ALIGN_CENTER;
                // cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                // cell.Border = 0;
                // cell.SetLeading(2f, 1f);
                // table.AddCell(cell);
                // p = new Phrase("Chủ tài khoản", boldFont);
                // cell = new PdfPCell(p);
                // cell.HorizontalAlignment = Element.ALIGN_CENTER;
                // cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                // cell.Border = 0;
                // cell.SetLeading(2f, 1f);
                // table.AddCell(cell);
                // p = new Phrase("Kế toán", boldFont);
                // cell = new PdfPCell(p);
                // cell.HorizontalAlignment = Element.ALIGN_CENTER;
                // cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                // cell.Border = 0;
                // cell.SetLeading(2f, 1f);
                // table.AddCell(cell);
                // p = new Phrase("Người phê duyệt", boldFont);
                // cell = new PdfPCell(p);
                // cell.HorizontalAlignment = Element.ALIGN_CENTER;
                // cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                // cell.Border = 0;
                // cell.SetLeading(2f, 1f);
                // table.AddCell(cell);
                // space += table.CalculateHeights() - 18f;
                // doc.Add(table);

                table = new PdfPTable(8);
                table.TotalWidth = doc.PageSize.Width - 2 * 72f;
                table.LockedWidth = true;
                widths = new float[] { 0.15f, 1.25f, 2f, 0.15f, 1.25f, 2f, 3.4f, 3.4f };
                table.SetWidths(widths);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký bởi:", blueFont);
                cell = keToan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(keToan_KyBoi, blueFont);
                cell = keToan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký bởi:", blueFont);
                cell = chuTaiKhoan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(chuTaiKhoan_KyBoi, blueFont);
                cell = chuTaiKhoan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký ngày:", blueFont);
                cell = keToan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(keToan_KyNgay, blueFont);
                cell = keToan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase("Ký ngày:", blueFont);
                cell = chuTaiKhoan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                p = new Phrase(chuTaiKhoan_KyNgay, blueFont);
                cell = chuTaiKhoan_DaKy ? new PdfPCell(p) : new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.SetLeading(2f, 1f);
                table.AddCell(cell);
                space += table.CalculateHeights() - 18f;
                doc.Add(table);

                doc.Close();
                fs.Close();
                writer.Close();

                return path;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return null;
            }
        }
    }

    public class ResponseBase64
    {
        public string Data { get; set; }

        public ResponseBase64(string Data)
        {
            this.Data = Data;
        }
    }
}
