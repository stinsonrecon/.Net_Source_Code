using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Statics
{
    public static class ClaimData
    {
        public static List<string> UserClaims { get; set; } = new List<string>
                                                            {
                                                                "Add Customer",
                                                                "Edit Customer",
                                                                "Delete Customer"
                                                            };
    }
}
