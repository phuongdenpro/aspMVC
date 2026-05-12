using CMVC.Models;

public interface IUserService
{
    List<User> GetAll();
    User GetById(int id);
    void Create(User user);
    void Update(User user);
    void Delete(int id);
}