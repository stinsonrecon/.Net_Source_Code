using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using BCXN.Data;
using BCXN.ViewModels;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using Epayment.Models;

namespace Epayment.Repositories
{
    public interface ICauHinhNganHangRepository
    {
        List<CauHinhNganHangViewModel> GetCauHinhNganHang();
        Response CreateCauHinhNganHang(CauHinhNganHangParams cauHinh);
        ResponseGetCauHinhNganHang GetAllCauHinhNganHang(CauHinhNganHangPagination cauHinhPagination);
        Response UpdateCauHinhNganHang(CauHinhNganHangViewModel cauHinh);
        Response DeleteCauHinhNganHang(Guid cauHinhId);
    }
    public class CauHinhNganHangRepository : ICauHinhNganHangRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CauHinhNganHangRepository> _logger;
        private readonly IMapper _mapper;
        public CauHinhNganHangRepository(ApplicationDbContext context, ILogger<CauHinhNganHangRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        public Response CreateCauHinhNganHang(CauHinhNganHangParams cauHinh)
        {
            try
            {
                var nganHangItem = _context.NganHang.FirstOrDefault(
                    x => x.NganHangId == cauHinh.NganHangId
                );

                if(nganHangItem == null)
                {

                    return new Response(
                        message: "Không tìm thấy ngân hàng",
                        data: "", 
                        errorcode: "002",
                        success: false
                    );
                }

                var cauHinhItem = _context.CauHinhNganHang.FirstOrDefault(
                    x => x.NganHang.NganHangId == cauHinh.NganHangId
                );
                if(cauHinhItem != null)
                {
                    cauHinhItem.NganHang = nganHangItem;
                    cauHinhItem.LinkTransferUrl = cauHinh.LinkTransferUrl;
                    cauHinhItem.LinkQueryBaseUrl = cauHinh.LinkQueryBaseUrl;
                    cauHinhItem.FTPPath = cauHinh.FTPPath;
                    cauHinhItem.FTPUserName = cauHinh.FTPUserName;
                    cauHinhItem.FTPPassword = cauHinh.FTPPassword;
                    cauHinhItem.EVNDSAPath = cauHinh.EVNDSAPath;
                    cauHinhItem.PassPhraseDSA = cauHinh.PassPhraseDSA;
                    cauHinhItem.EVNRSAPath = cauHinh.EVNRSAPath;
                    cauHinhItem.PassPhraseRSA = cauHinh.PassPhraseRSA;
                    cauHinhItem.BankDSAPath = cauHinh.BankDSAPath;
                    cauHinhItem.BankRSAPath = cauHinh.BankRSAPath;
                    cauHinhItem.BankApprover = cauHinh.BankApprover;
                    cauHinhItem.BankSenderAddr = cauHinh.BankSenderAddr;
                    cauHinhItem.BankModel = cauHinh.BankModel;
                    cauHinhItem.Version = cauHinh.Version;
                    cauHinhItem.SoftwareProviderId = cauHinh.SoftwareProviderId;
                    cauHinhItem.BankLanguage = cauHinh.BankLanguage;
                    cauHinhItem.BankAppointedApprover = cauHinh.BankAppointedApprover;
                    cauHinhItem.BankChannel = cauHinh.BankChannel;
                    cauHinhItem.BankMerchantId = cauHinh.BankMerchantId;
                    cauHinhItem.BankClientIP = cauHinh.BankClientIP;
                    cauHinhItem.DaXoa = cauHinh.DaXoa;

                    _context.SaveChanges();

                    return new Response(
                        message: "Cấu hình ngân hàng này đã tồn tại, chỉnh sửa cấu hình",
                        data: cauHinhItem,
                        errorcode: "",
                        success: true
                    );
                }

                _context.CauHinhNganHang.Add(new CauHinhNganHang {
                    NganHang = nganHangItem,
                    LinkTransferUrl = cauHinh.LinkTransferUrl,
                    LinkQueryBaseUrl = cauHinh.LinkQueryBaseUrl,
                    FTPPath = cauHinh.FTPPath,
                    FTPUserName = cauHinh.FTPUserName,
                    FTPPassword = cauHinh.FTPPassword,
                    EVNDSAPath = cauHinh.EVNDSAPath,
                    PassPhraseDSA = cauHinh.PassPhraseDSA,
                    EVNRSAPath = cauHinh.EVNRSAPath,
                    PassPhraseRSA = cauHinh.PassPhraseRSA,
                    BankDSAPath = cauHinh.BankDSAPath,
                    BankRSAPath = cauHinh.BankRSAPath,
                    BankApprover = cauHinh.BankApprover,
                    BankSenderAddr = cauHinh.BankSenderAddr,
                    BankModel = cauHinh.BankModel,
                    Version = cauHinh.Version,
                    SoftwareProviderId = cauHinh.SoftwareProviderId,
                    BankLanguage = cauHinh.BankLanguage,
                    BankAppointedApprover = cauHinh.BankAppointedApprover,
                    BankChannel = cauHinh.BankChannel,
                    BankMerchantId = cauHinh.BankMerchantId,
                    BankClientIP = cauHinh.BankClientIP
                });

                _context.SaveChanges();

                return new Response(
                    message: "Thêm mới thành công", 
                    data: "", 
                    errorcode: "", 
                    success: true
                );
            }
            catch(Exception e)
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

        public Response DeleteCauHinhNganHang(Guid cauHinhId)
        {
            try
            {
                var cauHinh = _context.CauHinhNganHang.Where(
                    s => s.CauHinhNganHangId == cauHinhId
                ).FirstOrDefault();

                if(cauHinh == null)
                {
                    return new Response(
                        message: "Không tìm thấy ngân hàng",
                        data: "", 
                        errorcode: "002",
                        success: false
                    );
                }

                _context.CauHinhNganHang.Remove(cauHinh);
                _context.SaveChanges();
                return new Response(
                    message: "Xóa thành công", 
                    data: "", 
                    errorcode: "", 
                    success: true
                );
            }
            catch(Exception e)
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

        public ResponseGetCauHinhNganHang GetAllCauHinhNganHang(CauHinhNganHangPagination cauHinhPagination)
        {
            try 
            {
                var listCauHinh = from ch in _context.CauHinhNganHang
                                select new CauHinhNganHangViewModel
                                {
                                    CauHinhNganHangId = ch.CauHinhNganHangId,
                                    NganHangId = ch.NganHang.NganHangId,
                                    MaNganHang = ch.NganHang.MaNganHang,
                                    TenNganHang = ch.NganHang.TenNganHang,
                                    LinkTransferUrl = ch.LinkTransferUrl,
                                    LinkQueryBaseUrl = ch.LinkQueryBaseUrl,
                                    FTPPath = ch.FTPPath,
                                    FTPUserName = ch.FTPUserName,
                                    FTPPassword = ch.FTPPassword,
                                    EVNDSAPath = ch.EVNDSAPath,
                                    PassPhraseDSA = ch.PassPhraseDSA,
                                    EVNRSAPath = ch.EVNRSAPath,
                                    PassPhraseRSA = ch.PassPhraseRSA,
                                    BankDSAPath = ch.BankDSAPath,
                                    BankRSAPath = ch.BankRSAPath,
                                    BankApprover = ch.BankApprover,
                                    BankSenderAddr = ch.BankSenderAddr,
                                    BankModel = ch.BankModel,
                                    Version = ch.Version,
                                    SoftwareProviderId = ch.SoftwareProviderId,
                                    BankLanguage = ch.BankLanguage,
                                    BankAppointedApprover = ch.BankAppointedApprover,
                                    BankChannel = ch.BankChannel,
                                    BankMerchantId = ch.BankMerchantId,
                                    BankClientIP = ch.BankClientIP,
                                    DaXoa = ch.DaXoa
                                };

                listCauHinh = listCauHinh.OrderBy(x => x.MaNganHang);

                if(cauHinhPagination.CauHinhNganHangId != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.CauHinhNganHangId == cauHinhPagination.CauHinhNganHangId);
                }
                if(cauHinhPagination.NganHangId != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.NganHangId == cauHinhPagination.NganHangId);
                }
                if(cauHinhPagination.MaNganHang != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.MaNganHang == cauHinhPagination.MaNganHang);
                }
                if(cauHinhPagination.TenNganHang != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.TenNganHang == cauHinhPagination.TenNganHang);
                }
                if(cauHinhPagination.LinkTransferUrl != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.LinkTransferUrl == cauHinhPagination.LinkTransferUrl);
                }
                if(cauHinhPagination.FTPPath != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.FTPPath == cauHinhPagination.FTPPath);
                }
                if(cauHinhPagination.FTPUserName != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.FTPUserName == cauHinhPagination.FTPUserName);
                }
                if(cauHinhPagination.FTPPassword != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.FTPPassword == cauHinhPagination.FTPPassword);
                }
                if(cauHinhPagination.EVNDSAPath != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.EVNDSAPath == cauHinhPagination.EVNDSAPath);
                }
                if(cauHinhPagination.PassPharseDSA != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.PassPhraseDSA == cauHinhPagination.PassPharseDSA);
                }
                if (cauHinhPagination.EVNRSAPath != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.EVNRSAPath == cauHinhPagination.EVNRSAPath);
                }
                if (cauHinhPagination.PassPharseRSA != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.PassPhraseRSA == cauHinhPagination.PassPharseRSA);
                }
                if (cauHinhPagination.BankDSAPath != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.BankDSAPath == cauHinhPagination.BankDSAPath);
                }
                if(cauHinhPagination.BankRSAPath != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.BankRSAPath == cauHinhPagination.BankRSAPath);
                }
                if(cauHinhPagination.BankApprover != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.BankApprover == cauHinhPagination.BankApprover);
                }
                if(cauHinhPagination.BankSenderAddr != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.BankSenderAddr == cauHinhPagination.BankSenderAddr);
                }
                if(cauHinhPagination.BankModel != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.BankModel == cauHinhPagination.BankModel);
                }
                if(cauHinhPagination.Version != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.Version == cauHinhPagination.Version);
                }
                if(cauHinhPagination.SoftwareProviderId != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.SoftwareProviderId == cauHinhPagination.SoftwareProviderId);
                }
                if(cauHinhPagination.BankLanguage != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.BankLanguage == cauHinhPagination.BankLanguage);
                }
                if(cauHinhPagination.BankAppointedApprover != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.BankAppointedApprover == cauHinhPagination.BankAppointedApprover);
                }
                if(cauHinhPagination.BankChannel != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.BankChannel == cauHinhPagination.BankChannel);
                }
                if(cauHinhPagination.BankMerchantId != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.BankMerchantId == cauHinhPagination.BankMerchantId);
                }
                if(cauHinhPagination.BankClientIP != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.BankClientIP == cauHinhPagination.BankClientIP);
                }
                if(cauHinhPagination.DaXoa != null)
                {
                    listCauHinh = listCauHinh.Where(s => s.DaXoa == cauHinhPagination.DaXoa);
                }

                var totalRecord = listCauHinh.ToList().Count();

                if(cauHinhPagination.PageIndex > 0)
                {
                    listCauHinh = listCauHinh.Skip(
                        cauHinhPagination.PageSize * (cauHinhPagination.PageIndex -1)
                    ).Take(
                        cauHinhPagination.PageSize
                    );
                }

                var response = listCauHinh.ToList();

                return new ResponseGetCauHinhNganHang("", "", true, totalRecord, response);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetCauHinhNganHang(e.Message, "001", false, 0, null);
            }
        }

        public List<CauHinhNganHangViewModel> GetCauHinhNganHang()
        {
            try
            {
                var listCauHinh = from ch in _context.CauHinhNganHang
                                select new CauHinhNganHangViewModel
                                {
                                    CauHinhNganHangId = ch.CauHinhNganHangId,
                                    NganHangId = ch.NganHang.NganHangId,
                                    MaNganHang = ch.NganHang.MaNganHang,
                                    TenNganHang = ch.NganHang.TenNganHang,
                                    LinkTransferUrl = ch.LinkTransferUrl,
                                    LinkQueryBaseUrl = ch.LinkQueryBaseUrl,
                                    FTPPath = ch.FTPPath,
                                    FTPUserName = ch.FTPUserName,
                                    FTPPassword = ch.FTPPassword,
                                    EVNDSAPath = ch.EVNDSAPath,
                                    PassPhraseDSA = ch.PassPhraseDSA,
                                    EVNRSAPath = ch.EVNRSAPath, 
                                    PassPhraseRSA = ch.PassPhraseRSA,
                                    BankDSAPath = ch.BankDSAPath,
                                    BankRSAPath = ch.BankRSAPath,
                                    BankApprover = ch.BankApprover,
                                    BankSenderAddr = ch.BankSenderAddr,
                                    BankModel = ch.BankModel,
                                    Version = ch.Version,
                                    SoftwareProviderId = ch.SoftwareProviderId,
                                    BankLanguage = ch.BankLanguage,
                                    BankAppointedApprover = ch.BankAppointedApprover,
                                    BankChannel = ch.BankChannel,
                                    BankMerchantId = ch.BankMerchantId,
                                    BankClientIP = ch.BankClientIP,
                                    DaXoa = ch.DaXoa
                                };
                listCauHinh = listCauHinh.OrderBy(x => x.MaNganHang);

                return listCauHinh.ToList();
            }
            catch(Exception e){
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public Response UpdateCauHinhNganHang(CauHinhNganHangViewModel cauHinh)
        {
            try
            {
                var item = _context.CauHinhNganHang.Where(
                    s => s.CauHinhNganHangId == cauHinh.CauHinhNganHangId
                ).FirstOrDefault();

                if(item == null){
                    return new Response(
                        message: "Không tìm thấy cấu hình ngân hàng", 
                        data: "", 
                        errorcode: "002", 
                        success: false
                    );
                }

                var nganHangItem = _context.NganHang.Where(
                    s => s.NganHangId == cauHinh.NganHangId
                ).FirstOrDefault();

                if(nganHangItem == null){
                    return new Response(
                        message: "Không tìm thấy ngân hàng", 
                        data: "", 
                        errorcode: "002", 
                        success: false
                    );
                }

                item.CauHinhNganHangId = cauHinh.CauHinhNganHangId;
                item.NganHang = nganHangItem;
                item.LinkTransferUrl = cauHinh.LinkTransferUrl;
                item.LinkQueryBaseUrl = cauHinh.LinkQueryBaseUrl;
                item.FTPPath = cauHinh.FTPPath;
                item.FTPUserName = cauHinh.FTPUserName;
                item.FTPPassword = cauHinh.FTPPassword;
                item.EVNDSAPath = cauHinh.EVNDSAPath;
                item.PassPhraseDSA = cauHinh.PassPhraseDSA;
                item.EVNRSAPath = cauHinh.EVNRSAPath;
                item.PassPhraseRSA = cauHinh.PassPhraseRSA;
                item.BankDSAPath = cauHinh.BankDSAPath;
                item.BankRSAPath = cauHinh.BankRSAPath;
                item.BankApprover = cauHinh.BankApprover;
                item.BankSenderAddr = cauHinh.BankSenderAddr;
                item.BankModel = cauHinh.BankModel;
                item.Version = cauHinh.Version;
                item.SoftwareProviderId = cauHinh.SoftwareProviderId;
                item.BankLanguage = cauHinh.BankLanguage;
                item.BankAppointedApprover = cauHinh.BankAppointedApprover;
                item.BankChannel = cauHinh.BankChannel;
                item.BankMerchantId = cauHinh.BankMerchantId;
                item.BankClientIP = cauHinh.BankClientIP;
                item.DaXoa = cauHinh.DaXoa;

                _context.SaveChanges();

                return new Response(
                    message: "Cập nhật thành công", 
                    data: "", 
                    errorcode: "", 
                    success: true
                );
            }
            catch(Exception e)
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
    }
}