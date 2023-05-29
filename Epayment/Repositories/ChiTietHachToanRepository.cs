using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.Repositories
{
    public interface IChiTietHachToanRepository
    {
        ResponseGetChiTietHachToan GetChiTietHachToan(ChiTietHachToanParams ctht);
        Response CreateChiTietHachToan(ChiTietHachToanParams ctht);
        Response UpdateChiTietHachToan(Guid id, ChiTietHachToanParams ctht);
    }

    public class ChiTietHachToanRepository : IChiTietHachToanRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChiTietHachToanRepository> _logger;

        public ChiTietHachToanRepository(ApplicationDbContext context, ILogger<ChiTietHachToanRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Response CreateChiTietHachToan(ChiTietHachToanParams ctht)
        {
            try
            {
                var chitiethachtoanItem = new ChiTietHachToan
                {                    
                    TKNo = ctht.TKNo,
                    TKCo = ctht.TKCo,
                    SoTien = ctht.SoTien,
                    ChungTu = _context.ChungTu.FirstOrDefault(x => (x.ChungTuId == ctht.ChungTuId))
                };
                _context.ChiTietHachToan.Add(chitiethachtoanItem);
                _context.SaveChanges();

                return new Response(message: "Thêm mới thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response UpdateChiTietHachToan(Guid id, ChiTietHachToanParams ctht)
        {
            try
            {
                var chitiethachtoanItem = _context.ChiTietHachToan.FirstOrDefault(x => (x.ChiTietHachToanId == id));

                if (chitiethachtoanItem == null) return new Response(message: "Không tìm thấy chi tiết hạch toán", data: "", errorcode: "001", success: false);

                chitiethachtoanItem.TKNo = ctht.TKNo;
                chitiethachtoanItem.TKCo = ctht.TKCo;
                chitiethachtoanItem.SoTien = ctht.SoTien;

                _context.SaveChanges();

                return new Response(message: "Cập nhật thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "002", success: false);
            }
        }

        public ResponseGetChiTietHachToan GetChiTietHachToan(ChiTietHachToanParams ctht)
        {
            try
            {
                var listChiTietHachToan = from ct in _context.ChiTietHachToan
                                  select new ChiTietHachToanParams
                                  {                                      
                                      TKNo = ct.TKNo,
                                      TKCo = ct.TKCo,
                                      SoTien = ct.SoTien,
                                      ChungTuId = ct.ChungTu.ChungTuId
                                  };
                if (ctht.TKNo != null) listChiTietHachToan = listChiTietHachToan.Where(x => (x.TKNo == ctht.TKNo));
                if (ctht.TKCo != null) listChiTietHachToan = listChiTietHachToan.Where(x => (x.TKCo == ctht.TKCo));
                if (ctht.ChungTuId != null) listChiTietHachToan = listChiTietHachToan.Where(x => (x.ChungTuId == ctht.ChungTuId));

                var totalRecord = listChiTietHachToan.ToList().Count();
                if (ctht.PageIndex > 0)
                {
                    listChiTietHachToan = listChiTietHachToan.Skip(ctht.PageSize * (ctht.PageIndex - 1)).Take(ctht.PageSize);
                }
                var response = listChiTietHachToan.ToList();

                return new ResponseGetChiTietHachToan(message: "", errorcode: "", success: true, items: response, totalRecord: totalRecord);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetChiTietHachToan(message: e.Message, errorcode: "001", success: false, items: null, totalRecord: 0);
            }
        }
    }
}
