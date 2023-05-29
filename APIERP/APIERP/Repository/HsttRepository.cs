using Microsoft.AspNetCore.Connections;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using APIERP.DataContext;
using APIERP.ViewModels;
using Serilog;

namespace APIERP.Repository
{
    public class HsttRepository : IHsttRepository
    {
        private readonly IConnectionContext _context;

        public HsttRepository(IConnectionContext context)
        {
            _context = context;
        }

        public ResponsePostView Pc_Evn_Insert_Hstt(HsttView hsttView)
        {
            try
            {
                Log.Information("{@input}", hsttView);
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
                    cmd.CommandText = "evn_epay_pkg.Pc_Evn_Insert_Hstt";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_User_Id", OracleDbType.Varchar2).Value = hsttView.UserName;
                    cmd.Parameters.Add("p_Segment1", OracleDbType.Varchar2).Value = hsttView.MaDonViERP;
                    cmd.Parameters.Add("p_Segment2", OracleDbType.Varchar2).Value = hsttView.MaChiNhanh;
                    cmd.Parameters.Add("p_Trang_Thai_Ap", OracleDbType.Varchar2).Value = hsttView.TrangThaiAp;
                    cmd.Parameters.Add("p_Trang_Thai_Gl", OracleDbType.Varchar2).Value = hsttView.TrangThaiGl;
                    cmd.Parameters.Add("p_Trang_Thai_Cm", OracleDbType.Varchar2).Value = hsttView.TrangThaiCm;
                    cmd.Parameters.Add("p_Ngay_Ct", OracleDbType.Varchar2).Value = hsttView.NgayChungTu;
                    cmd.Parameters.Add("p_Noi_Dung_Tt", OracleDbType.Varchar2).Value = hsttView.NoiDungThanhToan;
                    cmd.Parameters.Add("p_Ma_Hstt", OracleDbType.Varchar2).Value = hsttView.MaHSTT;
                    cmd.Parameters.Add("p_Ten_Hstt", OracleDbType.Varchar2).Value = hsttView.TenHSTT;
                    cmd.Parameters.Add("p_Tk_Thu_Huong", OracleDbType.Varchar2).Value = hsttView.TaiKhoanNguoiThuHuong;
                    cmd.Parameters.Add("p_Nguoi_Thu_Huong", OracleDbType.Varchar2).Value = hsttView.TenNguoiThuHuong;
                    cmd.Parameters.Add("p_Loai_Tien", OracleDbType.Varchar2).Value = hsttView.LoaiTien;
                    cmd.Parameters.Add("p_Ty_Gia", OracleDbType.Decimal).Value = hsttView.TyGia;
                    cmd.Parameters.Add("p_Sotien_Nguyente", OracleDbType.Decimal).Value = hsttView.SoTienNguyenTe;
                    cmd.Parameters.Add("p_Sotien_Quydoi", OracleDbType.Decimal).Value = hsttView.SoTienQuyDoi;
                    cmd.Parameters.Add("p_Trang_Thai_Hstt", OracleDbType.Varchar2).Value = hsttView.TrangThaiHSTT;
                    cmd.Parameters.Add("p_Id_Chinhanh_Thu_Huong", OracleDbType.Varchar2).Value = hsttView.MaChiNhanhNhan;
                    cmd.Parameters.Add("v_Result", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                    var tmp = cmd.ExecuteNonQuery();
                    Console.WriteLine(tmp);
                    result = cmd.Parameters["v_result"].Value;
                    Log.Information("{@output}", result);
                    PropertyInfo[] props = result.GetType().GetProperties();

                    foreach (PropertyInfo prop in props)
                    {
                        if(prop.Name == "Value")
                        {
                            object propValue = prop.GetValue(result, null);
                            if(propValue.Equals("1"))
                            {
                                return new ResponsePostView("Thêm mới thành công", 1);
                            }
                            else if (propValue.Equals("0"))
                            {
                                return new ResponsePostView("Thêm mới không thành công", 2);
                            }
                        }
                    }
                }
                return new ResponsePostView("Lỗi server", 0);
            }
            catch(Exception ex)
            {
                Log.Error("{@error}", ex);
                return new ResponsePostView("Lỗi server", 0);
            }
        }
    }

}

