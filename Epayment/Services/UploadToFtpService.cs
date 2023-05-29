using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Renci.SshNet;

namespace Epayment.Services
{
    public interface IUploadToFtpService
    {
        int UploadToFtp(string nameFile, string OutFileSignEncrypt);
        int UploadToSftp(string inputFile, string fileName);
        Task<string> UploadFileToSftpServer(IFormFile inputFile, string pathFolder);
        FileStream DownloadFileToSftpServer(string pathFile);
        Task<string> UploadFileToFtpServer(IFormFile inputFile, string pathFolder);
        Response DownloadFileFtpServerConvertBase64(string pathFile);
        Response DownloadFileToSftpServerConvertBase64(string pathFile);
        string UploadByteFileFTP(byte[] inputFile, string pathFile, string namFile, string TypeFile);
        string UploadByteFileSFTP(byte[] inputFile, string pathFolder, string namFile, string TypeFile);
        string FtpUploadTxt(string textContent, string pathFolder, string namFile, string TypeFile);
        string SftpUploadTxt(string textContent, string pathFolder, string namFile, string TypeFile);
    }
    public class UploadToFtpService : IUploadToFtpService
    {
        private readonly ILogger<UploadToFtpService> _logger;
        private readonly IConfiguration _configuration;

        public UploadToFtpService(IConfiguration configuration, ILogger<UploadToFtpService> logger)
        {
            _configuration = configuration;
            _logger = logger;
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
                _logger.LogWarning($"[UploadToFtp] lỗi: {ex}");
                return 0;
            }
            return 1;
        }
        public int UploadToSftp(string inputFile, string fileName)
        {
            string host = _configuration.GetSection("PathFTP:host").Value;
            int port = Convert.ToInt32(_configuration.GetSection("PathFTP:port").Value);
            string username = _configuration.GetSection("PathFTP:userName").Value;
            string password = _configuration.GetSection("PathFTP:password").Value;
            string pathFolderVCB = _configuration.GetSection("PathFTP:pathFileVCB").Value;
            //string pathFolder = _configuration.GetSection("PathFTP:pathFile").Value;
            // var connectionInfo = new Renci.SshNet.ConnectionInfo(host, "sftp", new PasswordAuthenticationMethod(username, password));

            // Upload File
            try
            {
                //var connectionInfo = new ConnectionInfo(host, "sftp", new PasswordAuthenticationMethod(username, password));
                using (var client = new SftpClient(host, port, username, password))
                {
                    //string fileinput = _env.ContentRootPath + "\\" + @"FileServer\raw4.encrypt";
                    //string filename = "raw4.encrypt";
                    client.Connect();
                    client.ChangeDirectory(pathFolderVCB);
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
                _logger.LogWarning($"[UploadToSftp] lỗi: {ex}");
                return 0;
            }
            return 1;
        }
        public async Task<string> UploadFileToSftpServer(IFormFile inputFile, string pathFolder)
        {
            string host = _configuration.GetSection("FileServer:host").Value;
            int port = Convert.ToInt32(_configuration.GetSection("FileServer:port").Value);
            string username = _configuration.GetSection("FileServer:userName").Value;
            string password = _configuration.GetSection("FileServer:password").Value;
            try
            {
                using (var client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        var pathFileServer = pathFolder;
                        // var pathFileServer = pathFolder + "/" + DateTime.Now.Day
                        // + DateTime.Now.Month
                        // + DateTime.Now.Year;
                        if (!client.Exists(pathFileServer))
                        {
                            client.CreateDirectory(pathFileServer);
                        }
                        else
                        {
                            client.ChangeDirectory(pathFileServer);
                        }

                        var fileName = DateTime.Now.Day
                        + "-" + DateTime.Now.Month
                        + "-" + DateTime.Now.Year
                        + "-" + DateTime.Now.Hour
                        + "-" + DateTime.Now.Minute
                        + "-" + DateTime.Now.Second + DateTime.Now.Millisecond
                        + "_" + inputFile.FileName.ToString().Replace(" ", "-").Replace("_", "-");

                        var memoryStream = new MemoryStream();
                        await inputFile.CopyToAsync(memoryStream);
                        // using (Stream uplfileStream = memoryStream )
                        // {
                        //     client.UploadFile(uplfileStream, Name);
                        // }
                        using (Stream uplfileStream = inputFile.OpenReadStream())
                        {
                            client.UploadFile(uplfileStream, fileName);
                        }
                        _logger.LogInformation("Da day file thanh cong: ", fileName);
                        client.Disconnect();
                        return pathFileServer + "/" + fileName;
                    }
                    return "";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[UploadFileToSftpServer] lỗi: {ex}");
                return "";
            }
        }

        public async Task<string> UploadFileToFtpServer(IFormFile inputFile, string pathFolder)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var fileName = DateTime.Now.Day
                        + "-" + DateTime.Now.Month
                        + "-" + DateTime.Now.Year
                        + "-" + DateTime.Now.Hour
                        + "-" + DateTime.Now.Minute
                        + "-" + DateTime.Now.Second + DateTime.Now.Millisecond
                        + "_" + inputFile.FileName.ToString().Replace(" ", "-").Replace("_", "-");

                    var pathFileFolder = _configuration.GetSection("FileServer:pathFile").Value + pathFolder;
                    var userName = _configuration.GetSection("FileServer:userName").Value;
                    var password = _configuration.GetSection("FileServer:password").Value;
                    if (FtpDirectoryExists(pathFileFolder) == true)
                    {
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(pathFileFolder + "/" + fileName);
                        request.Credentials = new NetworkCredential(userName, password);
                        request.Method = WebRequestMethods.Ftp.UploadFile;

                        using (Stream ftpStream = request.GetRequestStream())
                        {
                            await inputFile.CopyToAsync(ftpStream);
                        }
                        var arrayPathFile = (pathFileFolder + "/" + fileName).ToString().Split(pathFolder);
                        var result = pathFolder + arrayPathFile[arrayPathFile.Length - 1];
                        return result;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[UploadFileToFtpServer] lỗi: {ex}");
                return "";
            }
        }
        public bool FtpDirectoryExists(string directoryPath)
        {
            bool IsExists = true;
            try
            {
                //create the directory
                var userName = _configuration.GetSection("FileServer:userName").Value;
                var password = _configuration.GetSection("FileServer:password").Value;
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(directoryPath);
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential(userName, password);
                using (var resp = (FtpWebResponse)requestDir.GetResponse())
                {
                    IsExists = true;
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    IsExists = true;
                }
                else
                {
                    response.Close();
                    IsExists = false;
                }
            }
            return IsExists;
        }

