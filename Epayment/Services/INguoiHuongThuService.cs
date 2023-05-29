using System;
using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public interface INguoiHuongThuService
    {
        List<NguoiHuongThuViewModel> GetListNguoiHuongThu();
    }
}