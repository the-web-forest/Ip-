﻿using Ipe.Domain.Models;
using Ipe.Domain.Errors;
using Ipe.UseCases.Interfaces;
using BCryptLib = BCrypt.Net.BCrypt;
using Ipe.Domain;
using Ipe.UseCases.Login;
using Ipe.UseCases.Interfaces.Services;
using Ipe.External.Services.DTOs;
using Ipe.UseCases.Register;

namespace Ipe.UseCases.GoogleLogin
{
    public class GoogleLoginUseCase : IUseCase<GoogleLoginUseCaseInput, LoginUseCaseOutput>
    {

        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IGoogleService _googleService;

        public GoogleLoginUseCase(
            IAuthService authService,
            IUserRepository userRepository,
            IGoogleService googleService
        )
        {
            _authService = authService;
            _userRepository = userRepository;
            _googleService = googleService;
        }

        public async Task<LoginUseCaseOutput> Run(GoogleLoginUseCaseInput Input)
        {

            var GoogleUser = await _googleService.GetUserInfoByGoogleToken(Input.Token);

            if(GoogleUser.Email is null)
            {
                throw new InvalidEmailException();
            }

            var User = await _userRepository.GetByEmail(GoogleUser.Email);

            if (User is null)
            {
                await CreateUser(GoogleUser);
                User = await _userRepository.GetByEmail(GoogleUser.Email);
            }

            if (User.EmailVerified is false)
            {
                await UpdateUserWithGoogleData(GoogleUser, User);
                User = await _userRepository.GetByEmail(GoogleUser.Email);
            }

            ValidateUser(GoogleUser, User);

            return BuildResponse(User);
        }

        private async Task CreateUser(GoogleUserResponse GoogleUser)
        {
            await _userRepository.Create(new User
            {
                Email = GoogleUser.Email,
                Name = GoogleUser.Name,
                Password = BCryptLib.HashPassword(new Random().Next().ToString()),
                City = "",
                State = "",
                EmailVerified = true,
                Origin = Origins.Google.ToString()
            });
        }

        private static void ValidateUser(GoogleUserResponse GoogleUser, User User)
        {
            if (GoogleUser.EmailVerified == "false")
                throw new UnverifiedEmailException();

            if (User is null)
                throw new InvalidPasswordException();

            if (User.EmailVerified is false)
                throw new UnverifiedEmailException();
        }

        private LoginUseCaseOutput BuildResponse(User User)
        {
            return new LoginUseCaseOutput
            {
                AccessToken = _authService.GenerateToken(User, Roles.User),
                TokenType = "Bearer",
                User = new OutputUser
                {
                    Id = User.Id,
                    Email = User.Email,
                    Name = User.Name
                }
            };
        }

        private async Task UpdateUserWithGoogleData(GoogleUserResponse GoogleUser, User User)
        {
            User.EmailVerified = true;
            User.Origin = Origins.Google.ToString();
            await _userRepository.Update(User);
        }
    }
}