        public FileStream DownloadFileToSftpServer(string pathFile)
        {
            string host = _configuration.GetSection("FileServer:host").Value;
            int port = Convert.ToInt32(_configuration.GetSection("FileServer:port").Value);
            string username = _configuration.GetSection("FileServer:userName").Value;
            string password = _configuration.GetSection("FileServer:password").Value;
            string localFileName = System.IO.Path.GetFileName(pathFile);
            using (var sftp = new SftpClient(host, port, username, password))
            {
                sftp.Connect();
                var file = File.OpenWrite(localFileName);
                //byte [] test =  sftp.ReadAllBytes(localFileName);
                sftp.DownloadFile(pathFile, file);
                sftp.Disconnect();
                return file;
            }
        }

        public Response DownloadFileToSftpServerConvertBase64(string pathFile)
        {
            try
            {
                string host = _configuration.GetSection("FileServer:host").Value;
                int port = Convert.ToInt32(_configuration.GetSection("FileServer:port").Value);
                string username = _configuration.GetSection("FileServer:userName").Value;
                string password = _configuration.GetSection("FileServer:password").Value;
                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();
                    byte[] bytes = sftp.ReadAllBytes(pathFile);
                    string file = Convert.ToBase64String(bytes);
                    // UploadByteFileSFTP(bytes, "Epayment-Test", "test", "doc");

                    // StringBuilder sb = new StringBuilder("");
                    // sb.Append("aaa,cccc \n");
                    // sb.Append("bbbbb,dddd \n");
                    // SftpUploadTxt(sb.ToString(),"Epayment-Test", "test", "csv");
                    sftp.Disconnect();
                    return new Response(message: "", data: file, errorcode: "00", success: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[DownloadFileToSftpServerConvertBase64] lỗi: {ex}");
                return new Response(message: "lỗi tải file ", data: "", errorcode: "01", success: false);
            }

        }

        public Response DownloadFileFtpServerConvertBase64(string pathFile)
        {
            try
            {
                //Create FTP Request.
                var pathFileFolder = _configuration.GetSection("FileServer:pathFile").Value + pathFile;
                string username = _configuration.GetSection("FileServer:userName").Value;
                string password = _configuration.GetSection("FileServer:password").Value;

                var req = new WebClient();
                req.Credentials = new NetworkCredential(username, password);
                byte[] bytes = req.DownloadData(pathFileFolder);
                string file = Convert.ToBase64String(bytes);

                // test upload file với đầu vào là text
                // StringBuilder sb = new StringBuilder("");
                // sb.Append("aaa,cccc \n");
                // sb.Append("bbbbb,dddd \n");
                // FtpUploadTxt(sb.ToString() ,"Epayment-Test", "test", "csv" );
                //test upload file với đầu vào là byte
                //UploadByteFileFTP(bytes, "Epayment-Test", "test", "doc");

                // FtpWebRequest request = (FtpWebRequest)WebRequest.Create(pathFileFolder);
                // request.Method = WebRequestMethods.Ftp.DownloadFile;
                // request.Credentials = new NetworkCredential(username, password);

                // using (Stream response = request.GetResponse().GetResponseStream()){
                //     var arrayPathFile = pathFile.Split("/");
                //     var pathFileLocal = @"wwwroot/" + arrayPathFile[0];
                //     if (!Directory.Exists(pathFileLocal))
                //     {
                //        Directory.CreateDirectory(pathFileLocal);
                //     }
                //     if (!System.IO.File.Exists(pathFileLocal+"/"+ arrayPathFile[1])){
                //         var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), pathFileLocal, arrayPathFile[1]);
                //         var stream = new FileStream(pathToSave, FileMode.Create);
                //         response.CopyToAsync(stream);
                //     }
                //     //var tets = pathFile.Replace("Epayment-Test/", "");
                //     // using (Stream fileStream = File.Create(@"wwwroot\fileChiTietGiayToHoSTT\" + tets ))
                //     // {
                //     //     response.CopyTo(fileStream);
                //     // }   
                // } 
                return new Response(message: "", data: file, errorcode: "00", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[UploadToFtpServer] lỗi: {ex}");
                return new Response(message: "lỗi tải file ", data: "", errorcode: "01", success: false);
            }

        }

        public string UploadByteFileFTP(byte[] inputFile, string pathFolder, string namFile, string TypeFile)
        {
            try
            {
                var pathFileFolder = _configuration.GetSection("FileServer:pathFile").Value + pathFolder;
                string username = _configuration.GetSection("FileServer:userName").Value;
                string password = _configuration.GetSection("FileServer:password").Value;
                var fileName = DateTime.Now.Day
                           + "-" + DateTime.Now.Month
                           + "-" + DateTime.Now.Year
                           + "-" + DateTime.Now.Hour
                           + "-" + DateTime.Now.Minute
                           + "-" + DateTime.Now.Second + DateTime.Now.Millisecond
                           + "_" + namFile.ToString().Replace(" ", "-").Replace("_", "-")
                           + "." + TypeFile;
                byte[] fileData = inputFile;
                if (FtpDirectoryExists(pathFileFolder) == true)
                {
                    var ftpWebRequest = WebRequest.Create(pathFileFolder + "/" + fileName);
                    ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                    ftpWebRequest.Credentials = new NetworkCredential(username, password);
                    using (var requestStream = ftpWebRequest.GetRequestStream())
                    {
                        requestStream.Write(fileData, 0, fileData.Length);
                        requestStream.Close();
                    }
                    //var response = ftpWebRequest.GetResponse();
                    var arrayPathFile = (pathFileFolder + "/" + fileName).ToString().Split(pathFolder);
                    var result = pathFolder + arrayPathFile[arrayPathFile.Length - 1];
                    return result;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[UploadByteFileFTP] lỗi: {ex}");
                return "";
            }
        }

        public string UploadByteFileSFTP(byte[] inputFile, string pathFolder, string namFile, string TypeFile)
        {
            string host = _configuration.GetSection("FileServer:host").Value;
            int port = Convert.ToInt32(_configuration.GetSection("FileServer:port").Value);
            string username = _configuration.GetSection("FileServer:userName").Value;
            string password = _configuration.GetSection("FileServer:password").Value;
            try
            {
                using (var client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        var pathFileServer = pathFolder;
                        if (!client.Exists(pathFileServer))
                        {
                            client.CreateDirectory(pathFileServer);
                        }
                        else
                        {
                            client.ChangeDirectory(pathFileServer);
                        }

                        var fileName = DateTime.Now.Day
                        + "-" + DateTime.Now.Month
                        + "-" + DateTime.Now.Year
                        + "-" + DateTime.Now.Hour
                        + "-" + DateTime.Now.Minute
                        + "-" + DateTime.Now.Second + DateTime.Now.Millisecond
                        + "_" + namFile.ToString().Replace(" ", "-").Replace("_", "-")
                        + "." + TypeFile;
                        // var stream = new MemoryStream();
                        // stream.Write(inputFile, 0, inputFile.Length);
                        // stream.Position = 0;
                        // client.UploadFile(stream, pathFileServer);
                        client.WriteAllBytes(pathFileServer + "/" + fileName, inputFile);
                        _logger.LogInformation("Da day file thanh cong: ", fileName);
                        client.Disconnect();
                        return pathFileServer + "/" + fileName;
                    }
                    return "";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[UploadByteFileSFTP] lỗi: {ex}");
                return "";
            }
        }
        public string FtpUploadTxt(string textContent, string pathFolder, string namFile, string TypeFile)
        {
            try
            {
                var pathFileFolder = _configuration.GetSection("FileServer:pathFile").Value + pathFolder;
                string username = _configuration.GetSection("FileServer:userName").Value;
                string password = _configuration.GetSection("FileServer:password").Value;
                var fileName = DateTime.Now.Day
                               + "-" + DateTime.Now.Month
                               + "-" + DateTime.Now.Year
                               + "-" + DateTime.Now.Hour
                               + "-" + DateTime.Now.Minute
                               + "-" + DateTime.Now.Second + DateTime.Now.Millisecond
                               + "_" + namFile.ToString().Replace(" ", "-").Replace("_", "-")
                               + "." + TypeFile;
                if (FtpDirectoryExists(pathFileFolder) == true)
                {
                    var ftpWebRequest = WebRequest.Create(pathFileFolder + "/" + fileName);
                    ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                    ftpWebRequest.Credentials = new NetworkCredential(username, password);
                    ftpWebRequest.ContentLength = textContent.Length;
                    using (Stream request_stream = ftpWebRequest.GetRequestStream())
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(textContent);
                        request_stream.Write(bytes, 0, textContent.Length);
                        request_stream.Close();
                    }
                    var arrayPathFile = (pathFileFolder + "/" + fileName).ToString().Split(pathFolder);
                    var result = pathFolder + arrayPathFile[arrayPathFile.Length - 1];
                    return result;
                }
                return "";
            }
            catch (Exception ex)
            {
                 _logger.LogWarning($"[UploadFileAsync] lỗi: {ex}");
                return "";
            }
        }
        public string SftpUploadTxt(string textContent, string pathFolder, string namFile, string TypeFile)
        {
            string host = _configuration.GetSection("FileServer:host").Value;
            int port = Convert.ToInt32(_configuration.GetSection("FileServer:port").Value);
            string username = _configuration.GetSection("FileServer:userName").Value;
            string password = _configuration.GetSection("FileServer:password").Value;
            try
            {
                using (var client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        var pathFileServer = pathFolder;
                        if (!client.Exists(pathFileServer))
                        {
                            client.CreateDirectory(pathFileServer);
                        }
                        else
                        {
                            client.ChangeDirectory(pathFileServer);
                        }
                        var fileName = DateTime.Now.Day
                        + "-" + DateTime.Now.Month
                        + "-" + DateTime.Now.Year
                        + "-" + DateTime.Now.Hour
                        + "-" + DateTime.Now.Minute
                        + "-" + DateTime.Now.Second + DateTime.Now.Millisecond
                        + "_" + namFile.ToString().Replace(" ", "-").Replace("_", "-")
                        + "." + TypeFile;
                        client.WriteAllText(pathFileServer + "/" + fileName, textContent);
                        _logger.LogInformation("Da day file thanh cong: ", fileName);
                        client.Disconnect();
                        return pathFileServer + "/" + fileName;
                    }
                    return "";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[UploadByteFileSFTP] lỗi: {ex}");
                return "";
            }
        }
        public async Task<string> UploadFileAsync(Microsoft.AspNetCore.Http.IFormFile fileUpload, string pathFile)
        {
            try
            {
                var fileName = DateTime.Now.Day
                    + "-" + DateTime.Now.Month
                    + "-" + DateTime.Now.Year
                    + "-" + DateTime.Now.Hour
                    + "-" + DateTime.Now.Minute
                    + "_" + DateTime.Now.Second + DateTime.Now.Millisecond + fileUpload.FileName.ToString().Replace(" ", "-");
                //var folderName = Path.Combine("wwwroot", "fileChiTietGiayToHoSTT");
                //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName.Replace(",", "-"));
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), pathFile, fileName.Replace(",", "-"));
                var stream = new FileStream(pathToSave, FileMode.Create);
                await fileUpload.CopyToAsync(stream);
                stream.Dispose();
                var folderName = pathToSave.ToString().Split(pathFile);
                var result = folderName + folderName[folderName.Length - 1];
                return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning($"[UploadFileAsync] lỗi: {e}");
                return "";
            }
        }
    }
}