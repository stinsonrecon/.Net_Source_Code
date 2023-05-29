using APIERP.ViewModels;
using System;
using Serilog;
using Oracle.ManagedDataAccess.Client;
using APIERP.DataContext;
using System.Data;
using System.Reflection;

namespace APIERP.Repository
{
    public class NganHangParams
    {
        public string TenNganHang { get; set; }
    }
    public class ChiNhanhNganHangParams
    {
        public string TenChiNhanh { get; set; }
    }
    public class TaiKhoanNganHangParams
    {
        public string SoTaiKhoan { get; set; }
        public string UserId { get; set; }
        public string MaDonViERP { get; set; }
        public string MaChiNhanhNganHang { get; set; }
        public string TenNganHang { get; set; }
        public string TenChiNhanhNganHang { get; set; }
        public string SwiftCode { get; set; }
        public string DiaChi { get; set; }
        public string ThanhPho { get; set; }
        public string QuocGia { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public bool OldActive { get; set; }
        public string TenNganHangVietTat { get; set; }
        public string LoaiTaiKhoan { get; set; }
        public bool LaTaiKhoanNhan { get; set; }
        public string TenTaiKhoan { get; set; }
    }
    public interface INganHangRepository 
    {
        Response Insert_Tai_Khoan_Ngan_Hang(TaiKhoanNganHangParams tk);
        Response Insert_Chi_Nhanh_Ngan_Hang(ChiNhanhNganHangParams cn);
        Response Insert_Ngan_Hang(NganHangParams nh);
        Response Update_Tai_Khoan_Ngan_Hang_Nhan(TaiKhoanNganHangParams tk);
    }
    public class NganHangRepository : INganHangRepository
    {
        private readonly IConnectionContext _context;

        public NganHangRepository(IConnectionContext context)
        {
            _context = context;
        }
        public Response Update_Tai_Khoan_Ngan_Hang_Nhan(TaiKhoanNganHangParams tk)
        {
            try
            {
                //Log.Information("{@input}", tk);
                object result = null;
                OracleCommand cmd = new OracleCommand();

                OracleConnection conn = _context.GetConnection();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "EVN_CM_EPAY_C03_PKG.UPDATE_ROW";

                    cmd.Parameters.Add("X_RECV_BANK_ACCOUNT", OracleDbType.Varchar2).Value = tk.SoTaiKhoan;
                    cmd.Parameters.Add("X_LOOKUP_TYPE", OracleDbType.Varchar2).Value = tk.LoaiTaiKhoan;
                    cmd.Parameters.Add("X_RECV_BRANCH_NAME", OracleDbType.Varchar2).Value = tk.TenChiNhanhNganHang;
                    cmd.Parameters.Add("X_RECV_SWIFT_CODE", OracleDbType.Varchar2).Value = tk.SwiftCode;
                    cmd.Parameters.Add("X_RECV_NAME", OracleDbType.Varchar2).Value = tk.TenTaiKhoan;
                    cmd.Parameters.Add("X_RECV_ADDRESS", OracleDbType.Varchar2).Value = tk.DiaChi;
                    cmd.Parameters.Add("X_RECV_CITY", OracleDbType.Varchar2).Value = tk.ThanhPho;
                    cmd.Parameters.Add("X_RECV_COUNTRY", OracleDbType.Varchar2).Value = tk.QuocGia;
                    cmd.Parameters.Add("X_RECV_PHONENO", OracleDbType.Varchar2).Value = tk.SoDienThoai;
                    cmd.Parameters.Add("X_RECV_EMAIL", OracleDbType.Varchar2).Value = tk.Email;
                    if (tk.OldActive == true) cmd.Parameters.Add("X_ENABLED_FLAG", OracleDbType.Varchar2).Value = "Y";
                    else cmd.Parameters.Add("X_ENABLED_FLAG", OracleDbType.Varchar2).Value = "N";
                    if (tk.Active == true) cmd.Parameters.Add("X_ENABLED_FLAG_NEW", OracleDbType.Varchar2).Value = "Y";
                    else cmd.Parameters.Add("X_ENABLED_FLAG_NEW", OracleDbType.Varchar2).Value = "N";

                    //cmd.Parameters.Add("X_RECV_BANK_ACCOUNT", OracleDbType.Varchar2).Value = "209823094";
                    //cmd.Parameters.Add("X_LOOKUP_TYPE", OracleDbType.Varchar2).Value = "CITAD";
                    //cmd.Parameters.Add("X_RECV_BRANCH_NAME", OracleDbType.Varchar2).Value = "Tràng An";
                    //cmd.Parameters.Add("X_RECV_SWIFT_CODE", OracleDbType.Varchar2).Value = "01202024";
                    //cmd.Parameters.Add("X_RECV_NAME", OracleDbType.Varchar2).Value = "FKASJFIKSD";
                    //cmd.Parameters.Add("X_RECV_ADDRESS", OracleDbType.Varchar2).Value = "kjsdjhkfkjashdfsdf";
                    //cmd.Parameters.Add("X_RECV_CITY", OracleDbType.Varchar2).Value = "Hồ Chí Minh";
                    //cmd.Parameters.Add("X_RECV_COUNTRY", OracleDbType.Varchar2).Value = "Việt Nam";
                    //cmd.Parameters.Add("X_RECV_PHONENO", OracleDbType.Varchar2).Value = "092384902349";
                    //cmd.Parameters.Add("X_RECV_EMAIL", OracleDbType.Varchar2).Value = "askldfjksld@gmail.com";
                    //cmd.Parameters.Add("X_ENABLED_FLAG", OracleDbType.Varchar2).Value = "Y";
                    //cmd.Parameters.Add("X_ENABLED_FLAG_NEW", OracleDbType.Varchar2).Value = "Y";

                    cmd.Parameters.Add("v_Result", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                    var tmp = cmd.ExecuteNonQuery();
                    Console.WriteLine(tmp);
                    result = cmd.Parameters["v_result"].Value;
                    Log.Information("{@output}", result);
                    PropertyInfo[] props = result.GetType().GetProperties();

                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.Name == "Value")
                        {
                            object propValue = prop.GetValue(result, null);
                            if (propValue.Equals("1"))
                            {
                                return new Response(message: "Thêm mới thành công", data: "", errorcode: "", success: true);
                            }
                            else if (propValue.Equals("0"))
                            {
                                return new Response(message: "Không insert được dữ liệu", data: "", errorcode: "001", success: false);
                            }
                        }
                    }
                }
                return new Response(message: "Không kết nối được tới server", data: "", errorcode: "002", success: false);
            }
            catch (Exception ex)
            {
                Log.Error("{@error}", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "003", success: false);
            }
        }

