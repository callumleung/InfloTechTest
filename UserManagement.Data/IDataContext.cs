using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Data;

public interface IDataContext
{
    /// <summary>
    /// Get a list of items
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetAll<TEntity>()
        where TEntity : class;

    Task<IEnumerable<User>> GetActiveUsers(bool active);

    Task<TEntity?> GetById<TEntity>(long id)
        where TEntity : class;

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<TEntity> Create<TEntity>(TEntity entity)
        where TEntity : class;

    /// <summary>
    /// Uodate an existing item matching the ID
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task Update<TEntity>(TEntity entity)
        where TEntity : class;

    Task Delete<TEntity>(TEntity entity)
        where TEntity : class;

    Task<IEnumerable<Log>> GetLogsForUser(long userId);
}
