using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;
internal class LogService : ILogService
{
    private readonly IDataContext _dataAccess;
    public LogService(IDataContext dataAccess) => _dataAccess = dataAccess;

    public async Task<IEnumerable<Log>> GetLogs() => await _dataAccess.GetAll<Log>();

    public async Task<IEnumerable<Log>> GetLogsByUser(long id) => await _dataAccess.GetLogsForUser(id);
}
