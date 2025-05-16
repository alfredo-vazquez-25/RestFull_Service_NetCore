using Dapper.Contrib.Extensions;
using Northwind.Repositories;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace Northwind.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected string _connectionString;
        public Repository(string connectionString)
        {
            SqlMapperExtensions.TableNameMapper = (type) => { return $"{type.Name}"; };
            _connectionString = connectionString;
        }
        public bool Delete(T entity)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                return connection.Delete(entity);
            }
        }

        public T GetById(int id)
        {
            const int maxRetries = 3;
            int attempt = 0;

            while (true)
            {
                try
                {
                    var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
                    connection.Open();
                    return connection.Get<T>(id);
                }
                catch (TypeInitializationException tex) when (attempt++ < maxRetries)
                {
                    // Reiniciar la resolución de ensamblados
                    SqlDependencyResolver.Initialize();
                    Thread.Sleep(100 * attempt); // Espera progresiva
                }
                catch (Exception ex)
                {
                    // Log detallado del error
                    Console.WriteLine($"[FATAL SQL ERROR] {ex}");
                    throw;
                }
            }
        }

        public IEnumerable<T> GetList()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.GetAll<T>();
            }
        }

        public int Insert(T entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return (int)connection.Insert(entity);
            }
        }

        public bool Update(T entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Update(entity);
            }
        }
    }
}
