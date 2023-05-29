using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BCXN.Data;
using BCXN.Repositories;
using BCXN.Statics;
using BCXN.ViewModels;
using Epayment.Repositories;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public class ChiTietGiayToHSTTService : IChiTietGiayToHSTTService
    {
        private readonly IChiTietGiayToHSTTRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILichSuChiTietGiayToService _ILichSuHoSoTT;
        private readonly IGiayToRepository _IGiayToRepos;
        private readonly ApplicationDbContext _context;
        private readonly IKySoTaiLieuService _IKySoTaiLieuService;
        private readonly IHoSoThanhToanRepository _IHSTTRepository;
        private readonly IUploadToFtpService _IUploadToFtpService;
        private readonly IUtilsService _utilsservice;

        public ChiTietGiayToHSTTService(IChiTietGiayToHSTTRepository repo, IMapper mapper, ILichSuChiTietGiayToService iLichSuHoSoTT,
        IGiayToRepository IGiayToRepos, ApplicationDbContext context, IUploadToFtpService IUploadToFtpService, IKySoTaiLieuService IKySoTaiLieuService, IHoSoThanhToanRepository IHSTTRepository,
        IUtilsService utilsservice)
        {
            _repo = repo;
            _mapper = mapper;
            _ILichSuHoSoTT = iLichSuHoSoTT;
            _IGiayToRepos = IGiayToRepos;
            _context = context;
            _IKySoTaiLieuService = IKySoTaiLieuService;
            _IHSTTRepository = IHSTTRepository;
            _IUploadToFtpService = IUploadToFtpService;
            _utilsservice = utilsservice;
        }

        public async Task<ResponsePostHSTTViewModel> CreateChiTietGiayToHSTT(ParmChiTietGiayToHSTTViewModel request, List<string> url)
        {
            List<string> lfile = new List<string>();
            ParmChiTietGiayToHSTTViewModel parmRequest = new ParmChiTietGiayToHSTTViewModel();
            var GetMaHoSoTT = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == request.HoSoThanhToanId);
            if (GetMaHoSoTT == null)
            {
                return new ResponsePostHSTTViewModel("Không tìm thấy hồ sơ", 400, null);
            }

            if (request.listFile != null)
            {
                foreach (var l in request.listFile)
                {
                    var result = await _IUploadToFtpService.UploadFileToFtpServer(l, GetMaHoSoTT.MaHoSo);
                    if (string.IsNullOrEmpty(result))
                    {
                        return new ResponsePostHSTTViewModel("Lỗi upload file lên server", 500, null);
                    }
                    lfile.Add(result.ToString());
                }
            }
            var ret = _repo.CreateChiTietGiayToHSTT(request, lfile);
            parmRequest = request;
            if (ret.idHoSoTT != null)
            {
                parmRequest.ChiTietHoSoId = ret.idHoSoTT;
                await _ILichSuHoSoTT.CreateLichSuChiTietGiayToHSTT(request, lfile);
            }

            return ret;
        }

        public async Task<ResponsePostViewModel> UpdateChiTietGiayToHSTT(ParmChiTietGiayToHSTTViewModel request, List<string> url)
        {
            var GetMaHoSoTT = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == request.HoSoThanhToanId);
            if (GetMaHoSoTT == null)
            {
                return new ResponsePostViewModel("Không tìm thấy hồ sơ", 400);
            }
            if (string.IsNullOrEmpty(request.ChiTietHoSoId) || request.ChiTietHoSoId == "null")
            {
                var result = await CreateChiTietGiayToHSTT(request, null);
                if (result.StatusCode == 200)
                {
                    return new ResponsePostViewModel("Cập nhật thành công", 200);
                }
                else
                {
                    return new ResponsePostViewModel(result.Message, 400);
                }
            }
            List<string> lfile = new List<string>();
            var fileDinhKem = _context.ChiTietGiayToHSTT.FirstOrDefault(x => x.ChiTietHoSoId.ToString() == request.ChiTietHoSoId).FileDinhKem.Split(",");
            if (fileDinhKem != null)
            {
                foreach (var f in fileDinhKem)
                {
                    if (request.urlFile != null)
                    {
                        foreach (var linkFile in request.urlFile)
                        {
                            if (f.Equals(linkFile))
                            {
                                lfile.Add(linkFile);
                            }
                        }
                    }
                    else
                    {
                        lfile.Add(f);
                    }
                }
            }
            if (request.listFile != null)
            {
                foreach (var l in request.listFile)
                {
                    var result = await _IUploadToFtpService.UploadFileToFtpServer(l, GetMaHoSoTT.MaHoSo);
                    lfile.Add(result.ToString());
                }
            }
            lfile = lfile.Distinct().ToList();
            var ret = _repo.UpdateChiTietGiayToHSTT(request, lfile);
            await _ILichSuHoSoTT.CreateLichSuChiTietGiayToHSTT(request, lfile);
            return ret;
        }
        public ResponsePostViewModel DeleteChiTietGiayToHSTT(Guid id)
        {
            var ret = _repo.DeleteChiTietGiayToHSTT(id);
            return ret;
        }

        public ResponseChiTietHSTTViewModel GetChiTietGiayToHSTT(ChiTietHSTTSearchViewModel request)
        {
            var loaiHoSoId = from hstt in _context.HoSoThanhToan
                             join LH in _context.LoaiHoSo on hstt.LoaiHoSo.LoaiHoSoId equals LH.LoaiHoSoId
                             where hstt.HoSoId == request.idHSTT
                             select new
                             {
                                 LH.LoaiHoSoId,
                                 hstt.NguoiTao.Id
                             };
            Guid LoaiHoSoId;
            var ret = from Ctgt in _context.ChiTietGiayToHSTT
                      where Ctgt.HoSoThanhToan.HoSoId == request.idHSTT
                      select new
                      {
                          Ctgt.GiayTo.GiayToId
                      };
            List<Guid> ListGiayToId = new List<Guid>();
            ListGiayToId = ret.Select(x => x.GiayToId).ToList();
            List<ChiTietGiayToHSTTViewModel> kq = new List<ChiTietGiayToHSTTViewModel>();
            if (loaiHoSoId.Count() > 0)
            {
                LoaiHoSoId = loaiHoSoId.First().LoaiHoSoId;
                var ItemGiayTo = _IGiayToRepos.GetGiayToByIdLoaiHoSo(LoaiHoSoId);
                List<Guid> listGiayToIdByLH = new List<Guid>();
                if (ItemGiayTo != null)
                {
                    foreach (var item in ItemGiayTo)
                    {
                        listGiayToIdByLH.Add(item.Id);
                        var ChiTietHSTTList = from cthstt in _context.ChiTietGiayToHSTT
                                              join htt in _context.HoSoThanhToan on cthstt.HoSoThanhToan.HoSoId equals htt.HoSoId
                                              //join LHS in _context.LoaiHoSo on htt.LoaiHoSo.LoaiHoSoId  equals LHS.LoaiHoSoId into emp
                                              where cthstt.TrangThaiGiayTo != -1 && cthstt.HoSoThanhToan.HoSoId == request.idHSTT && cthstt.GiayTo.GiayToId == item.Id
                                              select new ChiTietGiayToHSTTViewModel
                                              {
                                                  ChiTietHoSoId = cthstt.ChiTietHoSoId,
                                                  IdHoSoTT = htt.HoSoId,
                                                  LoaiHoSoId = htt.LoaiHoSo.LoaiHoSoId,
                                                  TenHoSoTT = htt.TenHoSo,
                                                  IdGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).GiayToId,
                                                  TenGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).TenGiayTo,
                                                  Nguon = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).Nguon,
                                                  TrangThaiGiayTo = cthstt.TrangThaiGiayTo,
                                                  MaGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).MaGiayTo,
                                                  FileDinhKem = cthstt.FileDinhKem,
                                                  NgayCapNhat = cthstt.NgayCapNhat,
                                                  NguoiCapNhat = cthstt.NguoiCapNhat.FirstName + " " + cthstt.NguoiCapNhat.LastName,
                                                  ThuTu = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayTo.GiayToId == cthstt.GiayTo.GiayToId && x.LoaiHoSo.LoaiHoSoId == htt.LoaiHoSo.LoaiHoSoId).ThuTu,
                                                  BatBuoc = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayTo.GiayToId == cthstt.GiayTo.GiayToId && x.LoaiHoSo.LoaiHoSoId == htt.LoaiHoSo.LoaiHoSoId).BatBuoc
                                              };
                        if (ChiTietHSTTList.FirstOrDefault() != null)
                        {
                            kq.Add(ChiTietHSTTList.FirstOrDefault());
                        }
                        else
                        {
                            ChiTietGiayToHSTTViewModel newO = new ChiTietGiayToHSTTViewModel();
                            newO.ChiTietHoSoId = null;
                            newO.IdHoSoTT = request.idHSTT;
                            newO.LoaiHoSoId = LoaiHoSoId;
                            newO.IdGiayTo = item.Id;
                            newO.TenGiayTo = item.TenGiayTo;
                            newO.Nguon = item.Nguon;
                            newO.TrangThaiGiayTo = item.TrangThai;
                            newO.MaGiayTo = item.MaGiayTo;
                            newO.FileDinhKem = null;
                            newO.NgayCapNhat = DateTime.Now;
                            newO.NguoiCapNhat = null;
                            newO.ThuTu = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayTo.GiayToId == item.Id && x.LoaiHoSo.LoaiHoSoId == LoaiHoSoId).ThuTu;
                            newO.BatBuoc = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayTo.GiayToId == item.Id && x.LoaiHoSo.LoaiHoSoId == LoaiHoSoId).BatBuoc;
                            kq.Add(newO);
                        }
                    }
                }
                var MS = ListGiayToId.Except(listGiayToIdByLH).ToList();
                if (MS != null)
                {
                    foreach (var item in MS)
                    {
                        var ChiTietHSTTList = from cthstt in _context.ChiTietGiayToHSTT
                                              join htt in _context.HoSoThanhToan on cthstt.HoSoThanhToan.HoSoId equals htt.HoSoId
                                              //join LHS in _context.LoaiHoSo on htt.LoaiHoSo.LoaiHoSoId  equals LHS.LoaiHoSoId into emp
                                              where cthstt.TrangThaiGiayTo != -1 && cthstt.HoSoThanhToan.HoSoId == request.idHSTT && cthstt.GiayTo.GiayToId == item
                                              select new ChiTietGiayToHSTTViewModel
                                              {
                                                  ChiTietHoSoId = cthstt.ChiTietHoSoId,
                                                  IdHoSoTT = htt.HoSoId,
                                                  LoaiHoSoId = htt.LoaiHoSo.LoaiHoSoId,
                                                  TenHoSoTT = htt.TenHoSo,
                                                  IdGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).GiayToId,
                                                  TenGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).TenGiayTo,
                                                  Nguon = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).Nguon,
                                                  TrangThaiGiayTo = cthstt.TrangThaiGiayTo,
                                                  MaGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).MaGiayTo,
                                                  FileDinhKem = cthstt.FileDinhKem,
                                                  NgayCapNhat = cthstt.NgayCapNhat,
                                                  NguoiCapNhat = cthstt.NguoiCapNhat.FirstName + " " + cthstt.NguoiCapNhat.LastName,
                                                  ThuTu = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayTo.GiayToId == cthstt.GiayTo.GiayToId && x.LoaiHoSo.LoaiHoSoId == htt.LoaiHoSo.LoaiHoSoId).ThuTu,
                                                  BatBuoc = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayTo.GiayToId == cthstt.GiayTo.GiayToId && x.LoaiHoSo.LoaiHoSoId == htt.LoaiHoSo.LoaiHoSoId).BatBuoc
                                              };
                        kq.Add(ChiTietHSTTList.FirstOrDefault());
                    }
                }
            }
            return new ResponseChiTietHSTTViewModel(kq, 200, kq.Count());
        }

        public ResponsePostViewModel DeleteChiTietGiayToByIdHSTT(Guid HoSoTTId)
        {

            var ret = _repo.DeleteChiTietGiayToByIdHSTT(HoSoTTId);
            return ret;
        }

        public async Task<Response> CreatePhieuThamTraHSTT(ParmPhieuThamTraHSTTViewModel request, string url)
        {
            try
            {
                if (request.File != null)
                {
                    var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet.FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == request.ThaoTacBuocPheDuyetId);
                    if (thaoTacBuocPheDuyet == null)
                    {
                        throw new Exception($"Không tìm thấy thao tác bước phê duyệt có Id = {request.ThaoTacBuocPheDuyetId}");
                    }

                    var checkPTT = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == request.HoSoThanhToanId);
                    if (checkPTT.TrangThaiHoSo == BCXN.Statics.TrangThaiHoSo.ThamTraHoSo ||
                    checkPTT.TrangThaiHoSo == BCXN.Statics.TrangThaiHoSo.YeuCauThaiDoi)
                    {
                        var checkFile = _context.ChiTietGiayToHSTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId.ToString() == request.HoSoThanhToanId && x.GiayTo.GiayToId == thaoTacBuocPheDuyet.GiayToId);
                        if (checkFile != null)
                        {
                            _context.ChiTietGiayToHSTT.Remove(checkFile);
                            if (!string.IsNullOrEmpty(checkFile.FileDinhKem))
                            {
                                if (System.IO.File.Exists(checkFile.FileDinhKem))
                                {
                                    System.IO.File.Delete(checkFile.FileDinhKem);
                                }
                            }
                        }

                        // var fileName = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + DateTime.Now.Millisecond + request.File.FileName;
                        // var folderName = Path.Combine("wwwroot", "fileChiTietGiayToHoSTT");
                        // var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName.Replace(",", "").Trim());
                        // var stream = new FileStream(pathToSave, FileMode.Create);
                        // await request.File.CopyToAsync(stream);
                        // stream.Dispose();
                        // var pathFile = pathToSave.ToString().Split("wwwroot");
                        // var result = "wwwroot" + pathFile[pathFile.Length - 1];
                        var pathFile = await _IUploadToFtpService.UploadFileToFtpServer(request.File, checkPTT.MaHoSo);
                        var result = pathFile.ToString();
                        var ret = _repo.CreatePhieuThamTraHSTT(request, result);
                        KySoPhieuThamTraPram ksptt = new KySoPhieuThamTraPram();
                        ksptt.HoSoThanhToanId = new Guid(request.HoSoThanhToanId);
                        ksptt.CapKy = TrangThaiChiTietGiayToThamTra.TrinhKyCap1;
                        ksptt.GiayToId = thaoTacBuocPheDuyet.GiayToId;
                        ksptt.NoiDungKy = "Đồng ý";
                        ksptt.TaiLieuGoc = result;
                        ksptt.NguoiKyId = request.NguoiCapNhatId;
                        ksptt.ThaoTacBuocPheDuyetId = request.ThaoTacBuocPheDuyetId; //thao tác bước phê duyệt
                        var kc1 = _IKySoTaiLieuService.KySoPhieuThamTraHoSo(ksptt);
                        if (kc1.Success == false)
                        {
                            var upDateCapKy = _IHSTTRepository.UpdateTrangThaiHsttById(new Guid(request.HoSoThanhToanId), BCXN.Statics.TrangThaiHoSo.YeuCauThaiDoi);
                            _utilsservice.SendMailKy(request.ThaoTacBuocPheDuyetId, new Guid(request.HoSoThanhToanId));
                            return new Response(message: kc1.Message, data: null, errorcode: "05", success: false);
                        }
                        return ret;
                    }
                    else
                    {
                        return new Response(message: "Hồ sơ đang được thẩm tra ", data: null, errorcode: "06", success: false);
                    }
                }
                else
                {
                    return new Response(message: "Lỗi lưu file", data: null, errorcode: "04", success: false);
                }
            }
            catch (Exception ex)
            {
                return new Response(message: ex.Message, data: null, errorcode: "03", success: false);
            }
        }
    }
}