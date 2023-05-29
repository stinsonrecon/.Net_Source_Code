using System;
using System.IO;

using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Bcpg;
using BCXN.ViewModels;
// using AutoMapper.Configuration;
using AutoMapper;
using Epayment.Repositories;
using Microsoft.Extensions.Configuration;
using Epayment.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using EVNSample;
using System.Net;
using Renci.SshNet;
using Microsoft.AspNetCore.Hosting;
using BCXN.Statics;

namespace Epayment.Services
{
    public interface IMaHoaPGDService
    {
        Response Encrypt(string filePath, string publicKeyFile, string outputFilePath);
        Response Decrypt(string filePath, string privateKeyFile, string passPhrase, string outputFilePath);
        Response CreateYeuCauChuyenKhoanVCB(YeuCauChiTienParams ycct);
    }
    public class MaHoaPGDService : IMaHoaPGDService
    {
        private readonly ILogger<MaHoaPGDService> _logger;
        private readonly IYeuCauChiTienRepository _repo;
        private readonly IChungTuRepository _ctrepo;
        private readonly IHoSoThanhToanRepository _hsttrepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private IHostingEnvironment _env;

        public MaHoaPGDService(IYeuCauChiTienRepository repo, IChungTuRepository ctrepo, IHoSoThanhToanRepository hsttrepo, IMapper mapper, IConfiguration configuration, ILogger<MaHoaPGDService> logger, IHostingEnvironment env)
        {
            _repo = repo;
            _ctrepo = ctrepo;
            _hsttrepo = hsttrepo;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _env = env;
        }
        private PgpPublicKey ReadPublicKey(Stream inputStream)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);
            PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(inputStream);
            foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey k in kRing.GetPublicKeys())
                {
                    if (k.IsEncryptionKey)
                    {
                        return k;
                    }
                }
            }
            throw new ArgumentException("Can't find encryption key in key ring.");
        }

        private PgpPrivateKey FindSecretKey(PgpSecretKeyRingBundle pgpSec, long keyId, char[] pass)
        {
            PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyId);
            if (pgpSecKey == null)
            {
                return null;
            }
            // lấy nội dung khoá
            return pgpSecKey.ExtractPrivateKey(pass);
        }

        public void DecryptFile(Stream inputStream, Stream keyIn, char[] passwd, string outputFilePath)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);
            PgpObjectFactory pgpF = new PgpObjectFactory(inputStream);
            PgpEncryptedDataList enc;
            PgpObject o = pgpF.NextPgpObject();
            if (o is PgpEncryptedDataList list)
            {
                enc = list;
            }
            else
            {
                enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
            }

            PgpPrivateKey sKey = null;
            PgpPublicKeyEncryptedData pbe = null;
            PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(keyIn));
            foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
            {
                sKey = FindSecretKey(pgpSec, pked.KeyId, passwd);
                if (sKey != null)
                {
                    pbe = pked;
                    break;
                }
            }
            if (sKey == null)
            {
                throw new ArgumentException("Secret key for message not found.");
            }
            Stream clear = pbe.GetDataStream(sKey);
            PgpObjectFactory plainFact = new PgpObjectFactory(clear);
            PgpObject message = plainFact.NextPgpObject();
            if (message is PgpCompressedData cData)
            {
                PgpObjectFactory pgpFact = new PgpObjectFactory(cData.GetDataStream());
                message = pgpFact.NextPgpObject();
            }

            if (message is PgpLiteralData data)
            {
                using (Stream fOut = File.Create(outputFilePath))
                {
                    Stream unc = data.GetInputStream();
                    Streams.PipeAll(unc, fOut);
                }
            }
            else if (message is PgpOnePassSignatureList)
            {
                throw new PgpException("Encrypted message contains a signed message - not literal data.");
            }
            else
            {
                throw new PgpException("Message is not a simple encrypted file - type unknown.");
            }

        }
        public void EncryptFile(Stream outputStream, string fileName, PgpPublicKey encKey, bool armor, bool withIntegrityCheck)
        {
            if (armor)
            {
                outputStream = new ArmoredOutputStream(outputStream);
            }

            try
            {
                MemoryStream bOut = new MemoryStream();
                PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
                PgpUtilities.WriteFileToLiteralData(comData.Open(bOut), PgpLiteralData.Binary, new FileInfo(fileName));
                comData.Close();

                PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());
                cPk.AddMethod(encKey);

                byte[] bytes = bOut.ToArray();
                Stream cOut = cPk.Open(outputStream, bytes.Length);
                cOut.Write(bytes, 0, bytes.Length);
                cOut.Close();

                if (armor)
                {
                    outputStream.Close();
                }
            }
            catch (PgpException e)
            {
                _logger.LogError("Error: ", e);
            }
        }

        public Response Encrypt(string filePath, string publicKeyFile, string outputFilePath)
        {
            // RSAkey
            //publicKeyFile = _configuration["RSAkey:public"]
            //publicKeyFile = _configuration.GetSection("RSAkey:public").Value;
            try
            {
                Stream keyStream = File.OpenRead(publicKeyFile);
                PgpPublicKey keyIn = ReadPublicKey(keyStream);
                Stream fos = File.Create(outputFilePath);
                EncryptFile(fos, filePath, keyIn, true, true);
                keyStream.Close();
                fos.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                // Utils.WriteLog("Error :" + '\t' + "Decrypt file error", ex.Message);
                return new Response(message: "", data: null, errorcode: "", success: false);
            }
            return new Response(message: "", data: null, errorcode: "", success: true);
        }

        public Response Decrypt(string filePath, string privateKeyFile, string passPhrase, string outputFilePath)
        {
            try
            {
                using (Stream fin = File.OpenRead(filePath))
                {
                    using (Stream keyIn = File.OpenRead(privateKeyFile))
                    {
                        DecryptFile(fin, keyIn, passPhrase.ToCharArray(), outputFilePath);
                    }
                }
                return new Response(message: "", data: null, errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                // Utils.WriteLog("Error :" + '\t' + "Decrypt file error", ex.Message);
                return new Response(message: "", data: null, errorcode: "", success: false);
            }
        }

        public Response CreateYeuCauChuyenKhoanVCB(YeuCauChiTienParams ycct)
        {
            try
            {
                var chungtuItem = _ctrepo.GetChungTuByID(ycct.ChungTuId);
                string feeAccount = chungtuItem.SoTaiKhoanChuyen;
                string feeType = chungtuItem.LoaiPhi;

                DateTime now = DateTime.Now;

                Random ranSTT = new Random();
                var nameFile = _configuration.GetSection("RSAkeySign:VCB:nameFileVCB").Value + now.ToString("ddMMyyyy") + "_" + now.Day + now.Month + now.Year + now.Hour + now.Minute + now.Second + now.Millisecond + "EVN" + chungtuItem.DonViThanhToan.ERPMaCongTy + "_" + ranSTT.Next(1, 100);
                string fileName = @"wwwroot\inPutRequestTransferMoney\Decrypt_" + checkName(nameFile + ".csv");
                Console.WriteLine(nameFile);
                string OutFileNameSign = @"wwwroot\inPutRequestTransferMoney\Encrypt_" + checkName(nameFile + ".sign");
                string OutFileSignEncrypt = @"wwwroot\OutRequestTransferMoney\" + checkName(nameFile + ".encrypt");

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
                //Neu ngay giao dich thuc te nho hon ngay hien tai thi lay ngay hien tai
                if (chungtuItem.NgayGiaoDichThucTe != null && chungtuItem.NgayGiaoDichThucTe.Date > DateTime.Now.Date)
                    payload.Add("ValueDate", chungtuItem.NgayGiaoDichThucTe.ToString("dd/MM/yyyy"));
                else
                    payload.Add("ValueDate", DateTime.Now.ToString("dd/MM/yyyy"));
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
                        // RSACryptoServiceProvider rsaa = new RSACryptoServiceProvider();
                        // rsaa.ImportRSAPrivateKey(Convert.FromBase64String(_configuration["RSAkey:private"]), out _);
                        // ASCIIEncoding ByteConvertera = new ASCIIEncoding();
                        // byte[] signedBytea = rsaa.SignData(ByteConvertera.GetBytes(txtSignature), SHA256.Create());
                        // string signedStringa = Convert.ToBase64String(signedBytea);
                        // writer.Write(signedStringa);
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
                        _logger.LogError("Error: ", ex);
                        return new Response(message: "Lỗi chưa xác định", data: ex.Message, errorcode: "005", success: false);
                    }
                }
                var secretKeyFile = _configuration.GetSection("RSAkeySign:VCB:EVNSceretDSA").Value;
                var PathfilePucblucKey = _configuration.GetSection("RSAkeySign:VCB:VCBPublicRSA").Value;
                var passPhrase = _configuration.GetSection("RSAkeySign:VCB:passPhraseDSA").Value;

                PGPEncryptSign.Sign(fileName, secretKeyFile, passPhrase, OutFileNameSign);
                PGPEncryptSign.Encrypt(OutFileNameSign, PathfilePucblucKey, OutFileSignEncrypt);//Ma hoa file

                // var createEncrypt = Encrypt(fileName, PathfilePucblucKey, OutFileName);
                // if ( createEncrypt.Success != true){
                //     return new Response(message: "Lỗi tạo file chuyển tiền", data: "", errorcode: "006", success: false);
                // }
                //Encrypt(fileName, PathfilePucblucKey, OutFileName);

                // using (var client = new WebClient())
                // {
                //     client.Credentials = new NetworkCredential(_configuration.GetSection("PathFTP:userName").Value, _configuration.GetSection("PathFTP:password").Value);
                //     client.UploadFile(_configuration.GetSection("PathFTP:pathFile").Value + nameFile, WebRequestMethods.Ftp.UploadFile, OutFileSignEncrypt);
                // }
                string sPathFolderSFTP = _configuration.GetSection("PathFTP:pathFile").Value;
                if (UploadToSftp(OutFileSignEncrypt, sPathFolderSFTP, nameFile + ".encrypt") == 0)
                {
                    _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                    {
                        ChungTuId = ycct.ChungTuId,
                        YeuCauChiTienId = ycct.YeuCauChiTienId,
                        TrangThaiChi = TrangThaiChiTien.LoiDayFile,
                        ChuKy = "",
                        MaKetQuaChi = "Lỗi upload file sftp"
                    });
                    _logger.LogError("loi khong upload duoc file");
                    return new Response(message: "Lỗi không upload được file", data: "Không upload được file", errorcode: "004", success: false);
                }
                //if (UploadToFtp(nameFile + ".encrypt", OutFileSignEncrypt) == 0)
                //{
                //    _logger.LogError("Error: ", "Lỗi không upload được file");
                //    return new Response(message: "Lỗi không upload được file", data: "", errorcode: "004", success: false);
                //}
                return new Response(message: "Thêm mới thành công", data: "", errorcode: "", success: true);
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
                _logger.LogError("Error: ", ex);
                return new Response(message: "Lỗi chưa xác định", data: ex.Message, errorcode: "003", success: false);
            }
        }
        public string checkName(string nameFile)
        {
            var listName = nameFile.Split("_");
            var file = listName.Last();
            var number = file.Split(".");
            var STT = number.First();
            if (System.IO.File.Exists(@"wwwroot\OutRequestTransferMoney\" + nameFile))
            {
                nameFile = _configuration.GetSection("RSAkeySign:VCB:nameFileVCB").Value + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + "_" + (Convert.ToInt32(STT) + 1);
                return checkName(nameFile);
            }
            // else
            // {
            //     return nameFile;
            // }
            return nameFile;
        }
        public int UploadToFtp(string nameFile, string OutFileSignEncrypt)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_configuration.GetSection("PathFTP:userName").Value, _configuration.GetSection("PathFTP:password").Value);
                    client.UploadFile(_configuration.GetSection("PathFTP:pathFile").Value + nameFile, WebRequestMethods.Ftp.UploadFile, OutFileSignEncrypt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return 0;
            }
            return 1;
        }
        public int UploadToSftp(string inputFile, string pathFolder, string fileName)
        {
            string host = _configuration.GetSection("PathFTP:host").Value;
            int port = Convert.ToInt32(_configuration.GetSection("PathFTP:port").Value);
            string username = _configuration.GetSection("PathFTP:userName").Value;
            string password = _configuration.GetSection("PathFTP:password").Value;
            //string pathFolder = _configuration.GetSection("PathFTP:pathFile").Value;
            // var connectionInfo = new Renci.SshNet.ConnectionInfo(host, "sftp", new PasswordAuthenticationMethod(username, password));

            // Upload File
            try
            {
                var connectionInfo = new ConnectionInfo(host, "sftp", new PasswordAuthenticationMethod(username, password));
                using (var client = new SftpClient(host, port, username, password))
                {
                    //string fileinput = _env.ContentRootPath + "\\" + @"FileServer\raw4.encrypt";
                    //string filename = "raw4.encrypt";
                    client.Connect();
                    client.ChangeDirectory("/VIETCOMBANK/OUT");
                    if (client.IsConnected)
                    {
                        //return Ok("Ket noi thanh cong");
                        using (var fileStream = new FileStream(inputFile, FileMode.Open))
                        {
                            client.BufferSize = 4 * 1024;
                            client.UploadFile(fileStream, fileName);
                        }
                        _logger.LogInformation("Da day file thanh cong: ", fileName);
                        client.Disconnect();
                    }
                }

            }


            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return 0;
            }
            return 1;
        }
    }
}