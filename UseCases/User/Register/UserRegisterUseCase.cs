﻿using Ipe.Domain;
using Ipe.Domain.Errors;
using Ipe.Domain.Models;
using Ipe.UseCases.Interfaces;
using Ipe.UseCases.Interfaces.Repositories;
using BCryptLib = BCrypt.Net.BCrypt;


namespace Ipe.UseCases.Register
{
	public class UserRegisterUseCase: IUseCase<UserRegisterUseCaseInput, UserRegisterUseCaseOutput>
	{
        private readonly IUserRepository _userRepository;
        private readonly IMailVerificationRepository _mailVerificationRepository;
        private readonly IEmailService _emailService;
        private readonly IPlantRepository _plantRepository;

        public UserRegisterUseCase(
            IUserRepository userRepository,
            IMailVerificationRepository mailVerificationRepository,
            IEmailService emailService,
            IPlantRepository plantRepository
        )
        {
            _userRepository = userRepository;
            _mailVerificationRepository = mailVerificationRepository;
            _emailService = emailService;
            _plantRepository = plantRepository;
        }

        public async Task<UserRegisterUseCaseOutput> Run(UserRegisterUseCaseInput Input)
        {
            await VerifyIfUserIsAlreadyRegistered(Input.Email);
            await CreateUser(Input);
            var UserRegistrationToken = await CreateMailVerificationRegister(Input);
            await SendWelcomeEmail(Input.Email, Input.Name, UserRegistrationToken);
            await RecoveryTrees(Input.Email);
            return new UserRegisterUseCaseOutput();            
        }

        private async Task VerifyIfUserIsAlreadyRegistered(string Email)
        {
            var UserAlreadyExists = await _userRepository.GetByEmail(Email);

            if (UserAlreadyExists != null)
                throw new EmailAlreadyRegisteredException();
        }

        private async Task CreateUser(UserRegisterUseCaseInput Input)
        {           
            await _userRepository.Create(new User
            {
                Email = Input.Email,
                Name = Input.Name,
                Password = BCryptLib.HashPassword(Input.Password),
                EmailVerified = false,
                Origin = Origins.WebForest.ToString()
            });
        }

        private async Task RecoveryTrees(string UserEmail)
        {
            var User = await _userRepository.GetByEmail(UserEmail);
            await _plantRepository.RecoveryPlants(User.Id, User.Email);
        }

        private async Task<string> CreateMailVerificationRegister(UserRegisterUseCaseInput Input)
        {
            var UserRegistrationToken = Guid.NewGuid().ToString();

            await _mailVerificationRepository.Create(new MailVerification {
                Role = Roles.User.ToString(),
                Email = Input.Email,
                Activated = false,
                ActivatedAt = null,
                Token = UserRegistrationToken
            });

            return UserRegistrationToken;
        }

        private async Task<bool> SendWelcomeEmail(string Email, string Name, string Token)
        {
            return await _emailService.SendWelcomeEmail(Email, Name, Token, Roles.User.ToString());
        }

    }
}

