using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    Task<IEnumerable<User>> FilterByActive(bool isActive);
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetUser(long id);
    Task<User> AddUser(User user);
    Task UpdateUser(User user);
    Task DeleteUser(User user);
}