        public Response Insert_Tai_Khoan_Ngan_Hang(TaiKhoanNganHangParams tk)
        {
            try
            {
                Log.Information("{@input}", tk);
                object result = null;
                OracleCommand cmd = new OracleCommand();

                OracleConnection conn = _context.GetConnection();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (tk.LaTaiKhoanNhan == false)
                    {
                        cmd.CommandText = "Evn_epay_pkg.Pc_Evn_Insert_Sender_Account";

                        cmd.Parameters.Add("p_User_Id", OracleDbType.Varchar2).Value = tk.UserId;
                        cmd.Parameters.Add("p_Segment1", OracleDbType.Varchar2).Value = tk.MaDonViERP;
                        cmd.Parameters.Add("p_Segment2", OracleDbType.Varchar2).Value = tk.MaChiNhanhNganHang;
                        cmd.Parameters.Add("p_Sender_Bank_Account", OracleDbType.Varchar2).Value = tk.SoTaiKhoan;
                        cmd.Parameters.Add("p_Sender_Bank_Name", OracleDbType.Varchar2).Value = tk.TenNganHang;
                        cmd.Parameters.Add("p_Sender_Branch_Name", OracleDbType.Varchar2).Value = tk.TenChiNhanhNganHang;
                        cmd.Parameters.Add("p_Sender_Swift_Code", OracleDbType.Varchar2).Value = tk.SwiftCode;
                        cmd.Parameters.Add("p_Sender_Address", OracleDbType.Varchar2).Value = tk.DiaChi;
                        cmd.Parameters.Add("p_Sender_City", OracleDbType.Varchar2).Value = tk.ThanhPho;
                        cmd.Parameters.Add("p_Sender_Country", OracleDbType.Varchar2).Value = tk.QuocGia;
                        cmd.Parameters.Add("p_Sender_Phoneno", OracleDbType.Varchar2).Value = tk.SoDienThoai;
                        cmd.Parameters.Add("p_Sender_Email", OracleDbType.Varchar2).Value = tk.Email;
                        if(tk.Active == true) cmd.Parameters.Add("p_Enabled_Flag", OracleDbType.Varchar2).Value = "Y";
                        else cmd.Parameters.Add("p_Enabled_Flag", OracleDbType.Varchar2).Value = "N";
                        cmd.Parameters.Add("p_Bank_Name_Erp", OracleDbType.Varchar2).Value = tk.TenNganHangVietTat;
                        cmd.Parameters.Add("p_Look_Type", OracleDbType.Varchar2).Value = tk.LoaiTaiKhoan;
                    }
                    else
                    {
                        cmd.CommandText = "EVN_CM_EPAY_C03_PKG.INSERT_ROW";

                        cmd.Parameters.Add("X_RECV_BANK_ACCOUNT", OracleDbType.Varchar2).Value = tk.SoTaiKhoan;
                        cmd.Parameters.Add("X_LOOKUP_TYPE", OracleDbType.Varchar2).Value = tk.LoaiTaiKhoan;
                        cmd.Parameters.Add("X_RECV_BANK_NAME", OracleDbType.Varchar2).Value = tk.TenNganHang;
                        cmd.Parameters.Add("X_RECV_BRANCH_NAME", OracleDbType.Varchar2).Value = tk.TenChiNhanhNganHang;
                        cmd.Parameters.Add("X_RECV_SWIFT_CODE", OracleDbType.Varchar2).Value = tk.SwiftCode;
                        cmd.Parameters.Add("X_RECV_NAME", OracleDbType.Varchar2).Value = tk.TenTaiKhoan;
                        cmd.Parameters.Add("X_RECV_ADDRESS", OracleDbType.Varchar2).Value = tk.DiaChi;
                        cmd.Parameters.Add("X_RECV_CITY", OracleDbType.Varchar2).Value = tk.ThanhPho;
                        cmd.Parameters.Add("X_RECV_COUNTRY", OracleDbType.Varchar2).Value = tk.QuocGia;
                        cmd.Parameters.Add("X_RECV_PHONENO", OracleDbType.Varchar2).Value = tk.SoDienThoai;
                        cmd.Parameters.Add("X_RECV_EMAIL", OracleDbType.Varchar2).Value = tk.Email;
                        if (tk.Active == true) cmd.Parameters.Add("X_ENABLED_FLAG", OracleDbType.Varchar2).Value = "Y";
                        else cmd.Parameters.Add("X_ENABLED_FLAG", OracleDbType.Varchar2).Value = "N";
                    }
                    

                    cmd.Parameters.Add("v_Result", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                    var tmp = cmd.ExecuteNonQuery();
                    Console.WriteLine(tmp);
                    result = cmd.Parameters["v_result"].Value;
                    Log.Information("{@output}", result);
                    PropertyInfo[] props = result.GetType().GetProperties();

                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.Name == "Value")
                        {
                            object propValue = prop.GetValue(result, null);
                            if (propValue.Equals("1"))
                            {
                                return new Response(message: "Thêm mới thành công", data: "", errorcode: "", success: true);
                            }
                            else if (propValue.Equals("0"))
                            {
                                return new Response(message: "Không insert được dữ liệu", data: "", errorcode: "001", success: false);
                            }
                        }
                    }
                }
                return new Response(message: "Không kết nối được tới server", data: "", errorcode: "002", success: false);
            }
            catch (Exception ex)
            {
                Log.Error("{@error}", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "003", success: false);
            }
        }
        public Response Insert_Chi_Nhanh_Ngan_Hang(ChiNhanhNganHangParams cn)
        {
            try
            {
                Log.Information("{@input}", cn);
                object result = null;
                OracleCommand cmd = new OracleCommand();

                OracleConnection conn = _context.GetConnection();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "evn_epay_pkg.yyy";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("test", OracleDbType.Varchar2).Value = cn.TenChiNhanh;
                    cmd.Parameters.Add("v_Result", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                    var tmp = cmd.ExecuteNonQuery();
                    Console.WriteLine(tmp);
                    result = cmd.Parameters["v_result"].Value;
                    Log.Information("{@output}", result);
                    PropertyInfo[] props = result.GetType().GetProperties();

                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.Name == "Value")
                        {
                            object propValue = prop.GetValue(result, null);
                            if (propValue.Equals("1"))
                            {
                                return new Response(message: "Thêm mới thành công", data: "", errorcode: "", success: true);
                            }
                            else if (propValue.Equals("0"))
                            {
                                return new Response(message: "Không insert được dữ liệu", data: "", errorcode: "001", success: false);
                            }
                        }
                    }
                }
                return new Response(message: "Không kết nối được tới server", data: "", errorcode: "002", success: false);
            }
            catch (Exception ex)
            {
                Log.Error("{@error}", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "003", success: false);
            }
        }

        public Response Insert_Ngan_Hang(NganHangParams nh) 
        {
            try
            {
                Log.Information("{@input}", nh);
                object result = null;
                OracleCommand cmd = new OracleCommand();

                OracleConnection conn = _context.GetConnection();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "evn_epay_pkg.xxx";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("test", OracleDbType.Varchar2).Value = nh.TenNganHang;
                    cmd.Parameters.Add("v_Result", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                    var tmp = cmd.ExecuteNonQuery();
                    Console.WriteLine(tmp);
                    result = cmd.Parameters["v_result"].Value;
                    Log.Information("{@output}", result);
                    PropertyInfo[] props = result.GetType().GetProperties();

                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.Name == "Value")
                        {
                            object propValue = prop.GetValue(result, null);
                            if (propValue.Equals("1"))
                            {
                                return new Response(message: "Thêm mới thành công", data: "", errorcode: "", success: true);
                            }
                            else if (propValue.Equals("0"))
                            {
                                return new Response(message: "Không insert được dữ liệu", data: "", errorcode: "001", success: false);
                            }
                        }
                    }
                }
                return new Response(message: "Không kết nối được tới server", data: "", errorcode: "002", success: false);
            }
            catch (Exception ex)
            {
                Log.Error("{@error}", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "003", success: false);
            }
        }
    }
}
