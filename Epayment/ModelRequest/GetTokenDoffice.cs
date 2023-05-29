using System.Collections.Generic;

namespace Epayment.ModelRequest
{
    public class GetTokenDoffice
    {
        public string UserName { get; set; }
        public string Password { get ;set; }
        public DeviceInfo DeviceInfo {get;set;}
    }
    public class DeviceInfo {
        public string DeviceId { get; set; }
    }
    public class SearchTaiLieuDOffice{
        public IEnumerable<string> url {get;set;}
        public int idDonvi { get; set; }
    }
}