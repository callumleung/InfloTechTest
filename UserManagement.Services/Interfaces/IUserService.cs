using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    IEnumerable<User> FilterByActive(bool isActive);
    IEnumerable<User> GetAll();
    User GetUser(long id);
    void AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);
}
