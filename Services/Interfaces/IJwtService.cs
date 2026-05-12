using CMVC.Models;


    public interface IJwtService
    {
        string CreateAccessToken(User user, IConfiguration config);
    }

