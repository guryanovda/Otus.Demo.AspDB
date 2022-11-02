using System.Data;
using System.Data.Common;
using EFCore.NamingConventions.Internal;
using WebApplication2.Abstractions;

namespace WebApplication2.Repos
{
    public class AdoRepository<T>: IRepository<T> where T: BaseEntity
    {
        private readonly DbConnection _connection;
        private readonly INameRewriter _nameRewriter;
        public AdoRepository(DbConnection connection, INameRewriter nameRewriter)
        {
            _connection = connection;
            _nameRewriter = nameRewriter;
            if(_connection.State != ConnectionState.Open) _connection.Open();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var result = new List<T>();
            await using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM {typeof(T).Name.ToLower()}s";
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var item = Activator.CreateInstance<T>();
                        foreach (var elem in item.GetType().GetProperties())
                        {
                            elem.SetValue(item, reader[_nameRewriter.RewriteName(elem.Name)]);
                        }
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public async Task<T?> GetAsync(Guid id)
        {
            T result = Activator.CreateInstance<T>();
            await using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM {typeof(T).Name.ToLower()}s WHERE Id = '{id}'";
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        foreach (var elem in result.GetType().GetProperties())
                        {
                            elem.SetValue(result, reader[_nameRewriter.RewriteName(elem.Name)]);
                        }
                    }
                }
            }
            return result;
        }

        public async Task AddAsync(T entity)
        {
            await using (var cmd = _connection.CreateCommand())
            {
                string fields = "";
                string values = "";
                foreach (var elem in entity.GetType().GetProperties())
                {
                    if (!string.IsNullOrEmpty(fields))
                    {
                        fields += ",";
                        values += ",";
                    }

                    fields += _nameRewriter.RewriteName(elem.Name);
                    values += $"'{elem.GetValue(entity)}'";
                }

                cmd.CommandText = $"INSERT INTO {typeof(T).Name.ToLower()}s ({fields}) VALUES ({values})";
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            await using (var cmd = _connection.CreateCommand())
            {
                string values = "";
                foreach (var elem in entity.GetType().GetProperties().Where(x => x.Name != "Id"))
                {
                    if (!string.IsNullOrWhiteSpace(values))
                    {
                        values += ",";
                    }

                    values += $"{_nameRewriter.RewriteName(elem.Name)} = '{elem.GetValue(entity)}'";
                }

                cmd.CommandText = $"UPDATE {typeof(T).Name.ToLower()}s SET {values} WHERE Id ='{entity.Id}'";
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            await using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = $"DELETE FROM {typeof(T).Name.ToLower()}s WHERE Id='{id}'";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
