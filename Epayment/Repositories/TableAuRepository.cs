using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCXN.Repositories
{
    public class TableAuRepository : ITableAuRepository
    {
        private readonly Data.ApplicationDbContext _context;
        public TableAuRepository(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public ViewModels.SlideInfo GetSlideInfoByID(int bcid)
        {
            try
            {
                var resp = (from bc in _context.BaoCao 
                            join dv in _context.DonVi on bc.DonViId equals dv.Id
                            join lv in _context.LinhVucBaoCao on bc.LinhVucId equals lv.Id
                            where bc.Id == bcid
                            select new ViewModels.SlideInfo
                            {
                                LinhVucBaoCao = lv.TieuDe,
                                TenBaoCao = bc.TenBaoCao,
                                PhongBan = dv.TenDonVi,
                                OrderBaoCao = lv.Order
                            }).FirstOrDefault();

                if (resp == null)
                {
                    resp = new ViewModels.SlideInfo
                    {
                        LinhVucBaoCao = "",
                        TenBaoCao = "",
                        PhongBan = "",
                        OrderBaoCao = 0
                    };
                }

                return resp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ViewModels.SlideInfo GetSlideInfo(string fileBaoCao)
        {
            try
            {
                var resp = (from xnbc in _context.XacNhanBaoCao
                            join bc in _context.BaoCao on xnbc.BaoCaoId equals bc.Id
                            join dv in _context.DonVi on bc.DonViId equals dv.Id
                            join lv in _context.LinhVucBaoCao on bc.LinhVucId equals lv.Id
                            where xnbc.FileBaoCao == "/fileTableAu/" + fileBaoCao.Replace(".pptx", ".jpg")
                            select new ViewModels.SlideInfo
                            {
                                LinhVucBaoCao = lv.TieuDe,
                                TenBaoCao = bc.TenBaoCao,
                                PhongBan = dv.TenDonVi
                            }).FirstOrDefault();

                if (resp == null)
                {
                    resp = new ViewModels.SlideInfo
                    {
                        LinhVucBaoCao = "",
                        TenBaoCao = "",
                        PhongBan = ""
                    };
                }

                return resp;
            }
            catch (Exception ex)
            {
                return null;
            }           
        }
    }
}
