using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public interface ITinhThanhPhoService
    {
        List<TinhThanhPhoViewModel> GetTinhThanhPho();
        ResponseTinhThanhPhoViewModel GetTinhThanhPhoWithPagination(ParamGetTinhThanhPhoViewModel request);
        ResponsePostViewModel CreateTinhThanhPho(TinhThanhPhoViewModel tinhTP);
        ResponsePostViewModel UpdateTinhThanhPho(TinhThanhPhoViewModel tinhTP);
        ResponsePostViewModel DeleteTinhThanhPho(Guid id);
    }
}