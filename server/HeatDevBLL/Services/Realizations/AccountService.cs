﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HeatDevBLL.Models;
using HeatDevBLL.Models.DTO;
using HeatDevBLL.Models.Entities;
using LinqToDB;

namespace HeatDevBLL.Services
{
    public class AccountService : IAccountService
    {
        public async Task<bool> UserExistsAsync(string login)
        {
            using (var db = new DBContext())
            {
                return await db.Users.FirstOrDefaultAsync(u => u.Login == login) != null;
            }
        }

        public async Task<UserProfile> GetUserProfileAsync(int userId)
        {
            using (var db = new DBContext())
            {
                return await db.UsersProfiles.FirstOrDefaultAsync(u => u.Id == userId);
            }
        }

        public async Task<User> FindUserByIdAsync(int id)
        {
            using (var db = new DBContext())
            {
                return await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
        }

        public async Task<User> SignInUserAsync(UserSignInDTO userData)
        {
            using (var db = new DBContext())
            {
                string hash = GeneratePassword(userData.Login, userData.Password);

                return await db.Users.FirstOrDefaultAsync(u => u.Login == userData.Login && u.Hash == hash);
            }
        }

        public async Task<User> SignUpUserAsync(UserSignUpDTO userData)
        {
            var user = Mapper.Map<User>(userData);
            user.Hash = GeneratePassword(userData.Login, userData.Password);

            user.Roles = new string[] { "user", "client" };

            using (var db = new DBContext())
            {
                int userId = await db.InsertWithInt32IdentityAsync(user);
                User createdUser = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                var userProfile = Mapper.Map<UserProfile>(userData);
                userProfile.Id = createdUser.Id;
                if (String.IsNullOrEmpty(userData.Avatar))
                {
                    userProfile.Avatar = "https://www.drupal.org/files/issues/default-avatar.png";
                }

                await db.InsertAsync(userProfile);

                return createdUser;
            }
        }

        public async Task StoreRefreshTokenAsync(User user, string refreshToken)
        {
            user.RefreshToken = refreshToken;

            using (var db = new DBContext())
            {
                await db.UpdateAsync(user);
            }
        }

        public async Task InvalidateTokenAsync(int userId, string refreshToken)
        {
            using (var db = new DBContext())
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.RefreshToken == refreshToken);
                if (user == null)
                {
                    return;
                }

                user.RefreshToken = String.Empty;

                await db.UpdateAsync(user);
            }
        }

        public async Task<bool> TokenIsValidAsync(int userId, string refreshToken)
        {
            using (var db = new DBContext())
            {
                return await db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.RefreshToken == refreshToken) != null;
            }
        }

        private string GeneratePassword(string arg1, string arg2)
        {
            SHA512 sha512 = SHA512.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(arg1 + "pepper" + arg2);
            byte[] hash = sha512.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        public async Task UpdateUserProfileAsync(UserProfile profile, UserProfileDTO newProfile)
        {
            profile.Avatar = newProfile.Avatar;
            profile.Name = newProfile.Name;
            profile.Surname = newProfile.Surname;
            profile.Phone = newProfile.Phone;

            using (var db = new DBContext())
            {
                await db.UpdateAsync(profile);
            }
        }

        public async Task ChangeUserPasswordAsync(User user, string password)
        {
            string hash = GeneratePassword(user.Login, password);
            user.Hash = hash;

            using (var db = new DBContext())
            {
                await db.UpdateAsync(user);
            }
        }

        public bool PasswordIsValidAsync(User user, string password) => user.Hash == GeneratePassword(user.Login, password);
    }
}
