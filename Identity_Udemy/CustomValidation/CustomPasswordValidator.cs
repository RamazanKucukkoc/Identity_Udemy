﻿using Identity_Udemy.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity_Udemy.CustomValidation
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                if (!user.Email.Contains(user.UserName))
                {
                    errors.Add(new IdentityError() { Code = "PasswordContainsUserName", Description = "şifre alanı kullanıcı adı içermez" });
                }
                errors.Add(new IdentityError()
                { Code = "PasswordContainsUserName", Description = "şifre alanı kullanıcı adı içermez" });

            }
            if (password.ToLower().Contains("1234"))
            {
                errors.Add(new IdentityError()
                { Code = "PasswordContains1234", Description = "şifre alanı ardışık sayı içermez" });

            }
            if (password.ToLower().Contains(user.Email.ToLower()))
            {
                errors.Add(new IdentityError() { Code = "PasswordContainsEmail", Description = "şifre alanı email adresiniz içermez" });
            }
            if (errors.Count==0)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
        }
    }
}
