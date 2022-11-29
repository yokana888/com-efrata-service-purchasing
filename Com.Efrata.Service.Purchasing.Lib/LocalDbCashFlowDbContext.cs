using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib
{
    public class LocalDbCashFlowDbContext : ILocalDbCashFlowDbContext
    {
        private readonly SqlConnection _connection;

        public LocalDbCashFlowDbContext(string connectionString)
        {
            _connection = CreateConnection(connectionString);
            _connection.Open();
        }

        private SqlConnection CreateConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString empty");
            }

            return new SqlConnection(connectionString);
        }

        public IDataReader ExecuteReader(string query, ICollection<SqlParameter> parameters)
        {


            SqlCommand command = new SqlCommand(query, _connection);
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command.ExecuteReader();
        }
        public IDataReader ExecuteReaderOnlyQuery(string query)
        {

            SqlCommand command = new SqlCommand(query, _connection);

            return command.ExecuteReader();
        }
    }
    public interface ILocalDbCashFlowDbContext
    {
        IDataReader ExecuteReader(string query, ICollection<SqlParameter> parameters);
        IDataReader ExecuteReaderOnlyQuery(string query);
    }

}
