// THIS IS ONLY COMPLETED FOR USER SALARY ROUTES
using APIOne.Models;

namespace APIOne.Data;

public class UserRepository : IUserRepository
{
    DataContextEF _EF;

    public UserRepository(IConfiguration config)
    {
        _EF = new DataContextEF(config);
    }

    public UserSalary? GetUserSalary(int id)
    {
        var salary = _EF.UserSalary?.FirstOrDefault<UserSalary>(u => u.UserId == id);
        if (salary != null)
        {
            return salary;
        }
        throw new Exception("Failed to find salary");
    }

    public void AddEntity<T>(T entity)
    {
        if (entity != null)
        {
            _EF.Add(entity);
        }
    }

    public bool SaveChanges()
    {
        return _EF.SaveChanges() > 0;
    }

    public void RemoveEntity<T>(T entity)
    {
        if (entity != null)
        {
            _EF.Remove(entity);
        }
    }
}