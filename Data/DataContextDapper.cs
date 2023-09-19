using System.Data.Common;
using Dapper;
using Microsoft.Data.SqlClient;

namespace APIOne.Data;

public class DataContextDapper
{
    private readonly IConfiguration _config;

    public DataContextDapper(IConfiguration config)
    {
        _config = config;
    }

    public IEnumerable<T> LoadData<T>(string sql)
    {
        DbConnection connection = new SqlConnection(_config.GetConnectionString("Default"));
        return connection.Query<T>(sql);
    }

    public T? LoadDataSingle<T>(string sql)
    {
        DbConnection connection = new SqlConnection(_config.GetConnectionString("Default"));
        return connection.QuerySingle<T>(sql);
    }

    public bool ExecuteSql(string sql)
    {
        DbConnection connection = new SqlConnection(_config.GetConnectionString("Default"));
        return connection.Execute(sql) > 0;
    }

    public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
    {
        var commandWithParams = new SqlCommand(sql);

        foreach(SqlParameter p in parameters)
        {
            commandWithParams.Parameters.Add(p);
        }
        
        SqlConnection connection = new SqlConnection(_config.GetConnectionString("Default"));
        connection.Open();
        
        commandWithParams.Connection = connection;
        
        var rows = commandWithParams.ExecuteNonQuery();

        connection.Close();

        return(rows > 0);
    }
}