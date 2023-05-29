using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;
using BCXN.ViewModels;

namespace BCXN.Services
{
    public interface IChucNangService
    {        
        List<ChucNangViewModel> GetAllChucNang(string NhomQuyenId);
        ResponseWithPaginationViewModel GetChucNang(ChucNangSearchViewModel request);
        ResponsePostViewModel CreateChucNang(ChucNangGetViewModel request);
        ResponsePostViewModel UpdateChucNang(ChucNangGetViewModel request);
        ResponsePostViewModel DeleteChucNang(int id);
    }
}
