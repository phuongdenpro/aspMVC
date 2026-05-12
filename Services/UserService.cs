using CMVC.Models;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public List<User> GetAll()
    {
        return _repo.GetAll();
    }

    public User GetById(int id)
    {
        return _repo.GetById(id);
    }

    public void Create(User user)
    {
        _repo.Add(user);
    }

    public void Update(User user)
    {
        _repo.Update(user);
    }

    public void Delete(int id)
    {
        var user = _repo.GetById(id);
        if (user != null)
            _repo.Delete(user);
    }
}