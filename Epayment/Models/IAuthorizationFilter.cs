using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BCXN.Models
{
public class Policies
    {
        // yêu cầu báo cáo
        // public const string YCBG_GET = "yeucaubaogia.get";
        public const string YCBC_CREATE = "ycbc.create";
        public const string YCBC_UPDATE = "ycbc.update";
        public const string YCBC_XACNHAN = "ycbc.xacnhan";
        public const string YCBC_DELETE = "ycbc.delete";
        public const string YCBC_DETAIL = "ycbc.detail";
        public const string YCBC_HISTORY = "ycbc.history";
        public const string YCBC_VIEW_THONGKE = "ycbc.thongke";
        // public static AuthorizationPolicy YcbgGetPolicy()
        // {
        //     return new AuthorizationPolicyBuilder()
        //         .RequireAuthenticatedUser()
        //         .RequireRole(YCBG_GET)
        //         .Build();
        // }
        public static AuthorizationPolicy YcbcCreatePolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(YCBC_CREATE)
                .Build();
        }

        public static AuthorizationPolicy YcbcUpdatePolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(YCBC_UPDATE)
                .Build();
        }

        public static AuthorizationPolicy YcbcXacnhanPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(YCBC_XACNHAN)
                .Build();
        }
        public static AuthorizationPolicy YcbcDeletePolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(YCBC_DELETE)
                .Build();
        }

        public static AuthorizationPolicy YcbcDetailPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(YCBC_DETAIL)
                .Build();
        }
        public static AuthorizationPolicy YcbcHistoryPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(YCBC_HISTORY)
                .Build();
        }
        public static AuthorizationPolicy YcbcThongkePolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(YCBC_VIEW_THONGKE)
                .Build();
        }
    }
}
