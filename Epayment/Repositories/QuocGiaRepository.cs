using AutoMapper;
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
    public interface IQuocGiaRepository
    {
        List<QuocGiaViewModel> GetQuocGia();
        Response CreateQuocGia(QuocGiaParam quocGia);
        ResponseGetQuocGia GetAllQuocGia(QuocGiaPagination quocGiaPagination);
        Response UpdateQuocGia(QuocGiaViewModel quocGia);
        Response DeleteQuocGia(Guid quocGiaId);
        List<TinhThanhPhoViewModel> GetTinhTPByQuocGiaId(Guid quocGiaId);
    }
    public class QuocGiaRepository : IQuocGiaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuocGiaRepository> _logger;
        private readonly IMapper _mapper;
        public QuocGiaRepository(ApplicationDbContext context, ILogger<QuocGiaRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        public Response CreateQuocGia(QuocGiaParam quocGia)
        {
            try
            {
                var quocGiaItem = _context.QuocGia.FirstOrDefault(
                    x => x.MaQuocGia == quocGia.MaQuocGia
                );
                if(quocGiaItem != null)
                {
                    quocGiaItem.MaQuocGia = quocGia.MaQuocGia;
                    quocGiaItem.TenQuocGia = quocGia.TenQuocGia;
                    quocGiaItem.DaXoa = quocGia.DaXoa;
                    quocGiaItem.TrangThai = quocGia.TrangThai;

                    _context.SaveChanges();

                    return new Response(
                        message: "Quốc gia này đã tồn tại, chỉnh sửa thông tin",
                        data: quocGiaItem,
                        errorcode: "",
                        success: true
                    );
                }

                _context.QuocGia.Add(new QuocGia
                {
                    TenQuocGia = quocGia.TenQuocGia,
                    MaQuocGia = quocGia.MaQuocGia,
                    TrangThai = quocGia.TrangThai,
                    DaXoa = quocGia.DaXoa,
                });

                _context.SaveChanges();

                return new Response(
                    message: "Thêm mới thành công",
                    data: "",
                    errorcode: "",
                    success: true
                );
            }
            catch(Exception ex)
            {
                _logger.LogError("Lỗi:", ex);

                return new Response(
                    message: "Lỗi",
                    data: ex.Message,
                    errorcode: "001",
                    success: false
                );
            }
        }

        public Response DeleteQuocGia(Guid quocGiaId)
        {
            try
            {
                var quocGia = _context.QuocGia.Where(
                    x => x.QuocGiaId == quocGiaId
                ).FirstOrDefault();

                if(quocGia == null)
                {
                    return new Response(
                        message: "Không tìm thấy quốc gia",
                        data: "",
                        errorcode: "002",
                        success: false
                    );
                }

                _context.QuocGia.Remove(quocGia);

                _context.SaveChanges();

                return new Response(
                    message: "Xóa thành công",
                    data: "",
                    errorcode: "",
                    success: true
                );
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(
                    message: "Lỗi",
                    data: ex.Message,
                    errorcode: "001",
                    success: false
                );
            }
        }

        public ResponseGetQuocGia GetAllQuocGia(QuocGiaPagination quocGiaPagination)
        {
            try
            {
                var listQuocGia = from x in _context.QuocGia
                                  select new QuocGiaViewModel
                                  {
                                      QuocGiaId = x.QuocGiaId,
                                      TenQuocGia = x.TenQuocGia,
                                      MaQuocGia = x.MaQuocGia,
                                      TrangThai = x.TrangThai,
                                      DaXoa = x.DaXoa
                                  };

                listQuocGia = listQuocGia.OrderBy(x => x.MaQuocGia);

                if (quocGiaPagination.QuocGiaId != null)
                {
                    listQuocGia = listQuocGia.Where(x => x.QuocGiaId == quocGiaPagination.QuocGiaId);
                }
                if (quocGiaPagination.TenQuocGia != null)
                {
                    listQuocGia = listQuocGia.Where(x => x.TenQuocGia == quocGiaPagination.TenQuocGia);
                }
                if (quocGiaPagination.MaQuocGia != null)
                {
                    listQuocGia = listQuocGia.Where(x => x.MaQuocGia == quocGiaPagination.MaQuocGia);
                }
                if (quocGiaPagination.TrangThai != -99)
                {
                    listQuocGia = listQuocGia.Where(x => x.TrangThai == quocGiaPagination.TrangThai);
                }
                // if (quocGiaPagination.DaXoa != -1)
                // {
                //     listQuocGia = listQuocGia.Where(x => x.DaXoa == quocGiaPagination.DaXoa);
                // }

                var totalRecord = listQuocGia.ToList().Count();

                if (quocGiaPagination.PageIndex > 0)
                {
                    listQuocGia = listQuocGia.Skip(
                        quocGiaPagination.PageSize * (quocGiaPagination.PageIndex - 1)
                    ).Take(
                        quocGiaPagination.PageSize
                    );
                }

                var response = listQuocGia.ToList();

                return new ResponseGetQuocGia("", "", true, totalRecord, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new ResponseGetQuocGia(ex.Message, "001", false, 0, null);
            }
        }

        public List<QuocGiaViewModel> GetQuocGia()
        {
            try
            {
                var listQuocGia = from QG in _context.QuocGia
                                  select new QuocGiaViewModel
                                  {
                                      QuocGiaId = QG.QuocGiaId,
                                      MaQuocGia = QG.MaQuocGia,
                                      TenQuocGia = QG.TenQuocGia,
                                      TrangThai = QG.TrangThai,
                                      DaXoa = QG.DaXoa
                                  };
                listQuocGia = listQuocGia.OrderBy(x => x.MaQuocGia);
                return listQuocGia.ToList();
            }
            catch (System.Exception e)
            {
                _logger.LogError("Lỗi:", e);
                throw;
            }
        }

        public Response UpdateQuocGia(QuocGiaViewModel quocGia)
        {
            try
            {
                var quocGiaItem = _context.QuocGia.FirstOrDefault(
                    x => x.MaQuocGia == quocGia.MaQuocGia
                );

                if (quocGiaItem == null)
                {
                    return new Response(
                        message: "Không tìm thấy quốc gia",
                        data: "",
                        errorcode: "002",
                        success: false
                    );
                }

                quocGiaItem.MaQuocGia = quocGia.MaQuocGia;
                quocGiaItem.TenQuocGia = quocGia.TenQuocGia;
                quocGiaItem.DaXoa = quocGia.DaXoa;
                quocGiaItem.TrangThai = quocGia.TrangThai;

                _context.SaveChanges();

                return new Response(
                    message: "Cập nhật thành công",
                    data: "",
                    errorcode: "",
                    success: true
                );
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);

                return new Response(
                    message: "Lỗi",
                    data: e.Message,
                    errorcode: "001",
                    success: false
                );
            }
        }
        public List<TinhThanhPhoViewModel> GetTinhTPByQuocGiaId(Guid quocGiaId)
        {
            try
            {
                var listTinhTP = from TP in _context.TinhThanhPho
                                 where TP.QuocGia.QuocGiaId == quocGiaId
                                 select new TinhThanhPhoViewModel
                                 {
                                     TinhTpId = TP.TinhTpId,
                                     QuocGia = TP.QuocGia,
                                     MaTinhTp = TP.MaTinhTp,
                                     TenTinhTp = TP.TenTinhTp,
                                     TrangThai = TP.TrangThai,
                                     DaXoa = TP.DaXoa
                                 };
                listTinhTP = listTinhTP.OrderBy(x => x.MaTinhTp);
                return listTinhTP.ToList();
            }
            catch (System.Exception e)
            {
                _logger.LogError("Lỗi:", e);
                throw;
            }
        }
    }
}