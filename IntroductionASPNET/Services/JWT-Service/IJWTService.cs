namespace IntroductionASPNET.Services.JWT_Service
{
    public interface IJWTService
    {
        Task<User> RegisterUser(UserDTO request, User user);
        Task<string> UserLogin(UserDTO request, User user, string token);
    }
}
