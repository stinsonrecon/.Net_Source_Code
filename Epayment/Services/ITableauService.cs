using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;
using BCXN.ViewModels;
namespace BCXN.Services
{
    public interface ITableauService
    {
        OutputDataSignIn GetTokenTableau(string sitename);
        string DownloadTableau(string siteId, string viewId, string token);
        object GetView(string siteId, string viewname, string token);
        object GetTicketTableau(string sitename);
        string ConvertImageToPPTX(string fileImg, string templatePPTX);
        string ConvertPPTXToImage(string filenamePPTX);
        string[] NormalizeSlidesByID(string[] sourcePresentations, int ycycbcId, int[] listid);
        object MergeSlide(string[] sourcePresentations, int ycbcId);
        void DownloadAutoBaoCaoTableau();        
    }
}
