using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseInMemoryDatabase("UserManagement.Data.DataContext");

    protected override void OnModelCreating(ModelBuilder model) =>
        model
            .Entity<User>()
            .HasData(
                new[]
                {
                    new User
                    {
                        Id = 1,
                        Forename = "Peter",
                        Surname = "Loew",
                        Email = "ploew@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1980, 1, 1),
                    },
                    new User
                    {
                        Id = 2,
                        Forename = "Benjamin Franklin",
                        Surname = "Gates",
                        Email = "bfgates@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1975, 5, 15),
                    },
                    new User
                    {
                        Id = 3,
                        Forename = "Castor",
                        Surname = "Troy",
                        Email = "ctroy@example.com",
                        IsActive = false,
                        DateOfBirth = new DateTime(1985, 3, 10),
                    },
                    new User
                    {
                        Id = 4,
                        Forename = "Memphis",
                        Surname = "Raines",
                        Email = "mraines@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1982, 7, 20),
                    },
                    new User
                    {
                        Id = 5,
                        Forename = "Stanley",
                        Surname = "Goodspeed",
                        Email = "sgodspeed@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1988, 11, 30),
                    },
                    new User
                    {
                        Id = 6,
                        Forename = "H.I.",
                        Surname = "McDunnough",
                        Email = "himcdunnough@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1983, 2, 25),
                    },
                    new User
                    {
                        Id = 7,
                        Forename = "Cameron",
                        Surname = "Poe",
                        Email = "cpoe@example.com",
                        IsActive = false,
                        DateOfBirth = new DateTime(1980, 6, 5),
                    },
                    new User
                    {
                        Id = 8,
                        Forename = "Edward",
                        Surname = "Malus",
                        Email = "emalus@example.com",
                        IsActive = false,
                        DateOfBirth = new DateTime(1978, 4, 18),
                    },
                    new User
                    {
                        Id = 9,
                        Forename = "Damon",
                        Surname = "Macready",
                        Email = "dmacready@example.com",
                        IsActive = false,
                        DateOfBirth = new DateTime(1984, 9, 12),
                    },
                    new User
                    {
                        Id = 10,
                        Forename = "Johnny",
                        Surname = "Blaze",
                        Email = "jblaze@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1981, 8, 22),
                    },
                    new User
                    {
                        Id = 11,
                        Forename = "Robin",
                        Surname = "Feld",
                        Email = "rfeld@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1986, 12, 1),
                    },
                }
            );

    public DbSet<User>? Users { get; set; }
    public DbSet<Log>? Logs { get; set; }

    public async Task<IEnumerable<TEntity>> GetAll<TEntity>()
        where TEntity : class => await base.Set<TEntity>().ToListAsync();

    public async Task<IEnumerable<User>> GetActiveUsers(bool active)
    {
        if (Users == null)
        {
            return Enumerable.Empty<User>();
        }

        return await Users.Where(user => user.IsActive == active).ToListAsync();
    }

    public async Task<TEntity?> GetById<TEntity>(long id)
        where TEntity : class => await base.Set<TEntity>().FindAsync(id);

    public async Task<TEntity> Create<TEntity>(TEntity entity)
        where TEntity : class
    {
        base.Add(entity);
        await SaveChangesAsync();
        return entity;
    }

    public new async Task Update<TEntity>(TEntity entity)
        where TEntity : class
    {
        base.Update(entity);
        await SaveChangesAsync();
    }

    public async Task Delete<TEntity>(TEntity entity)
        where TEntity : class
    {
        base.Remove(entity);
        await SaveChangesAsync();
    }

    public async Task<IEnumerable<Log>> GetLogsForUser(long userId)
    {
        if (Logs == null)
        {
            return Enumerable.Empty<Log>();
        }

        return await Logs.Where(log => log.UserId == userId).ToListAsync();
    }
}
