using System;
using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;

namespace Epayment.Repositories
{
    public interface INguoiHuongThuRepository
    {
        List<NguoiHuongThuViewModel> GetListNguoiHuongThu();
    }
}