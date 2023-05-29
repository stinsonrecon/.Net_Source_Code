using BCXN.Models;
using BCXN.ViewModels;
using System.Collections.Generic;

namespace BCXN.Repositories
{
    public interface ILinhVucBaoCaoRepository
    {
        List<LinhVucBaoCaoViewModel> GetLinhVucBaoCao();
        ResponseLinhVucBaoCaoViewModel GetLinhVucWithPagination(ParamGetLinhVucBaoCaoViewModel request);
        ResponsePostViewModel CreateLinhVucBaocao(LinhVucBaoCaoViewModel linhVuc);
        ResponsePostViewModel UpdateLinhVucBaoCao(LinhVucBaoCaoViewModel linhVuc);
        ResponsePostViewModel DeleteLinhVucBaoCao(int id);
    }
}
