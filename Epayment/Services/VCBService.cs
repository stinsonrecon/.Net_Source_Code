using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using BCXN.ViewModels;
using Epayment.Repositories;
using Epayment.ViewModels;
using EVNSample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using BCXN.Statics;
using System.Security.Cryptography;
using Epayment.Models;

namespace Epayment.Services
{
    public interface IVCBService
    {
        Response ChuyenKhoanVCB(YeuCauChiTienParams ycct, ChungTu chungtuItem);
    }
    public class VCBService : IVCBService
    {
        private readonly ILogger<VCBService> _logger;
        private readonly IYeuCauChiTienRepository _repo;
        private readonly IChungTuRepository _ctrepo;
        private readonly IHoSoThanhToanRepository _hsttrepo;
        //private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUploadToFtpService _IUploadToFtp;
        public VCBService(IYeuCauChiTienRepository repo, IChungTuRepository ctrepo, IHoSoThanhToanRepository hsttrepo, IConfiguration configuration, ILogger<VCBService> logger, IUploadToFtpService IUploadToFtp)
        {
            _repo = repo;
            _ctrepo = ctrepo;
            _hsttrepo = hsttrepo;
            _configuration = configuration;
            _logger = logger;
            _IUploadToFtp = IUploadToFtp;
        }

        public Response ChuyenKhoanVCB(YeuCauChiTienParams ycct, ChungTu chungtuItem)
        {
            try
            {
                var resp = _repo.CreateYeuCauChiTien(
                new YeuCauChiTienParams
                {
                    ChungTuId = ycct.ChungTuId,
                    NguoiYeuCauChiId = ycct.NguoiYeuCauChiId,
                    NgayGiaoDichThucTe = ycct.NgayGiaoDichThucTe
                }
                );
                if (resp.Success == false)
                {
                    _logger.LogWarning($"[CreateYeuCauChiTien]Không cập nhật được dữ liệu chi tiền vào db cho chứng từ: {ycct.ChungTuId}");
                    return new Response(message: "Không cập nhật được dữ liệu chi tiền vào db", data: "", errorcode: "201", success: false);
                }
                else
                {
                    Guid requestID = (Guid)resp.Data;
                    ycct.YeuCauChiTienId = requestID;

                    string feeAccount = chungtuItem.SoTaiKhoanChuyen;
                    string feeType = chungtuItem.LoaiPhi;
                    DateTime now = DateTime.Now;
                    Random ranSTT = new Random();
                    var nameFile = _configuration.GetSection("RSAkeySign:VCB:nameFileVCB").Value + now.ToString("ddMMyyyy") + "_" + now.Day + now.Month + now.Year + now.Hour + now.Minute + now.Second + now.Millisecond + "EVN" + chungtuItem.DonViThanhToan.ERPMaCongTy + "_" + ranSTT.Next(1, 100);
                    string fileName = _configuration.GetSection("PathTransferMoney:PathInPutTransferMoney").Value + "Decrypt_" + checkName(nameFile + ".csv");
                    string OutFileNameSign = _configuration.GetSection("PathTransferMoney:PathInPutTransferMoney").Value + "Encrypt_" + checkName(nameFile + ".sign");
                    string OutFileSignEncrypt = _configuration.GetSection("PathTransferMoney:PathOutPutTransferMoney").Value + checkName(nameFile + ".encrypt");

                    Dictionary<string, object> payload = new Dictionary<string, object>();
                    payload.Add("PaymentSerialNumber", 1);
                    payload.Add("TransactionTypeCode", chungtuItem.LoaiGiaoDich); //blank
                    payload.Add("PaymentType", chungtuItem.LoaiGiaoDichCha); //BT
                    payload.Add("CustomerReferenceNo", ycct.YeuCauChiTienId);
                    payload.Add("DebitAccountNo", chungtuItem.SoTaiKhoanChuyen);
                    payload.Add("BeneficiaryAccountNo", chungtuItem.SoTaiKhoanNhan);
                    payload.Add("BeneficiaryName", chungtuItem.TenTaiKhoanNhan);
                    payload.Add("DocumentID", "");
                    payload.Add("PlaceofIssue", "");
                    payload.Add("IDIssuanceDate", "");
                    payload.Add("BeneficiaryBankSwiftCodeIFSCCode", chungtuItem.MaChiNhanhNhan);
                    payload.Add("TransactionCurrency", chungtuItem.LoaiTienTe);
                    payload.Add("PaymentAmount", chungtuItem.SoTien.ToString().Replace(".00", ""));
                    payload.Add("ChargeType", chungtuItem.LoaiPhi);
                    if (chungtuItem.NoiDungTT != null && chungtuItem.NoiDungTT.Trim() != "")
                        payload.Add("Paymentdetails", BCXN.StringUtils.RemoveSign4VietnameseString(chungtuItem.NoiDungTT).Replace(",", ""));
                    else
                        payload.Add("Paymentdetails", chungtuItem.NoiDungTT);
                    payload.Add("PurposeCode", "1");
                    payload.Add("ValueDate", chungtuItem.NgayGiaoDichThucTe.ToString("dd/MM/yyyy"));
                    payload.Add("BeneficiaryID", "");
                    payload.Add("BeneficiaryAddr1", "");
                    payload.Add("BeneficiaryAddr2", "");


                    using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
                    {
                        try
                        {
                            List<string> header = new List<string>();
                            int icol = payload.Count;
                            int i = 0;
                            foreach (var keyVar in payload.Keys)
                            {
                                i++;
                                header.Add(keyVar);
                                if (i < icol)
                                    writer.Write(keyVar + ",");
                                else
                                    writer.Write(keyVar);
                            }
                            writer.WriteLine();
                            i = 0;
                            foreach (var item in header)
                            {
                                i++;
                                foreach (var keyVar in payload.Keys)
                                {
                                    if (item == keyVar)
                                    {
                                        if (i < icol)
                                            writer.Write(payload[keyVar] + ",");
                                        else
                                            writer.Write(payload[keyVar]);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                            {
                                ChungTuId = ycct.ChungTuId,
                                YeuCauChiTienId = ycct.YeuCauChiTienId,
                                TrangThaiChi = TrangThaiChiTien.LoiTaoFile,
                                ChuKy = "",
                                MaKetQuaChi = "Lỗi tạo ghi file"
                            });
                            _logger.LogWarning($"[CreateYeuCauChuyenKhoanVCB] lỗi: {ex}");
                            return new Response(message: "Lỗi chưa xác định", data: ex.Message, errorcode: "005", success: false);
                        }
                    }
                    var secretKeyFile = _configuration.GetSection("RSAkeySign:VCB:EVNSceretDSA").Value;
                    var PathfilePucblucKey = _configuration.GetSection("RSAkeySign:VCB:VCBPublicRSA").Value;
                    var passPhrase = _configuration.GetSection("RSAkeySign:VCB:passPhraseDSA").Value;

                    PGPEncryptSign.Sign(fileName, secretKeyFile, passPhrase, OutFileNameSign);
                    PGPEncryptSign.Encrypt(OutFileNameSign, PathfilePucblucKey, OutFileSignEncrypt);//Ma hoa file

                    // using (var client = new WebClient())
                    // {
                    //     client.Credentials = new NetworkCredential(_configuration.GetSection("PathFTP:userName").Value, _configuration.GetSection("PathFTP:password").Value);
                    //     client.UploadFile(_configuration.GetSection("PathFTP:pathFile").Value + nameFile, WebRequestMethods.Ftp.UploadFile, OutFileSignEncrypt);
                    // }
                    string sPathFolderSFTP = _configuration.GetSection("PathFTP:pathFile").Value;
                    if (_IUploadToFtp.UploadToSftp(OutFileSignEncrypt, nameFile + ".encrypt") == 0)
                    {
                        _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                        {
                            ChungTuId = ycct.ChungTuId,
                            YeuCauChiTienId = ycct.YeuCauChiTienId,
                            TrangThaiChi = TrangThaiChiTien.LoiDayFile,
                            ChuKy = "",
                            MaKetQuaChi = "Lỗi upload file sftp"
                        });
                        _logger.LogWarning($"[CreateYeuCauChuyenKhoanVCB] lỗi không upload được file");
                        return new Response(message: "Lỗi không upload được file", data: "Không upload được file", errorcode: "004", success: false);
                    }
                    // tạo chữ kí số
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(_configuration["RSAkey:private"]), out _);
                    ASCIIEncoding ByteConverter = new ASCIIEncoding();
                    string original_data = ycct.YeuCauChiTienId.ToString();
                    byte[] signedByte = rsa.SignData(ByteConverter.GetBytes(original_data), SHA256.Create());
                    string signedString = Convert.ToBase64String(signedByte);

                    _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                    {
                        ChungTuId = ycct.ChungTuId,
                        TrangThaiChi = TrangThaiChiTien.GuiLenhChiTienThanhCong,
                        YeuCauChiTienId = requestID,
                        MaKetQuaChi = "Đã gửi lệnh chi",
                        ChuKy = signedString
                    });

                    return new Response(message: "Gửi lệnh chi tiền thành công", data: "", errorcode: "", success: true);
                }
            }
            catch (Exception ex)
            {
                _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                {
                    ChungTuId = ycct.ChungTuId,
                    YeuCauChiTienId = ycct.YeuCauChiTienId,
                    TrangThaiChi = TrangThaiChiTien.LoiTaoFile,
                    ChuKy = "",
                    MaKetQuaChi = "Lỗi chưa xác định"
                });
                _logger.LogWarning($"[CreateYeuCauChuyenKhoanVCB] lỗi: {ex}");
                return new Response(message: "Lỗi chưa xác định", data: ex.Message, errorcode: "003", success: false);
            }
        }
        public string checkName(string nameFile)
        {
            var listName = nameFile.Split("_");
            var file = listName.Last();
            var number = file.Split(".");
            var STT = number.First();
            if (System.IO.File.Exists(_configuration.GetSection("PathTransferMoney:PathOutPutTransferMoney").Value + nameFile))
            {
                nameFile = _configuration.GetSection("RSAkeySign:VCB:nameFileVCB").Value + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + "_" + (Convert.ToInt32(STT) + 1);
                return checkName(nameFile);
            }
            return nameFile;
        }
    }
}