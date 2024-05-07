using ApplicationDev.Common.Exceptions;
using System.Net;
using ApplicationDev.Modules.User.DTOs;
using ApplicationDev.Modules.User.Entity;
using ApplicationDev.Modules.User.Repos;
using ApplicationDev.Common.Helper.EmailService;

namespace ApplicationDev.Modules.User.Services
{
	public class UserService
	{
		private readonly UserRepository _userRepo;

		private readonly ILogger<UserService> _logger;

		private readonly EmailService _emailService;
		public UserService(UserRepository userRepo, ILogger<UserService> logger, EmailService emailService)
		{
			_userRepo = userRepo;
			_logger = logger;
			_emailService = emailService;
		}


		public async Task<UserEntity> CreateUser(UserCreateDTO data)
		{
			//Check if that user is already registered or not
			//Even user name exists or email exists throw error
			UserEntity? existingUser = await _userRepo.FindOne(x => x.UserName == data.UserName || x.Email == data.Email);

			//If found throw conflict error
			if (existingUser != null)
			{
				throw new HttpException(HttpStatusCode.Conflict, "User Already Exists");
			}

			await _emailService.SendVerificationEmail(data.Email, data.UserName, $"https://localhost:7251/api/user/auth/verify-email/{WebUtility.UrlEncode(data.Email)}");

			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);
			_logger.LogInformation("Hashed Password: " + hashedPassword);

			UserEntity userDataToSend = new UserEntity { Email = data.Email, Name = data.Name, Password = hashedPassword, UserName = data.UserName };

			UserEntity createdUser = await _userRepo.CreateAsync(userDataToSend, true);

			return createdUser;
		}

		public async Task<UserEntity> ActivateUser(UserEntity entity)
		{
			UserEntity updatedUser = await _userRepo.UpdateAsync(entity);
			return updatedUser;
		}
		public async Task<UserEntity?> GetUserByIdAsync(int id)
		{
			return await _userRepo.FindByIdAsync(id);
		}

		async public Task<UserEntity?> FindOneByUserName(string userName)
		{
			return await _userRepo.FindOne(x => x.UserName == userName);
		}
		async public Task<UserEntity?> FindOneByEmail(string email)
		{
			return await _userRepo.FindOne(x => x.Email == email);
		}


	}
}