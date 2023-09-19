using APIOne.Models;

namespace APIOne.Data;

public interface IUserRepository
{
    public UserSalary? GetUserSalary(int userId);
    public bool SaveChanges();
    public void AddEntity<T>(T entity);
    public void RemoveEntity<T>(T entity);

}
