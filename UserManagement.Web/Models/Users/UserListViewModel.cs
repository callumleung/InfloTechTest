﻿using System;

namespace UserManagement.Web.Models.Users;

public class UserListViewModel
{
    public List<UserViewModel> Items { get; set; } = new();
}

