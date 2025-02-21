using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace API.Context
{
    public class DapperContext
    {
        public DapperContext()
        {
            (new AutoMapper()).SetMappings();
        }

        public IDbConnection CreateConnection()
        {
            var conn_str = "Server=R-S-PLBI-9DE2;Database=praktykant;Trusted_Connection=True;TrustServerCertificate=True";

            return new SqlConnection(conn_str);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>(query);
            }
        }

        public async Task<int> ExecuteAsync(string query, object? parms = null)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(query, parms);
            }
        }
    }
}
