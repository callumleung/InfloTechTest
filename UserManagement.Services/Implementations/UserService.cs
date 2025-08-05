using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public async Task<IEnumerable<User>> FilterByActive(bool isActive) => await _dataAccess.GetByActive<User>(isActive);
    public async Task<IEnumerable<User>> GetAll() => await _dataAccess.GetAll<User>();
    public async Task<User?> GetUser(long id) => await _dataAccess.GetById<User>(id);
    public async Task AddUser(User user) => await _dataAccess.Create<User>(user);
    public async Task UpdateUser(User user) => await _dataAccess.Update<User>(user);
    public async Task DeleteUser(User user) => await _dataAccess.Delete<User>(user);
}
