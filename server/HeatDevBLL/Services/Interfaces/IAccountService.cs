using HeatDevBLL.Models.DTO;
using HeatDevBLL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeatDevBLL.Services
{
    public interface IAccountService
    {
        Task<bool> UserExistsAsync(string login);
        Task<User> FindUserByIdAsync(int id);
        //Task<UserProfile> GetUserProfileAsync(int userId);
        Task<bool> PasswordIsValid(string login, string password);

        Task<User> SignUpUserAsync(UserSignUpDTO userData);
        Task<User> SignInUserAsync(UserSignInDTO userData);

        Task InvalidateTokenAsync(int userId, string refreshToken);
        Task StoreRefreshTokenAsync(User user, string refreshToken);
    }
}
