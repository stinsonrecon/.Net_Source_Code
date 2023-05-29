using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Reflection;
using APIERP.DataContext;
using APIERP.ViewModels;
using Serilog;

namespace APIERP.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IConnectionContext _context;

        public PaymentRepository(IConnectionContext context)
        {
            _context = context;
        }

        public ResponsePostView Pc_Evn_Update_Payment_Status(PaymentView paymentView)
        {
            try
            {
                Log.Information("{@input}", paymentView);
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
                    cmd.CommandText = "evn_epay_pkg.Pc_Evn_Update_Payment_Status";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_Trans_Id", OracleDbType.Int64).Value = paymentView.MaChungTuERP;
                    cmd.Parameters.Add("p_Segment1", OracleDbType.Varchar2).Value = paymentView.MaDonViERP;
                    cmd.Parameters.Add("p_Ma_Hstt", OracleDbType.Varchar2).Value = paymentView.MaHSTT;
                    cmd.Parameters.Add("p_Trans_Date", OracleDbType.Varchar2).Value = paymentView.NgayChuyenNganHang;
                    cmd.Parameters.Add("p_Gl_Date", OracleDbType.Varchar2).Value = paymentView.NgayNganHangThanhToan;
                    cmd.Parameters.Add("p_Sign1", OracleDbType.Varchar2).Value = paymentView.ChuKy1;
                    cmd.Parameters.Add("p_Sign2", OracleDbType.Varchar2).Value = paymentView.ChuKy2;
                    cmd.Parameters.Add("p_Sign3", OracleDbType.Varchar2).Value = paymentView.ChuKy3;
                    cmd.Parameters.Add("p_Sign4", OracleDbType.Varchar2).Value = paymentView.ChuKy4;
                    cmd.Parameters.Add("p_Trang_Thai_Hstt", OracleDbType.Varchar2).Value = paymentView.TrangThaiHSTT;
                    cmd.Parameters.Add("v_Result", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                    var tmp = cmd.ExecuteNonQuery();
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
                                return new ResponsePostView("Cập nhật thành công", 1);
                            }
                            else if (propValue.Equals("0"))
                            {
                                return new ResponsePostView("Cập nhật không thành công", 2);
                            }
                        }
                    }
                }
                return new ResponsePostView("Lỗi server", 0);
            }
            catch (Exception ex)
            {
                Log.Error("{@error}", ex);
                return new ResponsePostView("Lỗi server", 0);
            }
        }
    }
}
