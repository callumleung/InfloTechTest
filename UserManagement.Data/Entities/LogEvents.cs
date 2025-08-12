using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Data.Entities;
public enum LogEvents
{
    AddUser = 1000,
    EditUser = 1001,
    DeleteUser = 1002,
    ViewUser = 1003,
    FetchUser = 1004,
    FetchAllUsers = 1005,
}
