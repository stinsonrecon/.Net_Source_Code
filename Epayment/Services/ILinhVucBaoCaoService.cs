using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;
using BCXN.ViewModels;
namespace BCXN.Services
{
    public interface ILinhVucBaoCaoService
    {
        List<LinhVucBaoCaoViewModel> GetLinhVucBaoCao();
        ResponseLinhVucBaoCaoViewModel GetLinhVucBaoCaoWithPagination(ParamGetLinhVucBaoCaoViewModel request);
        ResponsePostViewModel CreateLinhVucBaoCao(LinhVucBaoCaoViewModel request);
        ResponsePostViewModel UpdateLinhVucBaoCao(LinhVucBaoCaoViewModel request);
        ResponsePostViewModel DeleteLinhVucBaoCao(int id);
    }
}
