using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace APIERP.DataContext
{
    public class ConnectionContext : IConnectionContext
    {
        private readonly IConfiguration _configuration;

        public ConnectionContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public OracleConnection GetConnection()
        {
            string conString = _configuration.GetConnectionString("DefaultConnection");
            OracleConnection conn = new OracleConnection(conString);
            return conn;
        }
    }

    public interface IConnectionContext
    {
        public OracleConnection GetConnection();
    }
}
