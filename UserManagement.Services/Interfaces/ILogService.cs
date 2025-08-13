using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Interfaces;

public interface ILogService
{
    Task<IEnumerable<Log>> GetAll();
    Task<IEnumerable<Log>> GetLogsByUser(long id);
}
