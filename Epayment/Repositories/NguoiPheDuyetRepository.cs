using System;
using System.Collections.Generic;
using System.Linq;
using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;

namespace Epayment.Repositories
{
    public class NguoiPheDuyetRepository : INguoiPheDuyetRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NguoiPheDuyetRepository> _logger;
        public NguoiPheDuyetRepository(ApplicationDbContext context, ILogger<NguoiPheDuyetRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public List<NguoiPheDuyetViewModel> GetLoaiHoSo()
        {
            throw new NotImplementedException();
        }
    }
}