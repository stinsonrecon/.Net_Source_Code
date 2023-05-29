using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Repositories
{
    public interface ITableAuRepository
    {
        ViewModels.SlideInfo GetSlideInfo(string fileBaoCao);

        ViewModels.SlideInfo GetSlideInfoByID(int bcid);
    }
}
